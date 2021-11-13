using Common_ClassLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public class FullRunner
    {
        public Parameters Parameters { get; set; }
        public JourneyRetrieverComponents Components { get; set; }
        public IFileIO FileIO { get; set; }
        public IExcelPrinter Printer { get; set; }
        public IFlightConnectionsDotComWorker_AirportCollector AirportCollector { get; set; }
        public IFlightConnectionsDotComWorker_AirportPopulator AirportPopulator { get; set; }
        public IDateTimeProvider DateTimeProvider { get; set; }
        public IMultiJourneyCollector MultiJourneyCollector { get; set; }

        public FullRunner(
            JourneyRetrieverComponents components,
            IFileIO fileIO,
            IDateTimeProvider dateTimeProvider,
            IExcelPrinter printer,
            IFlightConnectionsDotComWorker_AirportCollector airportCollector,
            IFlightConnectionsDotComWorker_AirportPopulator airportPopulator,
            IMultiJourneyCollector multiJourneyCollector)
        {
            FileIO = fileIO;
            Printer = printer;
            AirportCollector = airportCollector;
            AirportPopulator = airportPopulator;
            DateTimeProvider = dateTimeProvider;
            Components = components;
            MultiJourneyCollector = multiJourneyCollector;
        }

        public async Task<bool> DoRun(Parameters paramss)
        {
            Parameters = paramss;

            string runSummary = $"{Parameters.Origins.ConcatenateListOfStringsToCommaString()} - {Parameters.Destinations.ConcatenateListOfStringsToCommaString()}";
            runSummary += " - " + Parameters.DateFrom.ToString("yyyy-MM-dd");
            runSummary += " - " + Parameters.DateTo.ToString("yyyy-MM-dd");
            string runId = Globals.GetDateTimeFileNameFriendlyConcatenatedWithString(DateTimeProvider.Now(), runSummary);
            string runResultsPath = System.IO.Path.Combine(Parameters.FileSavePath, runId);
            if (!FileIO.DirectoryExists(runResultsPath)) FileIO.CreateDirectory(runResultsPath);

            List<Airport> airportsList;
            if (!Parameters.LocalAirportListFile.IsNullOrEmpty())
            {
                airportsList = FileIO.ReadAllText(Parameters.LocalAirportListFile).DeserializeObject<List<Airport>>();
            }
            else
            {
                airportsList = AirportCollector.CollectAirports();
                FileIO.WriteAllText($"{runResultsPath}\\{runId}_airportList.json", airportsList.SerializeObject(Formatting.Indented));
            }

            IAirportFilterer filterer = new NoFilterer();
            if (Parameters.EuropeOnly) filterer = new EuropeFilterer();
            else if (Parameters.UKAndBulgariaOnly) filterer = new UKBulgariaFilterer();

            Dictionary<string, HashSet<string>> airportsAndDestinations;
            if (!Parameters.LocalAirportDestinationsFile.IsNullOrEmpty())
            {
                airportsAndDestinations = FileIO.ReadAllText(Parameters.LocalAirportDestinationsFile).DeserializeObject<Dictionary<string, HashSet<string>>>();
            }
            else
            {
                airportsAndDestinations = AirportPopulator.PopulateAirports(airportsList, filterer);
                FileIO.WriteAllText($"{runResultsPath}\\{runId}_airportDestinations.json", airportsAndDestinations.SerializeObject(Formatting.Indented));
            }

            AirportListFilterer airportListFilterer = new(airportsList);
            Dictionary<string, HashSet<string>> filteredAirports = airportListFilterer.FilterAirports(airportsAndDestinations, filterer);

            AirportPathGenerator generator = new(filteredAirports);
            List<Path> paths = generator.GeneratePaths(Parameters.Origins, Parameters.Destinations, Parameters.MaxFlights, Parameters.OnlyIncludeShortestPaths);
            List<List<string>> pathsDetailed = new();
            foreach (Path path in paths)
            {
                List<string> pathDetailed = new();
                for (int i = 0; i < path.Count(); i++) pathDetailed.Add(airportsList.FirstOrDefault(x => x.Code.Equals(path[i])).ToString());
                pathsDetailed.Add(pathDetailed);
            }
            FileIO.WriteAllText($"{runResultsPath}\\{runId}_latestPaths.json", pathsDetailed.SerializeObject(Formatting.Indented));

            if (Parameters.OnlyPrintPaths)
            {
                SaveLogAndQuitDriver(runId, runResultsPath);
                return true;
            }

            PathsToDirectPathGroupsConverter converter = new();
            Dictionary<string, JourneyRetrieverData> results = converter.GetGroups(paths);

            MultiJourneyCollectorResults journeyCollectorResults = await MultiJourneyCollector.GetJourneys(Components, results, Parameters.DateFrom, Parameters.DateTo);
            //JourneyCollection journeys = 
            //    FileIO.ReadAllText(@"C:\D\FlightConnectionsDotCom\Results\2021-11-11--22-17-16_ABZ - VAR - 2021-01-01 - 2021-01-05\2021-11-11--22-17-16_ABZ - VAR - 2021-01-01 - 2021-01-05_journeys.json").DeserializeObject<JourneyCollection>();
            FileIO.WriteAllText($"{runResultsPath}\\{runId}_journeyCollectorResults.json", journeyCollectorResults.SerializeObject(Formatting.Indented));
            PrintPathsAndJourneysAndFinish(airportsList, journeyCollectorResults, runId, runResultsPath, paths);
            return true;
        }

        private void PrintPathsAndJourneysAndFinish(List<Airport> airportsList, MultiJourneyCollectorResults journeyCollectorResults, string runId, string runResultsPath, List<Path> paths)
        {
            SequentialJourneyCollectionBuilder builder = new();
            List<SequentialJourneyCollection> results = builder.GetFullPathCombinationOfJourneys(paths, journeyCollectorResults.JourneyCollection);

            DataTableCreator dtCreator = new();
            Printer.PrintTablesToWorksheet(dtCreator.GetTables(airportsList, results, Parameters.SkipUndoableJourneys, Parameters.SkipNotSameDayFinishJourneys, Parameters.NoLongerThan), $"{runResultsPath}\\{runId}_results.xlsx");
            Components.Logger.Log($"Saved files to {runResultsPath}");
            SaveLogAndQuitDriver(runId, runResultsPath);
        }

        private void SaveLogAndQuitDriver(string runId, string runResultsPath)
        {
            FileIO.WriteAllText($"{runResultsPath}\\{runId}_log.txt", Components.Logger.GetContent().SerializeObject(Formatting.Indented));
        }
    }
}