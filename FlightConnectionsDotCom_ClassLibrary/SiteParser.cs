using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class SiteParser
    {
        public SiteParser(IWebDriver driver, IJavaScriptExecutor jSExecutor, INavigationWorker navigationWorker, IDelayer delayer, IWebElementWorker webElementWorker)
        {
            Driver = driver;
            JSExecutor = jSExecutor;
            NavigationWorker = navigationWorker;
            Delayer = delayer;
            WebElementWorker = webElementWorker;
        }

        private IWebDriver Driver { get; set; }
        private IJavaScriptExecutor JSExecutor { get; set; }
        private INavigationWorker NavigationWorker { get; set; }
        private IDelayer Delayer { get; set; }
        private IWebElementWorker WebElementWorker { get; set; }

        public async Task<Dictionary<string, List<string>>> GetAirportsAndTheirConnections()
        {
            INavigation navigation = Driver.Navigate();
            NavigationWorker.GoToUrl(navigation, ("https://www.flightconnections.com/"));

            IWebElement fromDiv = (IWebElement)JSExecutor.ExecuteScript("return document.querySelector('#from')");
            WebElementWorker.Click(fromDiv);
            await Delayer.Delay(200);

            IWebElement fromDivInput = (IWebElement)JSExecutor.ExecuteScript("return document.querySelector('#from-input')");
            WebElementWorker.Click(fromDivInput);
            await Delayer.Delay(200);

            WebElementWorker.SendKeys(fromDivInput, "EDI");
            await Delayer.Delay(500);

            IWebElement fromDropdownList = (IWebElement)JSExecutor.ExecuteScript("return document.querySelector('#ui-id-1')");
            if (fromDropdownList != null)
            {
                int fromDropdownCount = (int)JSExecutor.ExecuteScript("return document.querySelector('#ui-id-1').getElementsByTagName('li').length");
                if (fromDropdownCount > 0)
                {
                    IWebElement fromDropdownFirstOption = (IWebElement)JSExecutor.ExecuteScript("return document.querySelector('#ui-id-1').querySelectorAll('li')[0]");
                    WebElementWorker.Click(fromDropdownFirstOption);
                    await Delayer.Delay(500);
                }
            }

            Dictionary<string, List<string>> results = new();
            results.Add("EDI", new List<string>() { "SOF" });
            results.Add("SOF", new List<string>() { "EDI" });
            return results;
        }

        public List<Airport> CollectAirports(CollectAirportCommands commands)
        {
            List<Airport> airports = new();
            INavigation navigation = Driver.Navigate();
            NavigationWorker.GoToUrl(navigation, ("https://www.flightconnections.com/airport-codes"));

            List<IWebElement> airportLists = (List<IWebElement>)JSExecutor.ExecuteScript(commands.GetAirportLists);
            foreach (IWebElement airportList in airportLists)
            {
                List<IWebElement> airportListEntries = (List<IWebElement>)JSExecutor.ExecuteScript(commands.GetAirportListEntries, airportList);
                foreach (IWebElement airportListEntry in airportListEntries)
                {
                    string code = (string)JSExecutor.ExecuteScript(commands.GetAirportCodeFromEntry, airportListEntry);
                    string airportCityAndCountry = (string)JSExecutor.ExecuteScript(commands.GetAirportCityAndCountryFromEntry, airportListEntry);
                    Match match = Regex.Match(airportCityAndCountry, "(.*?), (.*)");
                    string city = match.Groups[1].Value;
                    string country = match.Groups[2].Value;
                    string name = (string)JSExecutor.ExecuteScript(commands.GetAirportNameFromEntry, airportListEntry);
                    airports.Add(new Airport(code, city, country, name));
                }
            }
            return airports.OrderBy(a => a.Code).ToList();
        }
    }
}