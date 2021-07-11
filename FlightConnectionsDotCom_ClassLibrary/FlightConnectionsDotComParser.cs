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
    public class FlightConnectionsDotComParser
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

        public FlightConnectionsDotComParser(IWebDriver driver, IJavaScriptExecutorWithDelayer jSExecutorWithDelayer, INavigationWorker navigationWorker, IDelayer delayer, IWebElementWorker webElementWorker, ILogger logger)
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

                IWebElement showMoreButton = await JSExecutorWithDelayer.RunScriptAndGetElement(commands.GetShowMoreButton);
                if (showMoreButton != null)
                {
                    showMoreButton.Click();
                    await Delayer.Delay(1000);
                }

                HashSet<string> destinations = new();
                IWebElement popularDestinationsDiv = await JSExecutorWithDelayer.RunScriptAndGetElement(commands.GetPopularDestinationsDiv);
                if (popularDestinationsDiv == null)
                {
                    Logger.Log($"There was a problem with locating the popular destinations div for {airport.GetFullString()}");
                }
                else
                {
                    ReadOnlyCollection<IWebElement> popularDestinationsEntries = await JSExecutorWithDelayer.RunScriptAndGetElements(commands.GetPopularDestinationsEntries, popularDestinationsDiv);
                    for (int j = 0; j < popularDestinationsEntries.Count; j++)
                    {
                        IWebElement entry = popularDestinationsEntries[j];
                        string destination = await JSExecutorWithDelayer.RunScriptAndGetString(commands.GetDestinationFromEntry, entry);
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

        public Dictionary<string, HashSet<string>> GetAirportsAndTheirConnections2(List<Airport> airports, GetAirportsAndTheirConnectionsCommands commands)
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
                navigation.GoToUrl(airport.Link);

                try
                {
                    Driver.FindElement(By.CssSelector("body"));
                }
                catch (UnhandledAlertException)
                {
                    //Do nothing
                }

                ReadOnlyCollection<IWebElement> buttons = Driver.FindElements(By.CssSelector("button"));
                foreach (IWebElement button in buttons)
                {
                    string buttonText = button.Text;
                    if (buttonText.Contains("AGREE"))
                    {
                        button.Click();
                        break;
                    }
                }

                IWebElement showMoreButton = Driver.FindElement(By.CssSelector(".show-all-destinations-btn"));
                if (showMoreButton != null) showMoreButton.Click();

                HashSet<string> destinations = new();
                IWebElement popularDestinationsDiv = Driver.FindElement(By.CssSelector("#popular-destinations"));
                if (popularDestinationsDiv == null)
                {
                    Logger.Log($"There was a problem with locating the popular destinations div for {airport.GetFullString()}");
                }
                else
                {
                    ReadOnlyCollection<IWebElement> popularDestinationsEntries = popularDestinationsDiv.FindElements(By.CssSelector(".popular-destination"));
                    for (int j = 0; j < popularDestinationsEntries.Count; j++)
                    {
                        IWebElement entry = popularDestinationsEntries[j];
                        string destination = entry.GetAttribute("data-a");
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

            ReadOnlyCollection<IWebElement> airportListEntries = await JSExecutorWithDelayer.RunScriptAndGetElements(commands.GetAirportListEntries);
            Logger.Log($"{collectingAirports} {airportListEntries.Count} airports...");
            for (int i = 0; i < airportListEntries.Count; i++)
            {
                IWebElement airportListEntry = airportListEntries[i];
                string code = await JSExecutorWithDelayer.RunScriptAndGetString(commands.GetAirportCodeFromEntry, airportListEntry);
                string airportCityAndCountry = await JSExecutorWithDelayer.RunScriptAndGetString(commands.GetAirportCityAndCountryFromEntry, airportListEntry);
                Match match = Regex.Match(airportCityAndCountry, "(.*?), (.*)");
                string city = match.Groups[1].Value;
                string country = match.Groups[2].Value;
                string name = await JSExecutorWithDelayer.RunScriptAndGetString(commands.GetAirportNameFromEntry, airportListEntry);
                string link = await JSExecutorWithDelayer.RunScriptAndGetString(commands.GetAirportLinkFromEntry, airportListEntry);
                Airport airport = new(code, city, country, name, link);
                airports.Add(airport);
                Logger.Log($"Collected airport ({airport.GetFullString()} ({GetPercentageAndCountString(i, airportListEntries.Count)} airports done).");
            }
            Logger.Log($"Finished {collectingAirports} ({airportListEntries.Count} airports).");
            return airports;
        }

        public HashSet<Airport> CollectAirports2(CollectAirportCommands commands)
        {
            Logger.Log($"Navigating to airports page...");
            HashSet<Airport> airports = new();
            INavigation navigation = Driver.Navigate();
            //await NavigationWorker.GoToUrl(navigation, ("https://www.flightconnections.com/airport-codes"));
            navigation.GoToUrl(("https://www.flightconnections.com/airport-codes"));

            try
            {
                Driver.FindElement(By.CssSelector("body"));
            }
            catch (UnhandledAlertException)
            {
                //Do nothing
            }

            ReadOnlyCollection<IWebElement> buttons = Driver.FindElements(By.CssSelector("button"));
            foreach (IWebElement button in buttons)
            {
                string buttonText = button.Text;
                if (buttonText.Contains("AGREE"))
                {
                    button.Click();
                    break;
                }
            }

            ReadOnlyCollection<IWebElement> airportListEntries = Driver.FindElements(By.TagName("li"));
            Logger.Log($"{collectingAirports} {airportListEntries.Count} airports...");
            for (int i = 0; i < airportListEntries.Count; i++)
            {
                IWebElement airportListEntry = airportListEntries[i];
                string code = airportListEntry.FindElement(By.CssSelector(".airport-code")).Text;
                string airportCityAndCountry = airportListEntry.FindElement(By.CssSelector(".airport-city-country")).Text;
                Match match = Regex.Match(airportCityAndCountry, "(.*?), (.*)");
                string city = match.Groups[1].Value;
                string country = match.Groups[2].Value;
                string name = airportListEntry.FindElement(By.CssSelector(".airport-name")).Text;
                string link = airportListEntry.FindElement(By.CssSelector("a")).GetAttribute("href");
                Airport airport = new(code, city, country, name, link);
                airports.Add(airport);
                Logger.Log($"Collected airport ({GetPercentageAndCountString(i, airportListEntries.Count)} airports done) {airport.GetFullString()}.");
            }
            Logger.Log($"Finished {collectingAirports} ({airportListEntries.Count} airports).");
            return airports;
        }
    }
}