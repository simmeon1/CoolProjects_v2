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
        public void OpenFlights_ExpectedTabsOpenedWithNoErrors()
        {
            InitialiseMockObjects();
            List<string> path1 = new() { "ABZ", "SOF" };
            List<List<string>> paths = new() { path1 };

            driverMock.Setup(x => x.WindowHandles).Returns(new ReadOnlyCollection<string>(new List<string>() { }));
            driverMock.Setup(x => x.FindElement(By.CssSelector("header"))).Returns(new Mock<IWebElement>().Object);

            Mock<IWebElement> consentButton = new();
            consentButton.Setup(x => x.Text).Returns("I agree");

            Mock<IWebElement> searchButton = new();
            searchButton.Setup(x => x.GetAttribute("aria-label")).Returns("Done. Search for");
            
            Mock<IWebElement> stopsButton = new();
            stopsButton.Setup(x => x.GetAttribute("aria-label")).Returns("Stops");

            ReadOnlyCollection<IWebElement> radioGroupChildren = new(new List<IWebElement>() { new Mock<IWebElement>().Object, new Mock<IWebElement>().Object });
            Mock<IWebElement> radioGroup = new();
            radioGroup.Setup(x => x.FindElements(By.CssSelector("input"))).Returns(radioGroupChildren);
            driverMock.Setup(x => x.FindElement(By.CssSelector("[role=radiogroup]"))).Returns(radioGroup.Object);

            driverMock.SetupSequence(x => x.FindElements(By.CssSelector("button")))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { consentButton.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { searchButton.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { stopsButton.Object }));

            Mock<IWebElement> roundTripSpan = new();
            roundTripSpan.Setup(x => x.Text).Returns("Round trip");
            driverMock.Setup(x => x.FindElements(By.CssSelector("span"))).Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { roundTripSpan.Object }));

            Mock<IWebElement> oneWayLi = new();
            oneWayLi.Setup(x => x.Text).Returns("One way");
            driverMock.Setup(x => x.FindElements(By.CssSelector("li"))).Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { oneWayLi.Object }));

            ReadOnlyCollection<IWebElement> inputs = new(new List<IWebElement>() {
                    new Mock<IWebElement>().Object, new Mock<IWebElement>().Object, new Mock<IWebElement>().Object,
                    new Mock<IWebElement>().Object, new Mock<IWebElement>().Object, new Mock<IWebElement>().Object,
                    new Mock<IWebElement>().Object });
            driverMock.Setup(x => x.FindElements(By.CssSelector("input"))).Returns(inputs);

            ChromeWorker chromeWorker = new(driverMock.Object, jsExecutor.Object, logger.Object);
            int results = chromeWorker.OpenPaths(paths, new DateTime(2000, 10, 10));
            Assert.IsTrue(results == 0);
        }
    }
}
