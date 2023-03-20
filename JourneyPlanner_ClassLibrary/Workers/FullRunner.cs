using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.AirportFilterers;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Interfaces;
using Newtonsoft.Json;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class FullRunner
    {
        public Parameters Parameters { get; set; }
        public JourneyRetrieverComponents Components { get; set; }
        public IFileIO FileIO { get; set; }
        public IExcelPrinter Printer { get; set; }
        public IFlightConnectionsDotComWorkerAirportCollector AirportCollector { get; set; }
        public IFlightConnectionsDotComWorkerAirportPopulator AirportPopulator { get; set; }
        public IDateTimeProvider DateTimeProvider { get; set; }
        public IMultiJourneyCollector MultiJourneyCollector { get; set; }

        public FullRunner(
            JourneyRetrieverComponents components,
            IFileIO fileIO,
            IDateTimeProvider dateTimeProvider,
            IExcelPrinter printer,
            IFlightConnectionsDotComWorkerAirportCollector airportCollector,
            IFlightConnectionsDotComWorkerAirportPopulator airportPopulator,
            IMultiJourneyCollector multiJourneyCollector
        )
        {
            FileIO = fileIO;
            Printer = printer;
            AirportCollector = airportCollector;
            AirportPopulator = airportPopulator;
            DateTimeProvider = dateTimeProvider;
            Components = components;
            MultiJourneyCollector = multiJourneyCollector;
        }

        public async Task DoRun(Parameters paramss)
        {
            Parameters = paramss;

            string runSummary =
                $"{Parameters.Origins.ConcatenateListOfStringsToCommaAndSpaceString()} - {Parameters.Destinations.ConcatenateListOfStringsToCommaAndSpaceString()}";
            runSummary += " - " + Parameters.DateFrom.ToString("yyyy-MM-dd");
            runSummary += " - " + Parameters.DateTo.ToString("yyyy-MM-dd");
            string runId =
                Globals.GetDateTimeFileNameFriendlyConcatenatedWithString(DateTimeProvider.Now(), runSummary);
            string runResultsPath = System.IO.Path.Combine(Parameters.FileSavePath, runId);
            if (!FileIO.DirectoryExists(runResultsPath)) FileIO.CreateDirectory(runResultsPath);

            List<Airport> airportsList;
            if (!Parameters.AirportListFile.IsNullOrEmpty())
            {
                airportsList = FileIO.ReadAllText(Parameters.AirportListFile).DeserializeObject<List<Airport>>();
            }
            else
            {
                airportsList = AirportCollector.CollectAirports();
                FileIO.WriteAllText(
                    $"{runResultsPath}\\{runId}_airportList.json",
                    airportsList.SerializeObject(Formatting.Indented)
                );
            }

            IAirportFilterer filterer = new NoFilterer();
            if (Parameters.EuropeOnly) filterer = new EuropeFilterer();
            else if (Parameters.UKAndBulgariaOnly) filterer = new UKBulgariaFilterer();

            Dictionary<string, HashSet<string>> airportsAndDestinations;
            if (!Parameters.AirportDestinationsFile.IsNullOrEmpty())
            {
                airportsAndDestinations = FileIO.ReadAllText(Parameters.AirportDestinationsFile)
                    .DeserializeObject<Dictionary<string, HashSet<string>>>();
            }
            else
            {
                airportsAndDestinations = AirportPopulator.PopulateAirports(airportsList, filterer);
                FileIO.WriteAllText(
                    $"{runResultsPath}\\{runId}_airportDestinations.json",
                    airportsAndDestinations.SerializeObject(Formatting.Indented)
                );
            }

            AirportListFilterer airportListFilterer = new(airportsList);
            Dictionary<string, HashSet<string>> filteredAirportsAndDestinations =
                airportListFilterer.FilterAirports(airportsAndDestinations, filterer);

            Dictionary<string, JourneyRetrieverData> existingData = new();
            if (!Parameters.WorkerSetupFile.IsNullOrEmpty())
            {
                existingData = FileIO.ReadAllText(Parameters.WorkerSetupFile)
                    .DeserializeObject<Dictionary<string, JourneyRetrieverData>>();
            }

            JourneyRetrieverDataToLocalLinksConverter localLinksConverter = new();
            Dictionary<string, HashSet<string>> localLinks = localLinksConverter.DoConversion(existingData);

            Components.Log($"Generating paths...");
            AirportPathGenerator generator = new(filteredAirportsAndDestinations, localLinks);
            List<Path> paths = generator.GeneratePaths(
                Parameters.Origins,
                Parameters.Destinations,
                Parameters.MaxFlights,
                Parameters.MaxLocalLinks,
                Parameters.OnlyIncludeShortestPaths
            );
            List<List<string>> pathsDetailed = new();
            foreach (Path path in paths)
            {
                List<string> pathDetailed = new();
                for (int i = 0; i < path.Count(); i++)
                    pathDetailed.Add(airportsList.FirstOrDefault(x => x.Code.Equals(path[i])).ToString());
                pathsDetailed.Add(pathDetailed);
            }

            FileIO.WriteAllText(
                $"{runResultsPath}\\{runId}_latestPaths.json",
                pathsDetailed.SerializeObject(Formatting.Indented)
            );

            if (Parameters.OnlyPrintPaths)
            {
                SaveLogAndQuitDriver(runId, runResultsPath);
                return;
            }

            PathsToDirectPathGroupsConverter converter = new();

            Dictionary<string, JourneyRetrieverData> workersAndData =
                converter.GetGroups(paths, existingData, filteredAirportsAndDestinations);

            MultiJourneyCollectorResults existingResults = null;
            if (!Parameters.ProgressFile.IsNullOrEmpty())
            {
                existingResults = FileIO.ReadAllText(Parameters.ProgressFile)
                    .DeserializeObject<MultiJourneyCollectorResults>();
            }

            MultiJourneyCollectorResults journeyCollectorResults = await MultiJourneyCollector.GetJourneys(
                Components,
                workersAndData,
                Parameters.DateFrom,
                Parameters.DateTo,
                existingResults
            );
            FileIO.WriteAllText(
                $"{runResultsPath}\\{runId}_journeyCollectorResults.json",
                journeyCollectorResults.SerializeObject(Formatting.Indented)
            );
            PrintPathsAndJourneysAndFinish(airportsList, journeyCollectorResults, runId, runResultsPath, paths);
        }

        private void PrintPathsAndJourneysAndFinish(
            List<Airport> airportsList,
            MultiJourneyCollectorResults journeyCollectorResults,
            string runId,
            string runResultsPath,
            List<Path> paths
        )
        {
            SequentialJourneyCollectionBuilder builder = new();
            List<SequentialJourneyCollection> results = builder.GetFullPathCombinationOfJourneys(
                paths,
                journeyCollectorResults.JourneyCollection,
                Parameters.SkipUndoableJourneys,
                Parameters.SkipNotSameDayFinishJourneys,
                Parameters.NoLongerThan
            );

            DataTableCreator dtCreator = new();
            Printer.PrintTablesToWorksheet(
                dtCreator.GetTables(
                    airportsList,
                    results,
                    Parameters.SkipUndoableJourneys,
                    Parameters.SkipNotSameDayFinishJourneys,
                    Parameters.Home,
                    Parameters.TransportFromHomeCost,
                    Parameters.ExtraCostPerFlight,
                    Parameters.HotelCost,
                    Parameters.EarlyFlightHour
                ),
                $"{runResultsPath}\\{runId}_results.xlsx"
            );
            Components.Log($"Saved files to {runResultsPath}");
            SaveLogAndQuitDriver(runId, runResultsPath);
        }

        private void SaveLogAndQuitDriver(string runId, string runResultsPath)
        {
            FileIO.WriteAllText(
                $"{runResultsPath}\\{runId}_log.txt",
                Components.GetLoggerContent().SerializeObject(Formatting.Indented)
            );
        }
    }
}