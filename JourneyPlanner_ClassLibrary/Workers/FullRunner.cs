using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.AirportFilterers;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Interfaces;
using JourneyPlanner_ClassLibrary.JourneyRetrievers;
using Newtonsoft.Json;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class FullRunner
    {
        private Parameters Parameters { get; set; }
        private JourneyRetrieverComponents Components { get; set; }
        private IFileIO FileIO { get; set; }
        private IExcelPrinter Printer { get; set; }
        private IFlightConnectionsDotComWorkerAirportCollector AirportCollector { get; set; }
        private IFlightConnectionsDotComWorkerAirportPopulator AirportPopulator { get; set; }
        private IDateTimeProvider DateTimeProvider { get; set; }

        public FullRunner(
            JourneyRetrieverComponents components,
            IFileIO fileIO,
            IDateTimeProvider dateTimeProvider,
            IExcelPrinter printer,
            IFlightConnectionsDotComWorkerAirportCollector airportCollector,
            IFlightConnectionsDotComWorkerAirportPopulator airportPopulator
        )
        {
            FileIO = fileIO;
            Printer = printer;
            AirportCollector = airportCollector;
            AirportPopulator = airportPopulator;
            DateTimeProvider = dateTimeProvider;
            Components = components;
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

            Components.Log($"Generating paths...");
            AirportPathGenerator generator = new(filteredAirportsAndDestinations);
            List<Path> paths = generator.GeneratePaths(
                Parameters.Origins,
                Parameters.Destinations,
                Parameters.MaxFlights
            );
            List<List<string>> pathsDetailed = new();
            foreach (Path path in paths)
            {
                List<string> pathDetailed = new();
                for (int i = 0; i < path.Count(); i++)
                {
                    pathDetailed.Add(airportsList.FirstOrDefault(x => x.Code.Equals(path[i])).ToString());
                }
                pathsDetailed.Add(pathDetailed);
            }

            FileIO.WriteAllText(
                $"{runResultsPath}\\{runId}_latestPaths.json",
                pathsDetailed.SerializeObject(Formatting.Indented)
            );
            
            JourneyRetrieverData journeyRetrieverData = new(
                paths.SelectMany(x => x.GetDirectPaths())
                    .GroupBy(x => x.ToString())
                    .Select(g => g.First())
                    .OrderBy(x => x.ToString())
                    .ToList()
            );

            var journeyCollectorResults = await new MultiJourneyCollector().GetJourneys(
                Components,
                journeyRetrieverData,
                Parameters.DateFrom,
                Parameters.DateTo
            );
            
            FileIO.WriteAllText(
                $"{runResultsPath}\\{runId}_journeyCollectorResults.json",
                journeyCollectorResults.SerializeObject(Formatting.Indented)
            );
            PrintPathsAndJourneysAndFinish(airportsList, journeyCollectorResults, runId, runResultsPath, paths);
        }

        private void PrintPathsAndJourneysAndFinish(
            List<Airport> airportsList,
            JourneyCollection journeyCollectorResults,
            string runId,
            string runResultsPath,
            List<Path> paths
        )
        {
            SequentialJourneyCollectionBuilder builder = new();
            List<SequentialJourneyCollection> results = builder.GetFullPathCombinationOfJourneys(
                paths,
                journeyCollectorResults,
                Parameters.NoLongerThan
            );

            DataTableCreator dtCreator = new();
            Printer.PrintTablesToWorksheet(
                dtCreator.GetTables(airportsList, results),
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