using FlightConnectionsDotCom_ClassLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlightConnectionsDotCom_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            HashSet<string> arguments = new();
            foreach (string arg in args) arguments.Add(arg);

            string origin = "";
            string target = "";
            int maxFlights = 0;
            bool useLocalAirportList = false;
            bool useLocalAirportDestinations = false;
            bool openGoogleFlights = false;
            bool writeLocalFiles = false;
            DateTime date = DateTime.Today;

            foreach (string arg in arguments)
            {
                Match match = Regex.Match(arg, "origin-(.*)");
                if (match.Success) origin = match.Groups[1].Value;

                match = Regex.Match(arg, "target-(.*)");
                if (match.Success) target = match.Groups[1].Value;

                match = Regex.Match(arg, "maxFlights-(.*)");
                if (match.Success) maxFlights = int.Parse(match.Groups[1].Value);

                match = Regex.Match(arg, @"date-(\d\d-\d\d-\d\d\d\d)");
                if (match.Success) date = DateTime.ParseExact(match.Groups[1].Value, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                match = Regex.Match(arg, "useLocalAirportList");
                if (match.Success) useLocalAirportList = true;

                match = Regex.Match(arg, "useLocalAirportDestinations");
                if (match.Success) useLocalAirportDestinations = true;
                
                match = Regex.Match(arg, "openGoogleFlights");
                if (match.Success) openGoogleFlights = true;
                
                match = Regex.Match(arg, "writeLocalFiles");
                if (match.Success) writeLocalFiles = true;
            }

            const string airportListJsonFile = "airportList.json";
            const string airportDestinationJsonFile = "airportDestinations.json";

            Logger_Console logger = new();
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");

            ChromeDriver driver1 = null;
            if (!useLocalAirportList || !useLocalAirportDestinations) driver1 = new(chromeOptions);

            FlightConnectionsDotComParser siteParser = new(driver1, logger);
            List<Airport> airportsList = useLocalAirportList
                ? JsonConvert.DeserializeObject<List<Airport>>(File.ReadAllText(airportListJsonFile))
                : siteParser.CollectAirports();

            Dictionary<string, HashSet<string>> airportsAndDestinations = useLocalAirportDestinations
                ? JsonConvert.DeserializeObject<Dictionary<string, HashSet<string>>>(File.ReadAllText(airportDestinationJsonFile))
                : siteParser.GetAirportsAndTheirConnections(airportsList);
            if (driver1 != null) driver1.Quit();

            if (writeLocalFiles)
            {
                File.WriteAllText(airportListJsonFile, JsonConvert.SerializeObject(airportsList, Formatting.Indented));
                File.WriteAllText(airportDestinationJsonFile, JsonConvert.SerializeObject(airportsAndDestinations, Formatting.Indented));
            }

            AirportPathGenerator generator = new(airportsAndDestinations);
            List<List<string>> paths = generator.GeneratePaths(origin, target, maxFlights);
            List<List<string>> pathsDetailed = new();
            foreach (List<string> path in paths)
            {
                List<string> pathDetailed = new();
                foreach (string airport in path) pathDetailed.Add(airportsList.FirstOrDefault(x => x.Code.Equals(airport)).GetFullString());
                pathsDetailed.Add(pathDetailed);
            }
            File.WriteAllText($"latestPaths.json", JsonConvert.SerializeObject(pathsDetailed, Formatting.Indented));

            if (!openGoogleFlights) return;
            ChromeDriver driver2 = new();
            ChromeWorker chromeWorker = new(driver2, driver2, logger);
            chromeWorker.OpenPaths(paths, date);
        }

        private static string GetDateTimeNowString()
        {
            DateTime dateTimeNow = DateTime.Now;
            string dateTimeNowStr = dateTimeNow.ToString("s", CultureInfo.CreateSpecificCulture("en-US"));
            dateTimeNowStr = dateTimeNowStr.Replace(':', '-');
            return dateTimeNowStr;
        }
    }
}
