using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests
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
            navigationWorker = new(chromeDriver, delayer, new ClosePrivacyPopupCommands());
            siteParser = new(chromeDriver, chromeDriver, navigationWorker, delayer, webElementWorker);
        }


        [TestMethod]
        public void CollectAirports_ReturnsValues()
        {
            List<Airport> results = siteParser.CollectAirports(collectAirportCommands);
            Assert.IsTrue(results.Count > 0);
        }
        
        [TestMethod]
        public async Task GetAirportsAndTheirConnections_ReturnsValues()
        {
            Airport airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "https://www.flightconnections.com/flights-to-aberdeen-abz");
            Airport airport2 = new("SOF", "Sofia", "Bulgaria", "Sofia Airport", "https://www.flightconnections.com/flights-to-sofia-sof");
            List<Airport> airports = new() { airport1, airport2 };
            Dictionary<Airport, HashSet<Airport>> results = await siteParser.GetAirportsAndTheirConnections(airports, getAirportsAndTheirConnectionsCommands);
            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results[airport1].Count > 0);
            Assert.IsTrue(results[airport2].Count > 0);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            chromeDriver.Quit();
        }
    }
}
