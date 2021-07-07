using FlightConnectionsDotCom_ClassLibrary.Interfaces;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class SiteParser
    {
        private IWebDriver Driver { get; set; }
        private IJavaScriptExecutorWithDelayer JSExecutorWithDelayer { get; set; }
        private INavigationWorker NavigationWorker { get; set; }
        private IDelayer Delayer { get; set; }
        private IWebElementWorker WebElementWorker { get; set; }
        private ILogger Logger { get; set; }
        private const string gettingAirportsAndTheirConnections = "Getting airports and their connections";
        private const string buildingDictionaryWithCodesAndAirports = "Building dictionary with codes and airports";
        private const string collectingAirportDestinationsFromEachAirportPage = "Collecting airport destinations from each airport page";
        private const string collectingAirportDestinationsFromCurrentAirportPage = "Collecting airport destinations from current airport page";
        private const string collectingAirports = "Collecting airports";

        public SiteParser(IWebDriver driver, IJavaScriptExecutorWithDelayer jSExecutorWithDelayer, INavigationWorker navigationWorker, IDelayer delayer, IWebElementWorker webElementWorker, ILogger logger)
        {
            Driver = driver;
            JSExecutorWithDelayer = jSExecutorWithDelayer;
            NavigationWorker = navigationWorker;
            Delayer = delayer;
            WebElementWorker = webElementWorker;
            Logger = logger;
        }

        public async Task<Dictionary<string, HashSet<string>>> GetAirportsAndTheirConnections(List<Airport> airports, GetAirportsAndTheirConnectionsCommands commands)
        {
            Logger.Log($"{gettingAirportsAndTheirConnections} for {airports.Count} airports...");

            Logger.Log($"{buildingDictionaryWithCodesAndAirports}...");
            Dictionary<string, HashSet<string>> results = new();
            Dictionary<string, Airport> codesAndAirports = new();
            for (int i = 0; i < airports.Count; i++)
            {
                Airport airport = airports[i];
                codesAndAirports.Add(airport.Code, airport);
            }
            Logger.Log($"Finished {buildingDictionaryWithCodesAndAirports} for {airports.Count} airports.");

            Logger.Log($"{collectingAirportDestinationsFromEachAirportPage} for {airports.Count} airports...");
            for (int i = 0; i < airports.Count; i++)
            {
                Airport airport = airports[i];
                INavigation navigation = Driver.Navigate();
                await NavigationWorker.GoToUrl(navigation, (airport.Link));

                IWebElement showMoreButton = (IWebElement)await JSExecutorWithDelayer.ExecuteScriptAndWait(commands.GetShowMoreButton);
                if (showMoreButton != null)
                {
                    WebElementWorker.Click(showMoreButton);
                    await Delayer.Delay(1000);
                }

                HashSet<string> destinations = new();
                IWebElement popularDestinationsDiv = (IWebElement)await JSExecutorWithDelayer.ExecuteScriptAndWait(commands.GetPopularDestinationsDiv);
                if (popularDestinationsDiv == null)
                {
                    Logger.Log($"There was a problem with locating the popular destinations div for {airport.GetFullString()}");
                }
                else
                {
                    ReadOnlyCollection<IWebElement> popularDestinationsEntries = (ReadOnlyCollection<IWebElement>)await JSExecutorWithDelayer.ExecuteScriptAndWait(commands.GetPopularDestinationsEntries, popularDestinationsDiv);
                    for (int j = 0; j < popularDestinationsEntries.Count; j++)
                    {
                        IWebElement entry = popularDestinationsEntries[j];
                        string destination = (string)await JSExecutorWithDelayer.ExecuteScriptAndWait(commands.GetDestinationFromEntry, entry);
                        Match match = Regex.Match(destination, @"(.*?) \((...)\)$");
                        string name = match.Groups[1].Value;
                        string code = match.Groups[2].Value;
                        destinations.Add(code);
                    }
                }
                results.Add(airport.Code, destinations);
                Logger.Log($"Finished {collectingAirportDestinationsFromCurrentAirportPage} ({GetPercentageAndCountString(i, airports.Count)} airports done, {destinations.Count} destinations for airport {airport.GetFullString()}).");
            }
            Logger.Log($"Finished {collectingAirportDestinationsFromEachAirportPage} for {airports.Count} airports.");
            Logger.Log($"Finished {gettingAirportsAndTheirConnections} for {airports.Count} airports.");
            return results;
        }

        private static string GetPercentageAndCountString(int i, int maxCount)
        {
            int currentCount = i + 1;
            string percentageString = $"{((double)currentCount / (double)maxCount) * 100}%";
            Match match = Regex.Match(percentageString, @"(.*?\.\d\d).*%");
            if (match.Success) percentageString = $"{match.Groups[1].Value}%";
            string percentageAndCountString = $"{currentCount}/{maxCount} ({percentageString})";
            return percentageAndCountString;
        }

        public async Task<HashSet<Airport>> CollectAirports(CollectAirportCommands commands)
        {
            Logger.Log($"Navigating to airports page...");
            HashSet<Airport> airports = new();
            INavigation navigation = Driver.Navigate();
            await NavigationWorker.GoToUrl(navigation, ("https://www.flightconnections.com/airport-codes"));

            ReadOnlyCollection<IWebElement> airportListEntries = (ReadOnlyCollection<IWebElement>)await JSExecutorWithDelayer.ExecuteScriptAndWait(commands.GetAirportListEntries);
            Logger.Log($"{collectingAirports} {airportListEntries.Count} airports...");
            for (int i = 0; i < airportListEntries.Count; i++)
            {
                IWebElement airportListEntry = airportListEntries[i];
                string code = (string)await JSExecutorWithDelayer.ExecuteScriptAndWait(commands.GetAirportCodeFromEntry, airportListEntry);
                string airportCityAndCountry = (string)await JSExecutorWithDelayer.ExecuteScriptAndWait(commands.GetAirportCityAndCountryFromEntry, airportListEntry);
                Match match = Regex.Match(airportCityAndCountry, "(.*?), (.*)");
                string city = match.Groups[1].Value;
                string country = match.Groups[2].Value;
                string name = (string)await JSExecutorWithDelayer.ExecuteScriptAndWait(commands.GetAirportNameFromEntry, airportListEntry);
                string link = (string)await JSExecutorWithDelayer.ExecuteScriptAndWait(commands.GetAirportLinkFromEntry, airportListEntry);
                Airport airport = new(code, city, country, name, link);
                airports.Add(airport);
                Logger.Log($"Collected airport ({airport.GetFullString()} ({GetPercentageAndCountString(i, airportListEntries.Count)} airports done).");
            }
            Logger.Log($"Finished {collectingAirports} ({airportListEntries.Count} airports).");
            return airports;
        }
    }
}