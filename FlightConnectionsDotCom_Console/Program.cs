using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Console
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string parametersPath = "";
            foreach (string arg in args)
            {
                Match match = Regex.Match(arg, "parametersPath-(.*)");
                if (match.Success) parametersPath = match.Groups[1].Value;
            }
            Parameters parameterss = new();
            Parameters parameters = System.IO.File.ReadAllText(parametersPath).DeserializeObject<Parameters>();

            Logger_Console logger = new();
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");

            ChromeDriver driver1 = null;
            bool useLocalAirportList = !parameters.LocalAirportListFile.IsNullOrEmpty();
            bool useLocalAirportDestinations = !parameters.LocalAirportDestinationsFile.IsNullOrEmpty();
            if (!useLocalAirportList || !useLocalAirportDestinations) driver1 = new(chromeOptions);

            FlightConnectionsDotComWorker worker = new(driver1, logger, new RealWebDriverWait(driver1));
            FlightConnectionsDotComWorker_AirportCollector collector = new(worker);
            List<Airport> airportsList = useLocalAirportList
                ? JsonConvert.DeserializeObject<List<Airport>>(System.IO.File.ReadAllText(parameters.LocalAirportListFile))
                : collector.CollectAirports();

            string runSummary = $"{parameters.Origins.ConcatenateListOfStringsToCommaString()} - {parameters.Destinations.ConcatenateListOfStringsToCommaString()}";
            runSummary += " - " + parameters.DateFrom.ToString("yyyy-MM-dd");
            runSummary += " - " + parameters.DateTo.ToString("yyyy-MM-dd");
            string runId = Globals.GetDateTimeFileNameFriendlyConcatenatedWithString(DateTime.Now, runSummary);
            string runResultsPath = System.IO.Path.Combine(parameters.FileSavePath, runId);
            if (!System.IO.Directory.Exists(runResultsPath)) System.IO.Directory.CreateDirectory(runResultsPath);
            if (!useLocalAirportList) System.IO.File.WriteAllText($"{runResultsPath}\\{runId}_airportList.json", JsonConvert.SerializeObject(airportsList, Formatting.Indented));

            IAirportFilterer filterer = new NoFilterer();
            if (parameters.EuropeOnly) filterer = new EuropeFilterer();
            else if (parameters.UKAndBulgariaOnly) filterer = new UKBulgariaFilterer();

            FlightConnectionsDotComWorker_AirportPopulator populator = new(worker);
            Dictionary<string, HashSet<string>> airportsAndDestinations = useLocalAirportDestinations
                ? JsonConvert.DeserializeObject<Dictionary<string, HashSet<string>>>(System.IO.File.ReadAllText(parameters.LocalAirportDestinationsFile))
                : populator.PopulateAirports(airportsList, filterer);
            if (!useLocalAirportDestinations) System.IO.File.WriteAllText($"{runResultsPath}\\{runId}_airportDestinations.json", JsonConvert.SerializeObject(airportsAndDestinations, Formatting.Indented));

            if (driver1 != null) driver1.Quit();

            AirportPathGenerator generator = new(airportsAndDestinations);
            List<Path> paths = generator.GeneratePaths(parameters.Origins, parameters.Destinations, parameters.MaxFlights, airportsList, filterer);
            List<List<string>> pathsDetailed = new();
            foreach (Path path in paths)
            {
                List<string> pathDetailed = new();
                for (int i = 0; i < path.Count(); i++) pathDetailed.Add(airportsList.FirstOrDefault(x => x.Code.Equals(path[i])).ToString());
                pathsDetailed.Add(pathDetailed);
            }
            System.IO.File.WriteAllText($"{runResultsPath}\\{runId}_latestPaths.json", JsonConvert.SerializeObject(pathsDetailed, Formatting.Indented));

            if (!parameters.OpenGoogleFlights) return;
            ChromeDriver driver2 = new();
            ChromeWorker chromeWorker = new(driver2, logger, new RealDelayer());
            List<FullPathAndListOfPathsAndFlightCollections> pathsAndFlights = await chromeWorker.ProcessPaths(paths, parameters.DateFrom, parameters.DateTo);
            System.IO.File.WriteAllText($"{runResultsPath}\\{runId}_pathsAndFlights.json", JsonConvert.SerializeObject(pathsAndFlights, Formatting.Indented));

            FullPathCombinationOfFlightsCollector flightCollector = new();
            List<SequentialFlightCollection> results2 = new();
            foreach (FullPathAndListOfPathsAndFlightCollections pathAndFlights in pathsAndFlights)
            {
                results2.AddRange(flightCollector.GetFullPathCombinationOfFLights(pathAndFlights));
            }

            DataTableCreator dtCreator = new();
            ExcelPrinter printer = new();
            printer.PrintTablesToWorksheet(dtCreator.GetTables(results2), $"{runResultsPath}\\{runId}_results.xlsx");
            logger.Log($"Saved files to {runResultsPath}");
        }
    }
}
