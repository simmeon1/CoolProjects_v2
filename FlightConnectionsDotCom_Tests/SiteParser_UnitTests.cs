using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FlightConnectionsDotCom_Tests
{
    [TestClass]
    public class SiteParser_UnitTests
    {
        [TestMethod]
        public void GetAirportsAndTheirConnections()
        {
            //Mock<IWebDriver> driverMock = new();
            //Mock<IJavaScriptExecutor> jsExecutorMock = new();
            //Mock<INavigationWorker> navigationWorkerMock = new();

            //SiteParser siteParser = new(driverMock.Object, jsExecutorMock.Object, navigationWorkerMock.Object);
            //Dictionary<string, List<string>> results = siteParser.GetAirportsAndTheirConnections();
            //Assert.IsTrue(results.Count == 2);
            //Assert.IsTrue(results["EDI"].Count == 1);
            //Assert.IsTrue(results["EDI"][0].Equals("SOF"));
            //Assert.IsTrue(results["SOF"].Count == 1);
            //Assert.IsTrue(results["SOF"][0].Equals("EDI"));
        }

        [TestMethod]
        public void CollectAirports_ReturnsExpectedAirports()
        {
            Mock<IWebDriver> driverMock = new();
            Mock<IJavaScriptExecutor> jsExecutorMock = new();
            Mock<INavigationWorker> navigationWorkerMock = new();
            Mock<IDelayer> delayerMock = new();
            Mock<IWebElementWorker> webElementWorker = new();

            CollectAirportCommands commands = new();
            Airport airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "linkA");
            Airport airport2 = new("SOF", "Sofia", "Bulgaria", "Sofia Airport", "linkB");
            Airport airport3 = new("EDI", "Edinburgh", "United Kingdom", "Edinburgh Airport", "linkC");
            Airport airport4 = new("CIA", "Rome", "Spain", "Rome Ciampino", "linkD");

            IWebElement airportListEntryObject1 = SetUpAirportListEntryData(jsExecutorMock, commands, airport1);
            IWebElement airportListEntryObject2 = SetUpAirportListEntryData(jsExecutorMock, commands, airport2);
            IWebElement airportListEntryObject3 = SetUpAirportListEntryData(jsExecutorMock, commands, airport3);
            IWebElement airportListEntryObject4 = SetUpAirportListEntryData(jsExecutorMock, commands, airport4);

            jsExecutorMock.Setup(x => x.ExecuteScript(commands.GetAirportListEntries)).Returns(
                new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { airportListEntryObject1, airportListEntryObject2, airportListEntryObject3, airportListEntryObject4 }) 
            );

            SiteParser siteParser = new(driverMock.Object, jsExecutorMock.Object, navigationWorkerMock.Object, delayerMock.Object, webElementWorker.Object);
            List<Airport> results = siteParser.CollectAirports(commands);
            Assert.IsTrue(results.Count == 4);
            Assert.IsTrue(results[0].Equals(airport1));
            Assert.IsTrue(results[3].Equals(airport2));
            Assert.IsTrue(results[2].Equals(airport3));
            Assert.IsTrue(results[1].Equals(airport4));
        }

        private static IWebElement SetUpAirportListEntryData(Mock<IJavaScriptExecutor> jsExecutorMock, CollectAirportCommands commands, Airport airport)
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
