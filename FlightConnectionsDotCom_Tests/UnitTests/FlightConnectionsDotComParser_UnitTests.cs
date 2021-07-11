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
        Mock<ILogger> logger;

        private void InitialiseMockObjects()
        {
            driverMock = new();
            logger = new();
        }

        [TestMethod]
        public void GetAirportsAndTheirConnections_ReturnsExpectedResults()
        {
            InitialiseMockObjects();
            Airport airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "linkA");
            Airport airport2 = new("SOF", "Sofia", "Bulgaria", "Sofia Airport", "linkB");
            Airport airport3 = new("EDI", "Edinburgh", "United Kingdom", "Edinburgh Airport", "linkC");
            List<Airport> airports = new() { airport1, airport2, airport3 };

            Mock<IWebElement> mockEntry1 = new();
            Mock<IWebElement> mockEntry2 = new();
            Mock<IWebElement> mockEntry3 = new();
            mockEntry1.Setup(x => x.GetAttribute("data-a")).Returns($"gg ({airport1.Code})");
            mockEntry2.Setup(x => x.GetAttribute("data-a")).Returns($"cc ({airport2.Code})");
            mockEntry3.Setup(x => x.GetAttribute("data-a")).Returns($"dd ({airport3.Code})");

            IWebElement mockEntry1Object = mockEntry1.Object;
            IWebElement mockEntry2Object = mockEntry2.Object;
            IWebElement mockEntry3Object = mockEntry3.Object;
            ReadOnlyCollection<IWebElement> entries1 = new(new List<IWebElement>() { mockEntry2Object, mockEntry3Object });
            ReadOnlyCollection<IWebElement> entries2 = new(new List<IWebElement>() { mockEntry1Object });
            ReadOnlyCollection<IWebElement> entries3 = new(new List<IWebElement>());

            Mock<IWebElement> popularDestinationsDivMock1 = new();
            Mock<IWebElement> popularDestinationsDivMock2 = new();
            Mock<IWebElement> popularDestinationsDivMock3 = new();
            popularDestinationsDivMock1.Setup(x => x.FindElements(By.CssSelector(".popular-destination"))).Returns(entries1);
            popularDestinationsDivMock2.Setup(x => x.FindElements(By.CssSelector(".popular-destination"))).Returns(entries2);
            popularDestinationsDivMock3.Setup(x => x.FindElements(By.CssSelector(".popular-destination"))).Returns(entries3);

            IWebElement popularDestinationsDivMock1Object = popularDestinationsDivMock1.Object;
            IWebElement popularDestinationsDivMock2Object = popularDestinationsDivMock2.Object;
            IWebElement popularDestinationsDivMock3Object = popularDestinationsDivMock3.Object;
            driverMock.SetupSequence(x => x.FindElement(By.CssSelector("#popular-destinations")))
                .Returns(popularDestinationsDivMock1Object)
                .Returns(popularDestinationsDivMock2Object)
                .Returns(popularDestinationsDivMock3Object);

            FlightConnectionsDotComParser siteParser = new(driverMock.Object, logger.Object);
            Dictionary<string, HashSet<string>> result = siteParser.GetAirportsAndTheirConnections(airports);
            Assert.IsTrue(result[airport1.Code].Count == 2);
            Assert.IsTrue(result[airport1.Code].Contains(airport2.Code));
            Assert.IsTrue(result[airport1.Code].Contains(airport3.Code));
            Assert.IsTrue(result[airport2.Code].Count == 1);
            Assert.IsTrue(result[airport2.Code].Contains(airport1.Code));
            Assert.IsTrue(result[airport3.Code].Count == 0);
        }

        [TestMethod]
        public void CollectAirports_ReturnsExpectedAirports()
        {
            InitialiseMockObjects();
            Airport airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "linkA");
            Airport airport2 = new("SOF", "Sofia", "Bulgaria", "Sofia Airport", "linkB");
            Airport airport3 = new("EDI", "Edinburgh", "United Kingdom", "Edinburgh Airport", "linkC");
            Airport airport4 = new("CIA", "Rome", "Spain", "Rome Ciampino", "linkD");

            IWebElement airportListEntryObject1 = SetUpAirportListEntryData(airport1);
            IWebElement airportListEntryObject2 = SetUpAirportListEntryData(airport2);
            IWebElement airportListEntryObject3 = SetUpAirportListEntryData(airport3);
            IWebElement airportListEntryObject4 = SetUpAirportListEntryData(airport4);

            driverMock.Setup(x => x.FindElements(By.TagName("li"))).Returns(
                new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { airportListEntryObject1, airportListEntryObject2, airportListEntryObject3, airportListEntryObject4 })
            );

            FlightConnectionsDotComParser siteParser = new(driverMock.Object, logger.Object);
            HashSet<Airport> results = siteParser.CollectAirports();
            Assert.IsTrue(results.Count == 4);
            Assert.IsTrue(results.Contains(airport1));
            Assert.IsTrue(results.Contains(airport2));
            Assert.IsTrue(results.Contains(airport3));
            Assert.IsTrue(results.Contains(airport4));
        }

        private IWebElement SetUpAirportListEntryData(Airport airport)
        {
            Mock<IWebElement> airportListEntry = new();
            airportListEntry.Setup(x => x.FindElement(By.CssSelector(".airport-code")).Text).Returns(airport.Code);
            airportListEntry.Setup(x => x.FindElement(By.CssSelector(".airport-city-country")).Text).Returns($"{airport.City}, {airport.Country}");
            airportListEntry.Setup(x => x.FindElement(By.CssSelector(".airport-name")).Text).Returns(airport.Name);
            airportListEntry.Setup(x => x.FindElement(By.CssSelector("a")).GetAttribute("href")).Returns(airport.Link);
            IWebElement airportListEntryObject = airportListEntry.Object;
            return airportListEntryObject;
        }
    }
}
