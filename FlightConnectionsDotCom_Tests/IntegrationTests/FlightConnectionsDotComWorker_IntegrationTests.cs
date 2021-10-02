using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.IO;

namespace FlightConnectionsDotCom_Tests.IntegrationTests
{
    [TestClass]
    public class FlightConnectionsDotComWorker_IntegrationTests
    {

        ChromeDriver chromeDriver;
        Logger_Debug logger;
        FlightConnectionsDotComWorker worker;

        [TestInitialize]
        public void TestInitialize()
        {
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");
            chromeOptions.AddArgument("disable-gpu");
            chromeOptions.AddArgument("window-size=1280,800");
            chromeOptions.AddArgument("allow-insecure-localhost");
            chromeDriver = new(chromeOptions);
            logger = new();
            worker = new(chromeDriver, logger, new RealWebDriverWait(chromeDriver));
        }

        [TestMethod]
        public void CollectAirports_ReturnsValues()
        {
            FlightConnectionsDotComWorker_AirportCollector collector = new(worker);
            List<Airport> results = collector.CollectAirports(maxCountToCollect: 10);
            Assert.IsTrue(results.Count == 10);
        }
        
        [TestMethod]
        public void GetAirportsAndTheirConnections_ReturnsValuesAndHandlesExceptions()
        {
            Airport airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "https://www.flightconnections.com/flights-to-aberdeen-abz");
            Airport airport2 = new("LHR", "asd", "asd", "asd", "https://www.flightconnections.com/flights-to-london-heathrow-lhr");
            List<Airport> airports = new() { airport1, airport2 };
            FlightConnectionsDotComWorker_AirportPopulator populator = new(worker);
            Dictionary<string, HashSet<string>> results = populator.PopulateAirports(airports);
            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results[airport1.Code].Count > 0);
            Assert.IsTrue(results[airport2.Code].Count > 0);
        }
        
        [Ignore]
        [TestMethod]
        public void FullTest_JsonsReceived()
        {
            const string airportListJsonFileName = "airportsListJson.json";

            //HashSet<Airport> airports = await siteParser.CollectAirports(new CollectAirportCommands());
            //List<Airport> airportsList = airports.OrderBy(x => x.Code).ToList();
            List<Airport> airportsList = JsonConvert.DeserializeObject<List<Airport>>(File.ReadAllText(airportListJsonFileName));

            string airportsListJson = JsonConvert.SerializeObject(airportsList);
            File.WriteAllText(airportListJsonFileName, airportsListJson);

            FlightConnectionsDotComWorker_AirportPopulator populator = new(worker);
            Dictionary<string, HashSet<string>> results = populator.PopulateAirports(airportsList);
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
