using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Linq;

namespace FlightConnectionsDotCom_Tests
{
    [TestClass]
    public class SiteParser_IntegrationTests
    {

        ChromeDriver chromeDriver;
        NavigationWorker navigationWorker;
        Delayer delayer;
        WebElementWorker webElementWorker;
        CollectAirportCommands collectAirportCommand;

        [TestInitialize]
        public void TestInitialize()
        {
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");
            chromeDriver = new(chromeOptions);
            navigationWorker = new();
            delayer = new();
            webElementWorker = new();
            collectAirportCommand = new();
        }


        [TestMethod]
        public void CollectAirports_ReturnsValues()
        {
            SiteParser siteParser = new(chromeDriver, chromeDriver, navigationWorker, delayer, webElementWorker);
            List<Airport> results = siteParser.CollectAirports(collectAirportCommand);
            Assert.IsTrue(results.Count > 0);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            chromeDriver.Quit();
        }
    }
}
