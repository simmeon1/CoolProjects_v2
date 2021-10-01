using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class ChromeWorker_UnitTests
    {
        Mock<IWebDriver> driverMock;
        Mock<ILogger> logger;
        Mock<IJavaScriptExecutor> jsExecutor;

        private void InitialiseMockObjects()
        {
            driverMock = new();
            jsExecutor = new();
            logger = new();
        }

        [TestMethod]
        public async Task OpenFlights_ExpectedTabsOpenedWithNoErrors_1()
        {
            await RunTest();
        }

        [TestMethod]
        public async Task OpenFlights_ExpectedTabsOpenedWithNoErrors_2()
        {
            await RunTest(targetLocatorReturnsNull: true, noConsentButtons: true, noSpansForTripType: true, searchingForButtonsReturnsEmptyList: true, stopButtonContainsNullText: true, hasNoFlightList: true);
        }

        [TestMethod]
        public async Task OpenFlights_ExpectedTabsOpenedWithNoErrors_3()
        {
            await RunTest(noOptionForOneWayFlight: true, flightListThrowsException: true);
        }
        
        [TestMethod]
        public async Task OpenFlights_ExpectedTabsOpenedWithNoErrors_4()
        {
            await RunTest(flightsAreNull: true);
        }

        private async Task RunTest(
            bool targetLocatorReturnsNull = false,
            bool noConsentButtons = false,
            bool navigatonIsNull = false,
            bool noSpansForTripType = false,
            bool noOptionForOneWayFlight = false,
            bool searchingForButtonsReturnsEmptyList = false,
            bool stopButtonContainsNullText = false,
            bool hasNoFlightList = false,
            bool flightListThrowsException = false,
            bool flightsAreNull = false)
        {
            InitialiseMockObjects();
            List<string> path1 = new() { "ABZ", "SOF" };
            List<Path> paths = new() { new Path(path1) };

            driverMock.Setup(x => x.Navigate()).Returns(navigatonIsNull ? null : new Mock<INavigation>().Object);
            driverMock.Setup(x => x.SwitchTo()).Returns(targetLocatorReturnsNull ? null : new Mock<ITargetLocator>().Object);
            driverMock.Setup(x => x.WindowHandles).Returns(new ReadOnlyCollection<string>(new List<string>() { "1", "1", "1" }));
            driverMock.Setup(x => x.FindElement(By.CssSelector("header"))).Returns(new Mock<IWebElement>().Object);

            if (!hasNoFlightList)
            {
                if (flightListThrowsException)
                {
                    driverMock.Setup(x => x.FindElement(By.CssSelector("[role=list]"))).Throws(new NoSuchElementException());
                }
                else
                {
                    Mock<IWebElement> flightListMock = new();
                    Mock<IWebElement> flightMock = new();

                    string flightText = "9:45 PM\n– \n2:55 AM+1\nWizz Air\n3 hr 10 min\nLTN–VAR\nNonstop\n£27";
                    flightMock.SetupSequence(x => x.GetAttribute("innerText"))
                        .Returns("more flights")
                        .Returns(flightText)
                        .Returns(flightText);
                    flightListMock.Setup(x => x.FindElements(By.CssSelector("[role=listitem]"))).Returns(flightsAreNull ? null : new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightMock.Object }));
                    driverMock.Setup(x => x.FindElement(By.CssSelector("[role=list]"))).Returns(flightListMock.Object);
                }
            }

            Mock<IWebElement> consentButton = new();
            consentButton.Setup(x => x.Text).Returns("I agree");

            Mock<IWebElement> searchButton = new();
            searchButton.Setup(x => x.GetAttribute("aria-label")).Returns("Done. Search for");

            Mock<IWebElement> stopsButton = new();
            stopsButton.Setup(x => x.GetAttribute("aria-label")).Returns(stopButtonContainsNullText ? null : "Stops");

            ReadOnlyCollection<IWebElement> radioGroupChildren = new(new List<IWebElement>() { new Mock<IWebElement>().Object, new Mock<IWebElement>().Object });
            Mock<IWebElement> radioGroup = new();
            radioGroup.Setup(x => x.FindElements(By.CssSelector("input"))).Returns(radioGroupChildren);
            driverMock.Setup(x => x.FindElement(By.CssSelector("[role=radiogroup]"))).Returns(radioGroup.Object);

            driverMock.SetupSequence(x => x.FindElements(By.CssSelector("button")))
                .Returns(new ReadOnlyCollection<IWebElement>(noConsentButtons ? new List<IWebElement>() : new List<IWebElement>() { consentButton.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(searchingForButtonsReturnsEmptyList ? new List<IWebElement>() : new List<IWebElement>() { searchButton.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { stopsButton.Object }));

            Mock<IWebElement> roundTripSpan = new();
            roundTripSpan.Setup(x => x.Text).Returns("Round trip");
            driverMock.Setup(x => x.FindElements(By.CssSelector("span")))
                .Returns(noSpansForTripType ? new ReadOnlyCollection<IWebElement>(new List<IWebElement>()) : new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { roundTripSpan.Object }));

            Mock<IWebElement> oneWayLi = new();
            oneWayLi.Setup(x => x.Text).Returns("One way");
            driverMock.Setup(x => x.FindElements(By.CssSelector("li"))).Returns(noOptionForOneWayFlight ? new ReadOnlyCollection<IWebElement>(new List<IWebElement>()) : new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { oneWayLi.Object }));

            ReadOnlyCollection<IWebElement> inputs = new(new List<IWebElement>() {
                    new Mock<IWebElement>().Object, new Mock<IWebElement>().Object, new Mock<IWebElement>().Object,
                    new Mock<IWebElement>().Object, new Mock<IWebElement>().Object, new Mock<IWebElement>().Object,
                    new Mock<IWebElement>().Object });
            driverMock.Setup(x => x.FindElements(By.CssSelector("input"))).Returns(inputs);

            ChromeWorker chromeWorker = new(driverMock.Object, jsExecutor.Object, logger.Object, new Mock<IDelayer>().Object);
            List<KeyValuePair<Path, List<KeyValuePair<Path, List<Flight>>>>> results = await chromeWorker.ProcessPaths(paths, new DateTime(2000, 10, 10));

            AssertCommonExpectation(results);
            if (!hasNoFlightList && !flightListThrowsException && !flightsAreNull)
            {
                Assert.IsTrue(results[0].Value[0].Value.Count == 1);
                Assert.IsTrue(results[0].Value[0].Value[0].ToString().Equals(@"LTN–VAR - 10/10/2000 21:45:00 - 11/10/2000 02:55:00 - 27"));
            }
            else Assert.IsTrue(results[0].Value[0].Value.Count == 0);
        }

        private static void AssertCommonExpectation(List<KeyValuePair<Path, List<KeyValuePair<Path, List<Flight>>>>> results)
        {
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0].Key.ToString().Equals("ABZ-SOF"));
            Assert.IsTrue(results[0].Value.Count == 1);
            Assert.IsTrue(results[0].Value[0].Key.ToString().Equals("ABZ-SOF"));
        }
    }
}
