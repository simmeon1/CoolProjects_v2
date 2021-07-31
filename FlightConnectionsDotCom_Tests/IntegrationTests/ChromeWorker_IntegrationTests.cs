using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;

namespace FlightConnectionsDotCom_Tests.IntegrationTests
{
    [TestClass]
    public class ChromeWorker_IntegrationTests
    {

        ChromeDriver chromeDriver;
        Logger_Debug logger;

        [TestInitialize]
        public void TestInitialize()
        {
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");
            chromeDriver = new(chromeOptions);
            logger = new();
        }

        [TestMethod]
        public void OpenFlights_ExpectedTabsOpenedWithNoErrors()
        {
            List<string> path1 = new() { "ABZ", "SOF" };
            List<string> path2 = new() { "ABZ", "EDI", "SOF" };
            List<string> path3 = new() { "ABZ", "EDI", "CIA", "SOF" };
            List<List<string>> paths = new() { path1, path2, path3 };

            ChromeWorker chromeWorker = new(chromeDriver, chromeDriver, logger);
            int results = chromeWorker.OpenPaths(paths, DateTime.Today);
            Assert.IsTrue(results == 6);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            chromeDriver.Quit();
        }
    }
}
