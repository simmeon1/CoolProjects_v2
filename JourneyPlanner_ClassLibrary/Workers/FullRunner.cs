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
        //private Parameters Parameters { get; set; }
        private JourneyRetrieverComponents Components { get; set; }
        private IFileIO FileIo { get; set; }
        private IExcelPrinter Printer { get; set; }
        private IFlightConnectionsDotComWorkerAirportCollector AirportCollector { get; set; }
        private IFlightConnectionsDotComWorkerAirportPopulator AirportPopulator { get; set; }
        private IDateTimeProvider DateTimeProvider { get; set; }

        public FullRunner(
            JourneyRetrieverComponents components,
            IFileIO fileIo,
            IDateTimeProvider dateTimeProvider,
            IExcelPrinter printer,
            IFlightConnectionsDotComWorkerAirportCollector airportCollector,
            IFlightConnectionsDotComWorkerAirportPopulator airportPopulator
        )
        {
            FileIo = fileIo;
            Printer = printer;
            AirportCollector = airportCollector;
            AirportPopulator = airportPopulator;
            DateTimeProvider = dateTimeProvider;
            Components = components;
        }

        public async Task DoRun(Parameters paramss)
        {
            string runSummary =
                $"{paramss.Origins.ConcatenateListOfStringsToCommaAndSpaceString()} - {paramss.Destinations.ConcatenateListOfStringsToCommaAndSpaceString()}";
            runSummary += " - " + paramss.DateFrom.ToString("yyyy-MM-dd");
            runSummary += " - " + paramss.DateTo.ToString("yyyy-MM-dd");
            string runId =
                Globals.GetDateTimeFileNameFriendlyConcatenatedWithString(DateTimeProvider.Now(), runSummary);
            string runResultsPath = System.IO.Path.Combine(paramss.FileSavePath, runId);
            if (!FileIo.DirectoryExists(runResultsPath))
            {
                FileIo.CreateDirectory(runResultsPath);
            }

            List<Airport> airportsList;
            if (!paramss.AirportListFile.IsNullOrEmpty())
            {
                airportsList = FileIo.ReadAllText(paramss.AirportListFile).DeserializeObject<List<Airport>>();
            }
            else
            {
                airportsList = AirportCollector.CollectAirports();
                FileIo.WriteAllText(
                    $"{runResultsPath}\\{runId}_airportList.json",
                    airportsList.SerializeObject(Formatting.Indented)
                );
            }

            IAirportFilterer filterer = new NoFilterer();

            Dictionary<string, HashSet<string>> airportsAndDestinations;
            if (!paramss.AirportDestinationsFile.IsNullOrEmpty())
            {
                airportsAndDestinations = FileIo.ReadAllText(paramss.AirportDestinationsFile)
                    .DeserializeObject<Dictionary<string, HashSet<string>>>();
            }
            else
            {
                airportsAndDestinations = AirportPopulator.PopulateAirports(airportsList, filterer);
                FileIo.WriteAllText(
                    $"{runResultsPath}\\{runId}_airportDestinations.json",
                    airportsAndDestinations.SerializeObject(Formatting.Indented)
                );
            }

            AirportListFilterer airportListFilterer = new(airportsList);
            Dictionary<string, HashSet<string>> filteredAirportsAndDestinations =
                airportListFilterer.FilterAirports(airportsAndDestinations, filterer);

            Components.Log($"Generating paths...");


            AirportPathGenerator generator = new(filteredAirportsAndDestinations);
            JourneyCollectorResults results = paramss.ExistingResultsPath.IsNullOrEmpty()
                ? null
                : FileIo.ReadAllText(paramss.ExistingResultsPath)
                    .DeserializeObject<JourneyCollectorResults>();

            var origins = results?.Origins ?? paramss.Origins;
            var destinations = results?.Destinations ?? paramss.Destinations;
            var maxFlights = results?.MaxFlights ?? paramss.MaxFlights;
            var existingJourneyCollection = results?.JourneyCollection;

            await DoTheRest(
                paramss,
                generator,
                origins,
                destinations,
                maxFlights,
                airportsList,
                runResultsPath,
                runId,
                existingJourneyCollection
            );
        }

        private async Task DoTheRest(
            Parameters paramss,
            AirportPathGenerator generator,
            List<string> origins,
            List<string> destinations,
            int maxFlights,
            List<Airport> airportsList,
            string runResultsPath,
            string runId,
            JourneyCollection existingJourneyCollection
        )
        {
            List<Path> paths = generator.GeneratePaths(
                origins,
                destinations,
                maxFlights
            );

            SavePaths(paths, airportsList, runResultsPath, runId);

            var directPaths =
                paths.SelectMany(x => x.GetDirectPaths())
                    .GroupBy(x => x.ToString())
                    .Select(g => g.First())
                    .OrderBy(x => x.ToString())
                    .ToList();

            var journeyCollection = existingJourneyCollection ?? await new MultiJourneyCollector().GetJourneys(
                Components,
                directPaths,
                paramss.DateFrom,
                paramss.DateTo
            );

            var journeyCollectorResults = new JourneyCollectorResults
            {
                Origins = origins,
                Destinations = destinations,
                MaxFlights = maxFlights,
                JourneyCollection = journeyCollection
            };

            FileIo.WriteAllText(
                $"{runResultsPath}\\{runId}_journeyCollectorResults.json",
                journeyCollectorResults.SerializeObject(Formatting.Indented)
            );

            PrintPathsAndJourneysAndFinish(
                airportsList,
                journeyCollection,
                runId,
                runResultsPath,
                paths,
                paramss.NoLongerThan
            );
        }

        private void SavePaths(List<Path> paths, List<Airport> airportsList, string runResultsPath, string runId)
        {
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

            FileIo.WriteAllText(
                $"{runResultsPath}\\{runId}_latestPaths.json",
                pathsDetailed.SerializeObject(Formatting.Indented)
            );
        }

        private void PrintPathsAndJourneysAndFinish(
            List<Airport> airportsList,
            JourneyCollection journeyCollectorResults,
            string runId,
            string runResultsPath,
            List<Path> paths,
            int noLongerThan
        )
        {
            SequentialJourneyCollectionBuilder builder = new();
            List<SequentialJourneyCollection> results = builder.GetFullPathCombinationOfJourneys(
                paths,
                journeyCollectorResults,
                noLongerThan
            );

            DataTableCreator dtCreator = new();
            Printer.PrintTablesToWorksheet(
                dtCreator.GetTables(airportsList, results),
                $"{runResultsPath}\\{runId}_results.xlsx"
            );

            Components.Log($"Saved files to {runResultsPath}");
            FileIo.WriteAllText(
                $"{runResultsPath}\\{runId}_log.txt",
                Components.GetLoggerContent().SerializeObject(Formatting.Indented)
            );
        }
    }
}