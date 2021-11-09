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
        public ILogger Logger { get; set; }
        public IDelayer Delayer { get; set; }
        public IFileIO FileIO { get; set; }
        public IExcelPrinter Printer { get; set; }
        public IFlightConnectionsDotComWorker_AirportCollector AirportCollector { get; set; }
        public IFlightConnectionsDotComWorker_AirportPopulator AirportPopulator { get; set; }
        public IGoogleFlightsWorker ChromeWorker { get; set; }
        public IDateTimeProvider DateTimeProvider { get; set; }

        public FullRunner(
            ILogger logger,
            IDelayer delayer,
            IFileIO fileIO,
            IDateTimeProvider dateTimeProvider,
            IExcelPrinter printer,
            IFlightConnectionsDotComWorker_AirportCollector airportCollector,
            IFlightConnectionsDotComWorker_AirportPopulator airportPopulator,
            IGoogleFlightsWorker chromeWorker)
        {
            Logger = logger;
            Delayer = delayer;
            FileIO = fileIO;
            Printer = printer;
            AirportCollector = airportCollector;
            AirportPopulator = airportPopulator;
            ChromeWorker = chromeWorker;
            DateTimeProvider = dateTimeProvider;
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

            GoogleFlightsWorkerResults chromeWorkerResults;
            if (!Parameters.LocalGoogleFlightsWorkerResultsFile.IsNullOrEmpty())
            {
                chromeWorkerResults = FileIO.ReadAllText(Parameters.LocalGoogleFlightsWorkerResultsFile).DeserializeObject<GoogleFlightsWorkerResults>();
                PrintPathsAndJourneysAndFinish(airportsList, chromeWorkerResults.FullPathsAndJourneyCollections, runId, runResultsPath);
                return true;
            }
            else if (!Parameters.OpenGoogleFlights)
            {
                SaveLogAndQuitDriver(runId, runResultsPath);
                return true;
            }

            Dictionary<string, JourneyCollection> collectedPathJourneys = new();
            if (!Parameters.LocalCollectedPathJourneysFile.IsNullOrEmpty()) collectedPathJourneys = FileIO.ReadAllText(Parameters.LocalCollectedPathJourneysFile).DeserializeObject<GoogleFlightsWorkerResults>().PathsAndJourneys;
            chromeWorkerResults = await ChromeWorker.ProcessPaths(paths, Parameters.DateFrom, Parameters.DateTo, Parameters.DefaultDelay, collectedPathJourneys);
            FileIO.WriteAllText($"{runResultsPath}\\{runId}_pathsAndJourneys.json", chromeWorkerResults.SerializeObject(Formatting.Indented));
            PrintPathsAndJourneysAndFinish(airportsList, chromeWorkerResults.FullPathsAndJourneyCollections, runId, runResultsPath);
            return chromeWorkerResults.Success;
        }

        private void PrintPathsAndJourneysAndFinish(List<Airport> airportsList, List<FullPathAndListOfPathsAndJourneyCollections> pathsAndJourneys, string runId, string runResultsPath)
        {
            SequentialJourneyCollectionBuilder builder = new();
            List<SequentialJourneyCollection> results = new();
            foreach (FullPathAndListOfPathsAndJourneyCollections pathAndJourney in pathsAndJourneys)
            {
                results.AddRange(builder.GetFullPathCombinationOfJourneys(pathAndJourney));
            }

            DataTableCreator dtCreator = new();
            Printer.PrintTablesToWorksheet(dtCreator.GetTables(airportsList, results, Parameters.SkipUndoableJourneys, Parameters.SkipNotSameDayFinishJourneys, Parameters.NoLongerThan), $"{runResultsPath}\\{runId}_results.xlsx");
            Logger.Log($"Saved files to {runResultsPath}");
            SaveLogAndQuitDriver(runId, runResultsPath);
        }

        private void SaveLogAndQuitDriver(string runId, string runResultsPath)
        {
            FileIO.WriteAllText($"{runResultsPath}\\{runId}_log.txt", Logger.GetContent().SerializeObject(Formatting.Indented));
            //Driver.Quit();
        }
    }
}