﻿using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            Parameters parameters = File.ReadAllText(parametersPath).DeserializeObject<Parameters>();

            Logger_Console logger = new();
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");

            ChromeDriver driver1 = null;
            bool useLocalAirportList = !parameters.LocalAirportListFile.IsNullOrEmpty();
            bool useLocalAirportDestinations = !parameters.LocalAirportDestinationsFile.IsNullOrEmpty();
            if (!useLocalAirportList || !useLocalAirportDestinations) driver1 = new(chromeOptions);

            FlightConnectionsDotComParser siteParser = new(driver1, logger);
            List<Airport> airportsList = useLocalAirportList
                ? JsonConvert.DeserializeObject<List<Airport>>(File.ReadAllText(parameters.LocalAirportListFile))
                : siteParser.CollectAirports(europeOnly: parameters.EuropeOnly);

            string runId = Globals.GetDateConcatenatedWithGuid(DateTime.Now, Guid.NewGuid().ToString());
            if (!useLocalAirportList) File.WriteAllText($"{parameters.FileSavePath}\\airportList_{runId}.json", JsonConvert.SerializeObject(airportsList, Formatting.Indented));

            Dictionary<string, HashSet<string>> airportsAndDestinations = useLocalAirportDestinations
                ? JsonConvert.DeserializeObject<Dictionary<string, HashSet<string>>>(File.ReadAllText(parameters.LocalAirportDestinationsFile))
                : siteParser.GetAirportsAndTheirConnections(airportsList);
            if (!useLocalAirportDestinations) File.WriteAllText($"{parameters.FileSavePath}\\airportDestinations_{runId}.json", JsonConvert.SerializeObject(airportsAndDestinations, Formatting.Indented));

            if (driver1 != null) driver1.Quit();

            AirportPathGenerator generator = new(airportsAndDestinations);
            List<List<string>> paths = generator.GeneratePaths(parameters.Origin, parameters.Destination, parameters.MaxFlights);
            List<List<string>> pathsDetailed = new();
            foreach (List<string> path in paths)
            {
                List<string> pathDetailed = new();
                foreach (string airport in path) pathDetailed.Add(airportsList.FirstOrDefault(x => x.Code.Equals(airport)).GetFullString());
                pathsDetailed.Add(pathDetailed);
            }
            File.WriteAllText($"{parameters.FileSavePath}\\latestPaths_{runId}.json", JsonConvert.SerializeObject(pathsDetailed, Formatting.Indented));

            if (!parameters.OpenGoogleFlights) return;
            ChromeDriver driver2 = new();
            ChromeWorker chromeWorker = new(driver2, driver2, logger, new RealDelayer());
            await chromeWorker.OpenPaths(paths, parameters.Date);
        }
    }
}
