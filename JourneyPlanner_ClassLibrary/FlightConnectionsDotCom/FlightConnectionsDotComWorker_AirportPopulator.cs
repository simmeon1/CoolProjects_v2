using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Interfaces;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.FlightConnectionsDotCom
{
    public class FlightConnectionsDotComWorkerAirportPopulator : IFlightConnectionsDotComWorkerAirportPopulator
    {
        private FlightConnectionsDotComWorker Worker { get; set; }

        public FlightConnectionsDotComWorkerAirportPopulator(FlightConnectionsDotComWorker worker)
        {
            Worker = worker;
        }

        public Dictionary<string, HashSet<string>> PopulateAirports(List<Airport> airportsList)
        {
            const string gettingAirportsAndTheirConnections = "Getting airports and their connections";
            const string collectingAirportDestinationsFromEachAirportPage = "Collecting airport destinations from each airport page";
            const string collectingAirportDestinationsFromCurrentAirportPage = "Collecting airport destinations from current airport page";
            Worker.Logger.Log($"{gettingAirportsAndTheirConnections} for {airportsList.Count} airports...");
            Worker.Logger.Log($"{collectingAirportDestinationsFromEachAirportPage} for {airportsList.Count} airports...");

            Dictionary<string, HashSet<string>> results = new();
            for (int i = 0; i < airportsList.Count; i++)
            {
                Airport airport = airportsList[i];
                NavigateToAirportPage(airport);
                if (Worker.Driver.FindElements(By.CssSelector("#captcha-container")).Count > 0)
                {
                    var x = 1;
                    // Fix captcha and continue
                }
                var showMoreButton = Worker.Driver.FindElements(By.CssSelector(".show-all-destinations-btn")).FirstOrDefault();
                if (showMoreButton != null) showMoreButton.Click();
                HashSet<string> destinations = GetDestinationsFromAirportPage(airport, results);
                Worker.Logger.Log($"Finished {collectingAirportDestinationsFromCurrentAirportPage} ({Globals.GetPercentageAndCountString(i, airportsList.Count)} airports done, {destinations.Count} destinations for airport {airport}).");
            }
            Worker.Logger.Log($"Finished {collectingAirportDestinationsFromEachAirportPage} for {airportsList.Count} airports.");
            Worker.Logger.Log($"Finished {gettingAirportsAndTheirConnections} for {airportsList.Count} airports.");
            return results;
        }

        private void NavigateToAirportPage(Airport airport)
        {
            INavigation navigation = Worker.Driver.Navigate();
            Worker.GoToUrl(navigation, airport.Link);
        }

        private HashSet<string> GetDestinationsFromAirportPage(Airport airport, Dictionary<string, HashSet<string>> results)
        {
            HashSet<string> destinations = new();
            var popularDestinationsDiv = Worker.Driver.FindElements(By.CssSelector("#popular-destinations")).FirstOrDefault();
            if (popularDestinationsDiv == null) Worker.Logger.Log($"There was a problem with locating the popular destinations div for {airport}");
            else AddDestinationsFromPopularDivToDestinationsList(popularDestinationsDiv, destinations);
            results.Add(airport.Code, destinations);
            return destinations;
        }
        
        private void AddDestinationsFromPopularDivToDestinationsList(IWebElement popularDestinationsDiv, HashSet<string> destinations)
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
    }
}