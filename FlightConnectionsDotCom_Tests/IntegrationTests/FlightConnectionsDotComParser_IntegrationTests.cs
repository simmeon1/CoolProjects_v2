using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.IO;

namespace FlightConnectionsDotCom_Tests.IntegrationTests
{
    [TestClass]
    public class FlightConnectionsDotComParser_IntegrationTests
    {

        ChromeDriver chromeDriver;
        Logger_Debug logger;
        FlightConnectionsDotComParser siteParser;

        [TestInitialize]
        public void TestInitialize()
        {
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");
            chromeDriver = new(chromeOptions);
            logger = new();
            siteParser = new(chromeDriver, logger);
        }


        [TestMethod]
        public void CollectAirports_ReturnsValues()
        {
            List<Airport> results = siteParser.CollectAirports(maxCountToCollect: 10);
            Assert.IsTrue(results.Count == 10);
        }
        
        [TestMethod]
        public void GetAirportsAndTheirConnections_ReturnsValuesAndHandlesExceptions()
        {
            Airport airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "https://www.flightconnections.com/flights-to-aberdeen-abz");
            Airport airport2 = new("KQT", "asd", "asd", "asd", "https://www.flightconnections.com/flights-to-qurghonteppa-kqt");
            Airport airport3 = new("BWE", "asd", "asd", "asd", "https://www.flightconnections.com/flights-to-braunschweig-bwe");
            List<Airport> airports = new() { airport1, airport2, airport3 };
            Dictionary<string, HashSet<string>> results = siteParser.GetAirportsAndTheirConnections(airports);
            Assert.IsTrue(results.Count == 3);
            Assert.IsTrue(results[airport1.Code].Count > 0);
            Assert.IsTrue(results[airport2.Code].Count > 0);
            Assert.IsTrue(results[airport3.Code].Count == 0);
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

            Dictionary<string, HashSet<string>> results = siteParser.GetAirportsAndTheirConnections(airportsList);
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
