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

namespace FlightConnectionsDotCom_Tests.IntegrationTests
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
        JavaScriptExecutorWithDelayer jsExecutorWithDelay;

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
            jsExecutorWithDelay = new(chromeDriver, delayer, 10);
            navigationWorker = new(jsExecutorWithDelay, chromeDriver, new ClosePrivacyPopupCommands());
            siteParser = new(chromeDriver, jsExecutorWithDelay, navigationWorker, delayer, webElementWorker, logger);
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
            Airport airport2 = new("KQT", "asd", "asd", "asd", "https://www.flightconnections.com/flights-to-qurghonteppa-kqt");
            List<Airport> airports = new() { airport1, airport2 };
            Dictionary<string, HashSet<string>> results = await siteParser.GetAirportsAndTheirConnections(airports, getAirportsAndTheirConnectionsCommands);
            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results[airport1.Code].Count > 0);
            Assert.IsTrue(results[airport2.Code].Count > 0);
        }
        
        [Ignore]
        [TestMethod]
        public async Task FullTest_JsonsReceived()
        {
            const string airportListJsonFileName = "airportsListJson.json";

            //HashSet<Airport> airports = await siteParser.CollectAirports(new CollectAirportCommands());
            //List<Airport> airportsList = airports.OrderBy(x => x.Code).ToList();
            List<Airport> airportsList = JsonConvert.DeserializeObject<List<Airport>>(File.ReadAllText(airportListJsonFileName));

            string airportsListJson = JsonConvert.SerializeObject(airportsList, Formatting.Indented);
            File.WriteAllText(airportListJsonFileName, airportsListJson);

            Dictionary<string, HashSet<string>> results = await siteParser.GetAirportsAndTheirConnections(airportsList, getAirportsAndTheirConnectionsCommands);
            string resultsListJson = JsonConvert.SerializeObject(results);
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
