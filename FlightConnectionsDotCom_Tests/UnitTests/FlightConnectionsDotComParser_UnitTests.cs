using FlightConnectionsDotCom_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class FlightConnectionsDotComParser_UnitTests
    {
        Mock<IWebDriver> driverMock;
        Mock<IJavaScriptExecutorWithDelayer> jsExecutorWithDelayerMock;
        Mock<INavigationWorker> navigationWorkerMock;
        Mock<IDelayer> delayerMock;
        Mock<IWebElementWorker> webElementWorker;
        Mock<ILogger> logger;

        private void InitialiseMockObjects()
        {
            driverMock = new();
            jsExecutorWithDelayerMock = new();
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

            jsExecutorWithDelayerMock.SetupSequence(x => x.RunScriptAndGetElement(commands.GetPopularDestinationsDiv).Result)
                .Returns(popularDestinationsDivMock1)
                .Returns(popularDestinationsDivMock2)
                .Returns(popularDestinationsDivMock3);

            jsExecutorWithDelayerMock.Setup(x => x.RunScriptAndGetElements(commands.GetPopularDestinationsEntries, popularDestinationsDivMock1).Result).Returns(entries1);
            jsExecutorWithDelayerMock.Setup(x => x.RunScriptAndGetElements(commands.GetPopularDestinationsEntries, popularDestinationsDivMock2).Result).Returns(entries2);
            jsExecutorWithDelayerMock.Setup(x => x.RunScriptAndGetElements(commands.GetPopularDestinationsEntries, popularDestinationsDivMock3).Result).Returns(entries3);
            
            jsExecutorWithDelayerMock.Setup(x => x.RunScriptAndGetString(commands.GetDestinationFromEntry, mockEntry1).Result).Returns($"gg ({airport1.Code})");
            jsExecutorWithDelayerMock.Setup(x => x.RunScriptAndGetString(commands.GetDestinationFromEntry, mockEntry2).Result).Returns($"cc ({airport2.Code})");
            jsExecutorWithDelayerMock.Setup(x => x.RunScriptAndGetString(commands.GetDestinationFromEntry, mockEntry3).Result).Returns($"dd ({airport3.Code})");

            FlightConnectionsDotComParser siteParser = new(driverMock.Object, jsExecutorWithDelayerMock.Object, navigationWorkerMock.Object, delayerMock.Object, webElementWorker.Object, logger.Object);
            Dictionary<string, HashSet<string>> result = await siteParser.GetAirportsAndTheirConnections(airports, commands);
            Assert.IsTrue(result[airport1.Code].Count == 2);
            Assert.IsTrue(result[airport1.Code].Contains(airport2.Code));
            Assert.IsTrue(result[airport1.Code].Contains(airport3.Code));
            Assert.IsTrue(result[airport2.Code].Count == 1);
            Assert.IsTrue(result[airport2.Code].Contains(airport1.Code));
            Assert.IsTrue(result[airport3.Code].Count == 0);
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

            jsExecutorWithDelayerMock.Setup(x => x.RunScriptAndGetElements(commands.GetAirportListEntries).Result).Returns(
                new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { airportListEntryObject1, airportListEntryObject2, airportListEntryObject3, airportListEntryObject4 })
            );

            FlightConnectionsDotComParser siteParser = new(driverMock.Object, jsExecutorWithDelayerMock.Object, navigationWorkerMock.Object, delayerMock.Object, webElementWorker.Object, logger.Object);
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
            jsExecutorWithDelayerMock.Setup(x => x.RunScriptAndGetString(commands.GetAirportCodeFromEntry, airportListEntryObject).Result).Returns(airport.Code);
            jsExecutorWithDelayerMock.Setup(x => x.RunScriptAndGetString(commands.GetAirportCityAndCountryFromEntry, airportListEntryObject).Result).Returns($"{airport.City}, {airport.Country}");
            jsExecutorWithDelayerMock.Setup(x => x.RunScriptAndGetString(commands.GetAirportNameFromEntry, airportListEntryObject).Result).Returns(airport.Name);
            jsExecutorWithDelayerMock.Setup(x => x.RunScriptAndGetString(commands.GetAirportLinkFromEntry, airportListEntryObject).Result).Returns(airport.Link);
            return airportListEntryObject;
        }
    }
}
