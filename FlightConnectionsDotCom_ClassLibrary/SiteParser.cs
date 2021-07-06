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
        private IJavaScriptExecutor JSExecutor { get; set; }
        private INavigationWorker NavigationWorker { get; set; }
        private IDelayer Delayer { get; set; }
        private IWebElementWorker WebElementWorker { get; set; }
        public SiteParser(IWebDriver driver, IJavaScriptExecutor jSExecutor, INavigationWorker navigationWorker, IDelayer delayer, IWebElementWorker webElementWorker)
        {
            Driver = driver;
            JSExecutor = jSExecutor;
            NavigationWorker = navigationWorker;
            Delayer = delayer;
            WebElementWorker = webElementWorker;
        }

        public async Task<Dictionary<Airport, HashSet<Airport>>> GetAirportsAndTheirConnections(List<Airport> airports, GetAirportsAndTheirConnectionsCommands commands)
        {
            Dictionary<Airport, HashSet<Airport>> results = new();
            Dictionary<string, Airport> codesAndAirports = new();
            foreach (Airport airport in airports) codesAndAirports.Add(airport.Code, airport);

            foreach (Airport airport in airports)
            {
                INavigation navigation = Driver.Navigate();
                await NavigationWorker.GoToUrl(navigation, (airport.Link));

                IWebElement showMoreButton = (IWebElement)JSExecutor.ExecuteScript(commands.GetShowMoreButton);
                if (showMoreButton != null)
                {
                    WebElementWorker.Click(showMoreButton);
                    await Delayer.Delay(1000);
                }

                IWebElement popularDestinationsDiv = (IWebElement)JSExecutor.ExecuteScript(commands.GetPopularDestinationsDiv);
                ReadOnlyCollection<IWebElement> popularDestinationsEntries = (ReadOnlyCollection<IWebElement>)JSExecutor.ExecuteScript(commands.GetPopularDestinationsEntries, popularDestinationsDiv);

                HashSet<Airport> destinations = new();
                foreach (IWebElement entry in popularDestinationsEntries)
                {
                    string destination = (string)JSExecutor.ExecuteScript(commands.GetDestinationFromEntry, entry);
                    Match match = Regex.Match(destination, @"(.*?) \((...)\)$");
                    string name = match.Groups[1].Value;
                    string code = match.Groups[2].Value;
                    destinations.Add(codesAndAirports.ContainsKey(code) ? codesAndAirports[code] : new Airport(code, "", "", name, ""));
                }
                results.Add(airport, destinations);
            }
            return results;
        }

        public List<Airport> CollectAirports(CollectAirportCommands commands)
        {
            List<Airport> airports = new();
            INavigation navigation = Driver.Navigate();
            NavigationWorker.GoToUrl(navigation, ("https://www.flightconnections.com/airport-codes"));

            ReadOnlyCollection<IWebElement> airportListEntries = (ReadOnlyCollection<IWebElement>)JSExecutor.ExecuteScript(commands.GetAirportListEntries);
            foreach (IWebElement airportListEntry in airportListEntries)
            {
                string code = (string)JSExecutor.ExecuteScript(commands.GetAirportCodeFromEntry, airportListEntry);
                string airportCityAndCountry = (string)JSExecutor.ExecuteScript(commands.GetAirportCityAndCountryFromEntry, airportListEntry);
                Match match = Regex.Match(airportCityAndCountry, "(.*?), (.*)");
                string city = match.Groups[1].Value;
                string country = match.Groups[2].Value;
                string name = (string)JSExecutor.ExecuteScript(commands.GetAirportNameFromEntry, airportListEntry);
                string link = (string)JSExecutor.ExecuteScript(commands.GetAirportLinkFromEntry, airportListEntry);
                airports.Add(new Airport(code, city, country, name, link));
            }
            return airports.OrderBy(a => a.Code).ToList();
        }
    }
}