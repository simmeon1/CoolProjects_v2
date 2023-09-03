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

        public async Task DoRun(Parameters paramss)
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
                $"{paramss.Origins.ConcatenateListOfStringsToCommaAndSpaceString()} - {paramss.Destinations.ConcatenateListOfStringsToCommaAndSpaceString()}";
            runSummary += " - " + paramss.DateFrom.ToString("yyyy-MM-dd");
            runSummary += " - " + paramss.DateTo.ToString("yyyy-MM-dd");
            string runId =
                Globals.GetDateTimeFileNameFriendlyConcatenatedWithString(dateTimeProvider.Now(), runSummary);
            string runResultsPath = System.IO.Path.Combine(paramss.FileSavePath, runId);
            if (!fileIo.DirectoryExists(runResultsPath))
            {
                fileIo.CreateDirectory(runResultsPath);
            }

            List<Airport> airportsList;
            if (!paramss.AirportListFile.IsNullOrEmpty())
            {
                airportsList = fileIo.ReadAllText(paramss.AirportListFile).DeserializeObject<List<Airport>>();
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
            if (!paramss.AirportDestinationsFile.IsNullOrEmpty())
            {
                airportsAndDestinations = fileIo.ReadAllText(paramss.AirportDestinationsFile)
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
            JourneyCollectorResults results = paramss.ExistingResultsPath.IsNullOrEmpty()
                ? null
                : fileIo.ReadAllText(paramss.ExistingResultsPath)
                    .DeserializeObject<JourneyCollectorResults>();

            var origins = results?.Origins ?? paramss.Origins;
            var destinations = results?.Destinations ?? paramss.Destinations;
            var maxFlights = results?.MaxFlights ?? paramss.MaxFlights;
            var existingJourneyCollection = results?.JourneyCollection;

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
                components,
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

            fileIo.WriteAllText(
                $"{runResultsPath}\\{runId}_journeyCollectorResults.json",
                journeyCollectorResults.SerializeObject(Formatting.Indented)
            );

            var penalties = 
                !paramss.PenaltiesFile.IsNullOrEmpty() 
                    ? fileIo.ReadAllText(paramss.PenaltiesFile).DeserializeObject<Dictionary<string,int>>() 
                    : new Dictionary<string, int>();
            
            PrintPathsAndJourneysAndFinish(
                airportsList,
                journeyCollection,
                runId,
                runResultsPath,
                paths,
                paramss.NoLongerThan,
                penalties,
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
            Dictionary<string, int> penalties,
            JourneyRetrieverComponents components
        )
        {
            SequentialJourneyCollectionBuilder builder = new();
            List<SequentialJourneyCollection> results = builder.GetFullPathCombinationOfJourneys(
                paths,
                journeyCollectorResults,
                noLongerThan
            );

            DataTableCreator dtCreator = new();
            printer.PrintTablesToWorksheet(
                dtCreator.GetTables(airportsList, results, penalties),
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