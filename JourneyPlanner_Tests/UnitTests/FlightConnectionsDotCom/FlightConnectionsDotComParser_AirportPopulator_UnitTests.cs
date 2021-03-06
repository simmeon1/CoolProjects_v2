using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.AirportFilterers;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.FlightConnectionsDotCom;
using JourneyPlanner_ClassLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;

namespace JourneyPlanner_Tests.UnitTests.FlightConnectionsDotCom
{
    [TestClass]
    public class FlightConnectionsDotComParserAirportPopulatorUnitTests
    {
        Mock<IWebDriver> driverMock;
        Mock<ILogger> logger;
        Mock<IWebDriverWaitProvider> webDriverWait;
        FlightConnectionsDotComWorker worker;
        private Airport airport1;
        private Airport airport2;
        private Airport airport3;

        [TestInitialize]
        public void TestInitialize()
        {
            driverMock = new();
            logger = new();
            webDriverWait = new();
            worker = new(logger.Object, driverMock.Object, webDriverWait.Object);
            airport1 = new("ABZ", "Aberdeen", "United Kingdom", "Aberdeen Airport", "linkA");
            airport2 = new("SOF", "Sofia", "Bulgaria", "Sofia Airport", "linkB");
            airport3 = new("EDI", "Edinburgh", "United Kingdom", "Edinburgh Airport", "linkC");
        }

        [TestMethod]
        public void PopulateAirports_ReturnsExpectedResults()
        {
            AssertDefaultSuccessResults(GetResults());
        }
        
        [TestMethod]
        public void PopulateAirports_NoResultsDueToFiltering()
        {
            Mock<IAirportFilterer> filtererMock = new();
            filtererMock.Setup(x => x.AirportMeetsCondition(It.IsAny<Airport>())).Returns(false);
            Assert.IsTrue(GetResults(filterer: filtererMock.Object).Count == 0);
        }
        
        [TestMethod]
        public void PopulateAirports_SofiaIsFilteredOut()
        {
            Mock<IAirportFilterer> filtererMock = new();
            filtererMock.Setup(x => x.AirportMeetsCondition(It.IsAny<Airport>())).Returns(true);
            filtererMock.Setup(x => x.AirportMeetsCondition(airport2)).Returns(false);
            Dictionary<string, HashSet<string>> results = GetResults(filterer: filtererMock.Object);
            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results[airport1.Code].Count == 1);
            Assert.IsTrue(results[airport1.Code].Contains(airport3.Code));
            Assert.IsTrue(results[airport3.Code].Count == 1);
            Assert.IsTrue(results[airport3.Code].Contains(airport1.Code));
        }

        [TestMethod]
        public void PopulateAirports_ReturnsExpectedResults_ShowMoreButtonIsNull()
        {
            AssertDefaultSuccessResults(GetResults(showMoreButtonIsNull: true));
        }

        [TestMethod]
        public void PopulateAirports_ReturnsExpectedResults_ShowMoreButtonThrowsException()
        {
            AssertDefaultSuccessResults(GetResults(showMoreButtonThrowsException: true));
        }

        private void AssertDefaultSuccessResults(Dictionary<string, HashSet<string>> result)
        {
            Assert.IsTrue(result[airport1.Code].Count == 2);
            Assert.IsTrue(result[airport1.Code].Contains(airport2.Code));
            Assert.IsTrue(result[airport1.Code].Contains(airport3.Code));
            Assert.IsTrue(result[airport2.Code].Count == 1);
            Assert.IsTrue(result[airport2.Code].Contains(airport1.Code));
            Assert.IsTrue(result[airport3.Code].Count == 0);
        }

        [TestMethod]
        public void PopulateAirports_ReturnsExpectedResults_PopularDestinationsDivIsNull()
        {
            AssertDefaultFailResults(GetResults(popularDestinationsDivIsNull: true));
        }

        [TestMethod]
        public void PopulateAirports_ReturnsExpectedResults_PopularDestinationsThrowsException()
        {
            AssertDefaultFailResults(GetResults(popularDestinationsDivThrowsException: true));
        }

        private void AssertDefaultFailResults(Dictionary<string, HashSet<string>> result)
        {
            Assert.IsTrue(result[airport1.Code].Count == 0);
            Assert.IsTrue(result[airport2.Code].Count == 0);
            Assert.IsTrue(result[airport3.Code].Count == 0);
        }

        private Dictionary<string, HashSet<string>> GetResults(
            bool popularDestinationsDivIsNull = false,
            bool popularDestinationsDivThrowsException = false,
            bool showMoreButtonIsNull = false,
            bool showMoreButtonThrowsException = false,
            IAirportFilterer filterer = null
            )
        {
            List<Airport> airports = new() { airport1, airport2, airport3 };

            Mock<IWebElement> mockEntry1 = new();
            Mock<IWebElement> mockEntry2 = new();
            Mock<IWebElement> mockEntry3 = new();
            mockEntry1.Setup(x => x.GetAttribute("data-a")).Returns($"gg ({airport1.Code})");
            mockEntry2.Setup(x => x.GetAttribute("data-a")).Returns($"cc ({airport2.Code})");
            mockEntry3.Setup(x => x.GetAttribute("data-a")).Returns($"dd ({airport3.Code})");

            if (!showMoreButtonIsNull)
            {
                if (showMoreButtonThrowsException)
                {
                    driverMock.Setup(x => x.FindElement(By.CssSelector(".show-all-destinations-btn"))).Throws(new NoSuchElementException());
                }
                else driverMock.Setup(x => x.FindElement(By.CssSelector(".show-all-destinations-btn"))).Returns(new Mock<IWebElement>().Object);
            }

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
            if (!popularDestinationsDivIsNull)
            {
                if (popularDestinationsDivThrowsException)
                {
                    driverMock.Setup(x => x.FindElement(By.CssSelector("#popular-destinations"))).Throws(new NoSuchElementException());
                }
                else
                {
                    driverMock.SetupSequence(x => x.FindElement(By.CssSelector("#popular-destinations")))
                        .Returns(popularDestinationsDivMock1Object)
                        .Returns(popularDestinationsDivMock2Object)
                        .Returns(popularDestinationsDivMock3Object);
                }
            }

            FlightConnectionsDotComWorkerAirportPopulator siteParser = new(worker);
            Dictionary<string, HashSet<string>> result = siteParser.PopulateAirports(airports, filterer);
            return result;
        }
    }
}
