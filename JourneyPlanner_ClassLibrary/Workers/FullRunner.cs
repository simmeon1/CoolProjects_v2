using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.AirportFilterers;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Interfaces;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class FullRunner
    {
        private readonly IWebDriver driver;
        private readonly ILogger logger;
        private readonly IWebDriverWaitProvider wait;
        private readonly IDelayer delayer;
        private readonly IHttpClient http;
        private readonly IJavaScriptExecutor jsExecutor;
        private readonly IFileIO fileIo;
        private readonly IExcelPrinter printer;
        private readonly IFlightConnectionsDotComWorkerAirportCollector airportCollector;
        private readonly IFlightConnectionsDotComWorkerAirportPopulator airportPopulator;
        private readonly IDateTimeProvider dateTimeProvider;

        public FullRunner(
            IWebDriver driver,
            ILogger logger,
            IWebDriverWaitProvider wait,
            IDelayer delayer,
            IHttpClient http,
            IJavaScriptExecutor jsExecutor,
            IFileIO fileIo,
            IDateTimeProvider dateTimeProvider,
            IExcelPrinter printer,
            IFlightConnectionsDotComWorkerAirportCollector airportCollector,
            IFlightConnectionsDotComWorkerAirportPopulator airportPopulator
        )
        {
            this.driver = driver;
            this.logger = logger;
            this.wait = wait;
            this.delayer = delayer;
            this.http = http;
            this.jsExecutor = jsExecutor;
            this.fileIo = fileIo;
            this.printer = printer;
            this.airportCollector = airportCollector;
            this.airportPopulator = airportPopulator;
            this.dateTimeProvider = dateTimeProvider;
        }

        public async Task DoRun(Parameters p)
        {
            var components = new JourneyRetrieverComponents(
                driver,
                logger,
                wait,
                delayer,
                http,
                jsExecutor
            );
            
            string runSummary =
                $"{p.Origins.ConcatenateListOfStringsToCommaAndSpaceString()} - {p.Destinations.ConcatenateListOfStringsToCommaAndSpaceString()}";
            runSummary += " - " + p.DateFrom.ToString("yyyy-MM-dd");
            runSummary += " - " + p.DateTo.ToString("yyyy-MM-dd");
            string runId =
                Globals.GetDateTimeFileNameFriendlyConcatenatedWithString(dateTimeProvider.Now(), runSummary);
            string runResultsPath = System.IO.Path.Combine(p.FileSavePath, runId);
            if (!fileIo.DirectoryExists(runResultsPath))
            {
                fileIo.CreateDirectory(runResultsPath);
            }

            List<Airport> airportsList;
            if (!p.AirportListFile.IsNullOrEmpty())
            {
                airportsList = fileIo.ReadAllText(p.AirportListFile).DeserializeObject<List<Airport>>();
            }
            else
            {
                airportsList = airportCollector.CollectAirports();
                fileIo.WriteAllText(
                    $"{runResultsPath}\\{runId}_airportList.json",
                    airportsList.SerializeObject(Formatting.Indented)
                );
            }

            IAirportFilterer filterer = new NoFilterer();

            Dictionary<string, HashSet<string>> airportsAndDestinations;
            if (!p.AirportDestinationsFile.IsNullOrEmpty())
            {
                airportsAndDestinations = fileIo.ReadAllText(p.AirportDestinationsFile)
                    .DeserializeObject<Dictionary<string, HashSet<string>>>();
            }
            else
            {
                airportsAndDestinations = airportPopulator.PopulateAirports(airportsList, filterer);
                fileIo.WriteAllText(
                    $"{runResultsPath}\\{runId}_airportDestinations.json",
                    airportsAndDestinations.SerializeObject(Formatting.Indented)
                );
            }

            AirportListFilterer airportListFilterer = new(airportsList);
            Dictionary<string, HashSet<string>> filteredAirportsAndDestinations =
                airportListFilterer.FilterAirports(airportsAndDestinations, filterer);

            components.Log($"Generating paths...");


            AirportPathGenerator generator = new(logger, filteredAirportsAndDestinations);
            JourneyCollectorResults results = p.ExistingResultsPath.IsNullOrEmpty()
                ? null
                : fileIo.ReadAllText(p.ExistingResultsPath)
                    .DeserializeObject<JourneyCollectorResults>();

            var origins = results?.Origins ?? p.Origins;
            var destinations = results?.Destinations ?? p.Destinations;
            var maxFlights = results?.MaxFlights ?? p.MaxFlights;
            var existingJourneyCollection = results?.JourneyCollection;

            List<Path> paths = generator.GeneratePaths(
                origins,
                destinations,
                maxFlights
            );

            var bannedAirports = fileIo.ReadAllText(p.BannedAirportsFile)
                .DeserializeObject<List<string>>();

            //Airport not in google flights or banned
            paths = paths.Where(p => !bannedAirports.Any(x => p.ToString().Contains(x))).ToList();

            SavePaths(paths, airportsList, runResultsPath, runId);

            var directPaths =
                paths.SelectMany(x => x.GetDirectPaths())
                    .GroupBy(x => x.ToString())
                    .Select(g => g.First())
                    .OrderBy(x => x.ToString())
                    .ToList();

            var journeyCollection = existingJourneyCollection ?? await new MultiJourneyCollector().GetJourneys(
                components,
                directPaths,
                p.DateFrom,
                p.DateTo
            );

            var journeyCollectorResults = new JourneyCollectorResults
            {
                Origins = origins,
                Destinations = destinations,
                MaxFlights = maxFlights,
                JourneyCollection = journeyCollection
            };

            fileIo.WriteAllText(
                $"{runResultsPath}\\{runId}_journeyCollectorResults.json",
                journeyCollectorResults.SerializeObject(Formatting.Indented)
            );

            var timePenalties = 
                !p.TimePenaltiesFile.IsNullOrEmpty() 
                    ? fileIo.ReadAllText(p.TimePenaltiesFile).DeserializeObject<Dictionary<string,int>>() 
                    : new Dictionary<string, int>();
            
            var costPenalties = 
                !p.CostPenaltiesFile.IsNullOrEmpty() 
                    ? fileIo.ReadAllText(p.CostPenaltiesFile).DeserializeObject<Dictionary<string,int>>() 
                    : new Dictionary<string, int>();
            
            PrintPathsAndJourneysAndFinish(
                airportsList,
                journeyCollection,
                runId,
                runResultsPath,
                paths,
                p.NoLongerThan,
                timePenalties,
                costPenalties,
                components
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

            fileIo.WriteAllText(
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
            int noLongerThan,
            Dictionary<string, int> timePenalties,
            Dictionary<string, int> costPenalties,
            JourneyRetrieverComponents components
        )
        {
            SequentialJourneyCollectionBuilder builder = new();
            components.Log($"Building full journeys...");
            List<SequentialJourneyCollection> results = builder.GetFullPathCombinationOfJourneys(
                paths,
                journeyCollectorResults,
                noLongerThan
            );

            TableEntryCreator tableEntryCreator = new();
            var tableEntries = tableEntryCreator.GetTableEntries(airportsList, results, timePenalties, costPenalties);
            var tables = new DataTableCreator().GetTables(tableEntries);
            printer.PrintTablesToWorksheet(
                tables,
                $"{runResultsPath}\\{runId}_results.xlsx"
            );

            components.Log($"Saved files to {runResultsPath}");
            fileIo.WriteAllText(
                $"{runResultsPath}\\{runId}_log.txt",
                components.GetLoggerContent().SerializeObject(Formatting.Indented)
            );
        }
    }
}