using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class FlightConnectionsDotComParser_UnitTests
    {
        Mock<IWebDriver> driverMock;
        Mock<ILogger> logger;
        Mock<IWebDriverWait> webDriverWait;
        private Airport airport1;
        private Airport airport2;
        private Airport airport3;
        private Airport airport4;
        private Airport airport5;

        [TestInitialize]
        public void TestInitialize()
        {
            driverMock = new();
            logger = new();
            webDriverWait = new();
            airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "linkA");
            airport2 = new("SOF", "Sofia", "Bulgaria", "Sofia Airport", "linkB");
            airport3 = new("EDI", "Edinburgh", "United Kingdom", "Edinburgh Airport", "linkC");
            airport4 = new("CIA", "Rome", "Spain", "Rome Ciampino", "linkD");
            airport5 = new("USA", "USA", "USA", "USA", "USA");
        }

        [TestMethod]
        public void GetAirportsAndTheirConnections_ReturnsExpectedResults()
        {
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

            FlightConnectionsDotComParser siteParser = new(driverMock.Object, logger.Object, webDriverWait.Object);
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
            RunCollectAirportsTest(new List<IWebElement>() { SetUpAirportListEntryData(airport1), SetUpAirportListEntryData(airport2), SetUpAirportListEntryData(airport3), SetUpAirportListEntryData(airport4) }, 4);
        }

        [TestMethod]
        public void CollectAirports_ReturnsExpectedAirports_MaxCountToCollectIsOne()
        {
            RunCollectAirportsTest(new List<IWebElement>() { SetUpAirportListEntryData(airport1), SetUpAirportListEntryData(airport2), SetUpAirportListEntryData(airport3), SetUpAirportListEntryData(airport4) }, 1, maxCountToCollect: 1);
        }
        
        [TestMethod]
        public void CollectAirports_ReturnsExpectedAirports_MaxCountToCollectIsOne_NoAirportsToCollect()
        {
            RunCollectAirportsTest(new List<IWebElement>(), 0, maxCountToCollect: 1, waitUntilThrowsException: true);
        }
        
        [TestMethod]
        public void CollectAirports_ReturnsExpectedAirports_AmericanAirportExcluded()
        {
            RunCollectAirportsTest(new List<IWebElement>() { SetUpAirportListEntryData(airport5) }, 0, europeOnly: true, agreeButtonContainsFakeText: true);
        }

        private void RunCollectAirportsTest(
            List<IWebElement> airportListEntries,
            int resultCountExpectation,
            int maxCountToCollect = 0,
            bool europeOnly = false,
            bool navigationIsFalse = false,
            bool waitUntilThrowsException = false,
            bool agreeButtonContainsFakeText = false
            )
        {
            driverMock.Setup(x => x.Navigate()).Returns(navigationIsFalse ? null : new Mock<INavigation>().Object);
            driverMock.Setup(x => x.FindElements(By.TagName("li"))).Returns(new ReadOnlyCollection<IWebElement>(airportListEntries));
            Mock<IAlert> alert = new();
            if (waitUntilThrowsException) alert.Setup(x => x.Accept()).Throws(new WebDriverTimeoutException());
            webDriverWait.Setup(x => x.Until(It.IsAny<Func<IWebDriver, IAlert>>())).Returns(alert.Object);

            Mock<IWebElement> searchButton = new();
            searchButton.Setup(x => x.Text).Returns(agreeButtonContainsFakeText ? "Fake" : "AGREE");
            driverMock.Setup(x => x.FindElements(By.CssSelector("button"))).Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { searchButton.Object }));

            FlightConnectionsDotComParser siteParser = new(driverMock.Object, logger.Object, webDriverWait.Object);
            List<Airport> results = siteParser.CollectAirports(maxCountToCollect: maxCountToCollect, europeOnly);
            Assert.IsTrue(results.Count == resultCountExpectation);
            if (results.Count > 0) Assert.IsTrue(results[0].Equals(airport1));
            if (results.Count > 1) Assert.IsTrue(results[1].Equals(airport2));
            if (results.Count > 2) Assert.IsTrue(results[2].Equals(airport3));
            if (results.Count > 3) Assert.IsTrue(results[3].Equals(airport4));
        }

        private static IWebElement SetUpAirportListEntryData(Airport airport)
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
