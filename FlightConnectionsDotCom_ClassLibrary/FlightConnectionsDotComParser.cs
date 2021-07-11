using FlightConnectionsDotCom_ClassLibrary.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class FlightConnectionsDotComParser
    {
        private IWebDriver Driver { get; set; }
        private ILogger Logger { get; set; }
        private const string gettingAirportsAndTheirConnections = "Getting airports and their connections";
        private const string buildingDictionaryWithCodesAndAirports = "Building dictionary with codes and airports";
        private const string collectingAirportDestinationsFromEachAirportPage = "Collecting airport destinations from each airport page";
        private const string collectingAirportDestinationsFromCurrentAirportPage = "Collecting airport destinations from current airport page";
        private const string collectingAirports = "Collecting airports";

        public FlightConnectionsDotComParser(IWebDriver driver, ILogger logger)
        {
            Driver = driver;
            Logger = logger;
        }

        public Dictionary<string, HashSet<string>> GetAirportsAndTheirConnections(List<Airport> airports)
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
                GoToUrl(navigation, airport.Link);

                try
                {
                    IWebElement showMoreButton = Driver.FindElement(By.CssSelector(".show-all-destinations-btn"));
                    if (showMoreButton != null) showMoreButton.Click();
                }
                catch (NoSuchElementException)
                {
                    //No button to click
                }

                HashSet<string> destinations = new();

                IWebElement popularDestinationsDiv;
                try
                {
                    popularDestinationsDiv = Driver.FindElement(By.CssSelector("#popular-destinations"));
                }
                catch (NoSuchElementException)
                {
                    popularDestinationsDiv = null;
                }

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

        private void GoToUrl(INavigation navigation, string link)
        {
            if (navigation == null) return;
            navigation.GoToUrl(link);

            try
            {
                WebDriverWait wait = new(Driver, TimeSpan.FromMilliseconds(100));
                IAlert result = wait.Until(ExpectedConditions.AlertIsPresent());
                result.Accept();
            }
            catch (WebDriverTimeoutException)
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

        public HashSet<Airport> CollectAirports(int maxCountToCollect = 0)
        {
            Logger.Log($"Navigating to airports page...");
            HashSet<Airport> airports = new();
            INavigation navigation = Driver.Navigate();
            GoToUrl(navigation, "https://www.flightconnections.com/airport-codes");

            ReadOnlyCollection<IWebElement> airportListEntries = Driver.FindElements(By.TagName("li"));

            int countToCollect = airportListEntries.Count;
            if (maxCountToCollect > 0) countToCollect = maxCountToCollect > airportListEntries.Count ? airportListEntries.Count : maxCountToCollect;

            Logger.Log($"{collectingAirports} {countToCollect} airports...");
            for (int i = 0; i < countToCollect; i++)
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
                Logger.Log($"Collected airport ({GetPercentageAndCountString(i, countToCollect)} airports done) {airport.GetFullString()}.");
            }
            Logger.Log($"Finished {collectingAirports} ({countToCollect} airports).");
            return airports;
        }
    }
}