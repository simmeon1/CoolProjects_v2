using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.AirportFilterers;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Interfaces;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.FlightConnectionsDotCom
{
    public class FlightConnectionsDotComWorkerAirportPopulator : IFlightConnectionsDotComWorkerAirportPopulator
    {
        private const string gettingAirportsAndTheirConnections = "Getting airports and their connections";
        private const string collectingAirportDestinationsFromEachAirportPage = "Collecting airport destinations from each airport page";
        private const string collectingAirportDestinationsFromCurrentAirportPage = "Collecting airport destinations from current airport page";
        private FlightConnectionsDotComWorker Worker { get; set; }
        private List<Airport> AirportsList { get; set; }
        private IAirportFilterer Filterer { get; set; }

        public FlightConnectionsDotComWorkerAirportPopulator(FlightConnectionsDotComWorker worker)
        {
            Worker = worker;
        }

        public Dictionary<string, HashSet<string>> PopulateAirports(List<Airport> airportsList, IAirportFilterer filterer = null)
        {
            AirportsList = airportsList;
            Filterer = filterer ?? new NoFilterer();

            Worker.Logger.Log($"{gettingAirportsAndTheirConnections} for {AirportsList.Count} airports...");
            Worker.Logger.Log($"{collectingAirportDestinationsFromEachAirportPage} for {AirportsList.Count} airports...");

            Dictionary<string, HashSet<string>> results = new();
            for (int i = 0; i < AirportsList.Count; i++)
            {
                Airport airport = AirportsList[i];
                if (!Filterer.AirportMeetsCondition(airport)) continue;
                NavigateToAirportPage(airport);
                ClickShowMoreButtonIfItExists();
                HashSet<string> destinations = GetDestinationsFromAirportPage(airport, results);
                Worker.Logger.Log($"Finished {collectingAirportDestinationsFromCurrentAirportPage} ({Globals.GetPercentageAndCountString(i, AirportsList.Count)} airports done, {destinations.Count} destinations for airport {airport}).");
            }
            Worker.Logger.Log($"Finished {collectingAirportDestinationsFromEachAirportPage} for {AirportsList.Count} airports.");
            Worker.Logger.Log($"Finished {gettingAirportsAndTheirConnections} for {AirportsList.Count} airports.");
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
            IWebElement popularDestinationsDiv = GetPopularDestinationsDiv();
            if (popularDestinationsDiv == null) Worker.Logger.Log($"There was a problem with locating the popular destinations div for {airport}");
            else AddDestinationsFromPopularDivToDestinationsList(popularDestinationsDiv, destinations);
            results.Add(airport.Code, destinations);
            return destinations;
        }

        private IWebElement GetPopularDestinationsDiv()
        {
            IWebElement popularDestinationsDiv;
            try
            {
                popularDestinationsDiv = Worker.Driver.FindElement(By.CssSelector("#popular-destinations"));
            }
            catch (NoSuchElementException)
            {
                popularDestinationsDiv = null;
            }

            return popularDestinationsDiv;
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
                Airport airport = AirportsList.FirstOrDefault(a => a.Code.Equals(code));
                if (Filterer.AirportMeetsCondition(airport)) destinations.Add(code);
            }
        }

        private void ClickShowMoreButtonIfItExists()
        {
            try
            {
                IWebElement showMoreButton = Worker.Driver.FindElement(By.CssSelector(".show-all-destinations-btn"));
                if (showMoreButton != null) showMoreButton.Click();
            }
            catch (NoSuchElementException)
            {
                //No button to click
            }
        }
    }
}