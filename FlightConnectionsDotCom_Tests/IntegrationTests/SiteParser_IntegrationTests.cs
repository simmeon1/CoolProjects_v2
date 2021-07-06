using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests_IntegrationTests
{
    [TestClass]
    public class SiteParser_IntegrationTests
    {

        ChromeDriver chromeDriver;
        NavigationWorker navigationWorker;
        Delayer delayer;
        WebElementWorker webElementWorker;
        CollectAirportCommands collectAirportCommands;
        GetAirportsAndTheirConnectionsCommands getAirportsAndTheirConnectionsCommands;
        SiteParser siteParser;
        Logger_Debug logger;

        [TestInitialize]
        public void TestInitialize()
        {
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");
            chromeDriver = new(chromeOptions);
            delayer = new();
            webElementWorker = new();
            collectAirportCommands = new();
            getAirportsAndTheirConnectionsCommands = new();
            logger = new();
            navigationWorker = new(chromeDriver, delayer, new ClosePrivacyPopupCommands());
            siteParser = new(chromeDriver, chromeDriver, navigationWorker, delayer, webElementWorker, logger);
        }


        [TestMethod]
        public async Task CollectAirports_ReturnsValues()
        {
            HashSet<Airport> results = await siteParser.CollectAirports(collectAirportCommands);
            Assert.IsTrue(results.Count > 0);
        }
        
        [TestMethod]
        public async Task GetAirportsAndTheirConnections_ReturnsValues()
        {
            Airport airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "https://www.flightconnections.com/flights-to-aberdeen-abz");
            Airport airport2 = new("AMS", "Amsterdam", "Netherlands", "Schiphol Airport", "https://www.flightconnections.com/flights-to-amsterdam-ams");
            List<Airport> airports = new() { airport1, airport2 };
            Dictionary<Airport, HashSet<Airport>> results = await siteParser.GetAirportsAndTheirConnections(airports, getAirportsAndTheirConnectionsCommands);
            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results[airport1].Count > 0);
            Assert.IsTrue(results[airport2].Count > 0);
        }
        
        [Ignore]
        [TestMethod]
        public async Task FullTest_JsonsReceived()
        {
            HashSet<Airport> airports = await siteParser.CollectAirports(new CollectAirportCommands());
            List<Airport> airportsList = airports.OrderBy(x => x.Code).ToList();
            string airportsListJson = JsonConvert.SerializeObject(airportsList, Formatting.Indented);
            File.WriteAllText("airportsListJson.json", airportsListJson);

            Dictionary<Airport, HashSet<Airport>> results = await siteParser.GetAirportsAndTheirConnections(airportsList, getAirportsAndTheirConnectionsCommands);
            string resultsListJson = JsonConvert.SerializeObject(results, Formatting.Indented);
            File.WriteAllText("resultsListJson.json", resultsListJson);
            Assert.IsTrue(results.Count > 0);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            chromeDriver.Quit();
        }
    }
}
