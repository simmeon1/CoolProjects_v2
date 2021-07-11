using FlightConnectionsDotCom_ClassLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
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
                if (match.Success) date = DateTime.ParseExact(match.Groups[1].Value, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                match = Regex.Match(arg, "useLocalAirportList");
                if (match.Success) useLocalAirportList = true;

                match = Regex.Match(arg, "useLocalAirportDestinations");
                if (match.Success) useLocalAirportDestinations = true;
            }

            Logger_Console logger = new();
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");

            ChromeDriver driver1 = null;
            if (!useLocalAirportList || !useLocalAirportDestinations) driver1 = new(chromeOptions);

            FlightConnectionsDotComParser siteParser = new(driver1, logger);
            List<Airport> airportsList = useLocalAirportList
                ? JsonConvert.DeserializeObject<List<Airport>>(File.ReadAllText("airportList.json"))
                : siteParser.CollectAirports();

            Dictionary<string, HashSet<string>> airportsAndDestinations = useLocalAirportDestinations
                ? JsonConvert.DeserializeObject<Dictionary<string, HashSet<string>>>(File.ReadAllText("airportDestinations.json"))
                : siteParser.GetAirportsAndTheirConnections(airportsList);
            if (driver1 != null) driver1.Quit();

            AirportPathGenerator generator = new(airportsAndDestinations);
            List<List<string>> paths = generator.GeneratePaths(origin, target, maxFlights);

            ChromeDriver driver2 = new();
            ChromeWorker chromeWorker = new(driver2, driver2, logger);
            chromeWorker.OpenPaths(paths, date);
        }
    }
}
