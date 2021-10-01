using Common_ClassLibrary;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class FlightConnectionsDotComWorker_AirportCollector
    {
        private const string collectingAirports = "Collecting airports";
        private FlightConnectionsDotComWorker Worker { get; set; }
        public FlightConnectionsDotComWorker_AirportCollector(FlightConnectionsDotComWorker worker)
        {
            Worker = worker;
        }

        public List<Airport> CollectAirports(int maxCountToCollect = 0)
        {
            Worker.Logger.Log($"Navigating to airports page...");
            List<Airport> airports = new();
            INavigation navigation = Worker.Driver.Navigate();
            Worker.GoToUrl(navigation, "https://www.flightconnections.com/airport-codes");

            ReadOnlyCollection<IWebElement> airportListEntries = Worker.Driver.FindElements(By.TagName("li"));

            int countToCollect = airportListEntries.Count;
            if (maxCountToCollect > 0) countToCollect = maxCountToCollect > airportListEntries.Count ? airportListEntries.Count : maxCountToCollect;

            Worker.Logger.Log($"{collectingAirports} {countToCollect} airports...");
            for (int i = 0; i < countToCollect; i++)
            {
                IWebElement airportListEntry = airportListEntries[i];
                Airport airport = CreateAirportFromAirportEntry(airportListEntry);
                airports.Add(airport);
                Worker.Logger.Log($"Collected airport ({Globals.GetPercentageAndCountString(i, countToCollect)} airports done) {airport.GetFullString()}.");
            }
            Worker.Logger.Log($"Finished {collectingAirports} ({countToCollect} airports).");
            return airports;
        }

        private static Airport CreateAirportFromAirportEntry(IWebElement airportListEntry)
        {
            string code = airportListEntry.FindElement(By.CssSelector(".airport-code")).Text;
            string airportCityAndCountry = airportListEntry.FindElement(By.CssSelector(".airport-city-country")).Text;
            Match match = Regex.Match(airportCityAndCountry, "(.*?), (.*)");
            string city = match.Groups[1].Value;
            string country = match.Groups[2].Value;
            string name = airportListEntry.FindElement(By.CssSelector(".airport-name")).Text;
            string link = airportListEntry.FindElement(By.CssSelector("a")).GetAttribute("href");
            return new Airport(code, city, country, name, link);
        }
    }
}