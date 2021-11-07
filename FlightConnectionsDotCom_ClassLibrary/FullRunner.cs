using Common_ClassLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
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
        public IChromeWorker ChromeWorker { get; set; }
        public IDateTimeProvider DateTimeProvider { get; set; }

        public FullRunner(
            ILogger logger,
            IDelayer delayer,
            IFileIO fileIO,
            IDateTimeProvider dateTimeProvider,
            IExcelPrinter printer,
            IFlightConnectionsDotComWorker_AirportCollector airportCollector,
            IFlightConnectionsDotComWorker_AirportPopulator airportPopulator,
            IChromeWorker chromeWorker)
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

            ChromeWorkerResults chromeWorkerResults;
            if (!Parameters.LocalChromeWorkerResultsFile.IsNullOrEmpty())
            {
                chromeWorkerResults = FileIO.ReadAllText(Parameters.LocalChromeWorkerResultsFile).DeserializeObject<ChromeWorkerResults>();
                PrintPathsAndFlightsAndFinish(airportsList, chromeWorkerResults.FullPathsAndFlightCollections, runId, runResultsPath);
                return true;
            }
            else if (!Parameters.OpenGoogleFlights)
            {
                SaveLogAndQuitDriver(runId, runResultsPath);
                return true;
            }

            Dictionary<string, FlightCollection> collectedPathFlights = new();
            if (!Parameters.LocalCollectedPathFlightsFile.IsNullOrEmpty()) collectedPathFlights = FileIO.ReadAllText(Parameters.LocalCollectedPathFlightsFile).DeserializeObject<ChromeWorkerResults>().PathsAndFlights;
            chromeWorkerResults = await ChromeWorker.ProcessPaths(paths, Parameters.DateFrom, Parameters.DateTo, Parameters.DefaultDelay, collectedPathFlights);
            FileIO.WriteAllText($"{runResultsPath}\\{runId}_pathsAndFlights.json", chromeWorkerResults.SerializeObject(Formatting.Indented));
            PrintPathsAndFlightsAndFinish(airportsList, chromeWorkerResults.FullPathsAndFlightCollections, runId, runResultsPath);
            return chromeWorkerResults.Success;
        }

        private void PrintPathsAndFlightsAndFinish(List<Airport> airportsList, List<FullPathAndListOfPathsAndFlightCollections> pathsAndFlights, string runId, string runResultsPath)
        {
            FullPathCombinationOfFlightsCollector flightCollector = new();
            List<SequentialFlightCollection> results2 = new();
            foreach (FullPathAndListOfPathsAndFlightCollections pathAndFlights in pathsAndFlights)
            {
                results2.AddRange(flightCollector.GetFullPathCombinationOfFLights(pathAndFlights));
            }

            DataTableCreator dtCreator = new();
            Printer.PrintTablesToWorksheet(dtCreator.GetTables(airportsList, results2, Parameters.SkipUndoableFlights, Parameters.SkipNotSameDayFinishFlights, Parameters.NoLongerThan), $"{runResultsPath}\\{runId}_results.xlsx");
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