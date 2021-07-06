using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests_UnitTests
{
    [TestClass]
    public class SiteParser_UnitTests
    {
        Mock<IWebDriver> driverMock;
        Mock<IJavaScriptExecutor> jsExecutorMock;
        Mock<INavigationWorker> navigationWorkerMock;
        Mock<IDelayer> delayerMock;
        Mock<IWebElementWorker> webElementWorker;
        Mock<ILogger> logger;

        private void InitialiseMockObjects()
        {
            driverMock = new();
            jsExecutorMock = new();
            navigationWorkerMock = new();
            delayerMock = new();
            webElementWorker = new();
            logger = new();
        }

        [TestMethod]
        public async Task GetAirportsAndTheirConnections_ReturnsExpectedResults()
        {
            InitialiseMockObjects();
            Airport airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "linkA");
            Airport airport2 = new("SOF", "Sofia", "Bulgaria", "Sofia Airport", "linkB");
            Airport airport3 = new("EDI", "Edinburgh", "United Kingdom", "Edinburgh Airport", "linkC");
            List<Airport> airports = new() { airport1, airport2, airport3 };

            GetAirportsAndTheirConnectionsCommands commands = new();
            IWebElement popularDestinationsDivMock1 = new Mock<IWebElement>().Object;
            IWebElement popularDestinationsDivMock2 = new Mock<IWebElement>().Object;
            IWebElement popularDestinationsDivMock3 = new Mock<IWebElement>().Object;
            
            IWebElement mockEntry1 = new Mock<IWebElement>().Object;
            IWebElement mockEntry2 = new Mock<IWebElement>().Object;
            IWebElement mockEntry3 = new Mock<IWebElement>().Object;

            ReadOnlyCollection<IWebElement> entries1 = new(new List<IWebElement>() { mockEntry2, mockEntry3 });
            ReadOnlyCollection<IWebElement> entries2 = new(new List<IWebElement>() { mockEntry1 });
            ReadOnlyCollection<IWebElement> entries3 = new(new List<IWebElement>());

            jsExecutorMock.SetupSequence(x => x.ExecuteScript(commands.GetPopularDestinationsDiv))
                .Returns(popularDestinationsDivMock1)
                .Returns(popularDestinationsDivMock2)
                .Returns(popularDestinationsDivMock3);

            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetPopularDestinationsEntries, popularDestinationsDivMock1)).Returns(entries1);
            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetPopularDestinationsEntries, popularDestinationsDivMock2)).Returns(entries2);
            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetPopularDestinationsEntries, popularDestinationsDivMock3)).Returns(entries3);
            
            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetDestinationFromEntry, mockEntry1)).Returns($"gg ({airport1.Code})");
            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetDestinationFromEntry, mockEntry2)).Returns($"cc ({airport2.Code})");
            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetDestinationFromEntry, mockEntry3)).Returns($"dd ({airport3.Code})");

            SiteParser siteParser = new(driverMock.Object, jsExecutorMock.Object, navigationWorkerMock.Object, delayerMock.Object, webElementWorker.Object, logger.Object);
            Dictionary<Airport, HashSet<Airport>> result = await siteParser.GetAirportsAndTheirConnections(airports, commands);
            Assert.IsTrue(result[airport1].Count == 2);
            Assert.IsTrue(result[airport1].Contains(airport2));
            Assert.IsTrue(result[airport1].Contains(airport3));
            Assert.IsTrue(result[airport2].Count == 1);
            Assert.IsTrue(result[airport2].Contains(airport1));
            Assert.IsTrue(result[airport3].Count == 0);
        }

        [TestMethod]
        public async Task CollectAirports_ReturnsExpectedAirports()
        {
            InitialiseMockObjects();
            CollectAirportCommands commands = new();
            Airport airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "linkA");
            Airport airport2 = new("SOF", "Sofia", "Bulgaria", "Sofia Airport", "linkB");
            Airport airport3 = new("EDI", "Edinburgh", "United Kingdom", "Edinburgh Airport", "linkC");
            Airport airport4 = new("CIA", "Rome", "Spain", "Rome Ciampino", "linkD");

            IWebElement airportListEntryObject1 = SetUpAirportListEntryData(commands, airport1);
            IWebElement airportListEntryObject2 = SetUpAirportListEntryData(commands, airport2);
            IWebElement airportListEntryObject3 = SetUpAirportListEntryData(commands, airport3);
            IWebElement airportListEntryObject4 = SetUpAirportListEntryData(commands, airport4);

            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetAirportListEntries)).Returns(
                new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { airportListEntryObject1, airportListEntryObject2, airportListEntryObject3, airportListEntryObject4 })
            );

            SiteParser siteParser = new(driverMock.Object, jsExecutorMock.Object, navigationWorkerMock.Object, delayerMock.Object, webElementWorker.Object, logger.Object);
            HashSet<Airport> results = await siteParser.CollectAirports(commands);
            Assert.IsTrue(results.Count == 4);
            Assert.IsTrue(results.Contains(airport1));
            Assert.IsTrue(results.Contains(airport2));
            Assert.IsTrue(results.Contains(airport3));
            Assert.IsTrue(results.Contains(airport4));
        }

        private IWebElement SetUpAirportListEntryData(CollectAirportCommands commands, Airport airport)
        {
            Mock<IWebElement> airportListEntry = new();
            IWebElement airportListEntryObject = airportListEntry.Object;
            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetAirportCodeFromEntry, airportListEntryObject)).Returns(airport.Code);
            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetAirportCityAndCountryFromEntry, airportListEntryObject)).Returns($"{airport.City}, {airport.Country}");
            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetAirportNameFromEntry, airportListEntryObject)).Returns(airport.Name);
            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetAirportLinkFromEntry, airportListEntryObject)).Returns(airport.Link);
            return airportListEntryObject;
        }
    }
}
