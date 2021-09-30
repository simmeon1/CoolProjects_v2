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
    public class FlightConnectionsDotComParser_AirportCollector_UnitTests
    {
        Mock<IWebDriver> driverMock;
        Mock<ILogger> logger;
        Mock<IWebDriverWait> webDriverWait;
        FlightConnectionsDotComWorker worker;
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
            worker = new(driverMock.Object, logger.Object, webDriverWait.Object);
            airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "linkA");
            airport2 = new("SOF", "Sofia", "Bulgaria", "Sofia Airport", "linkB");
            airport3 = new("EDI", "Edinburgh", "United Kingdom", "Edinburgh Airport", "linkC");
            airport4 = new("CIA", "Rome", "Spain", "Rome Ciampino", "linkD");
            airport5 = new("USA", "USA", "USA", "USA", "USA");
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

            FlightConnectionsDotComWorker_AirportCollector siteParser = new(worker);
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
