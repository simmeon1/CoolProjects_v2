using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            //chromeOptions.AddArgument("disable-gpu");
            chromeOptions.AddArgument("window-size=1280,800");
            //chromeOptions.AddArgument("allow-insecure-localhost");
            chromeDriver = new(chromeOptions);
            logger = new();
        }

        [TestMethod]
        public async Task OpenFlights_ExpectedTabsOpenedWithNoErrors()
        {
            List<string> path1 = new() { "ABZ", "LTN", "VAR" };
            List<string> path2 = new() { "EDI", "SOF" };
            List<string> path3 = new() { "VAR", "LTN", "ABZ" };
            List<Path> paths = new() { new Path(path1), new Path(path2), new Path(path3) };

            ChromeWorker chromeWorker = new(logger, new RealDelayer(), chromeDriver);
            ChromeWorkerResults results = await chromeWorker.ProcessPaths(paths, new DateTime(2022, 1, 13), new DateTime(2022, 1, 14));
            Assert.IsTrue(results.PathsAndFlights.Count > 0);
            Assert.IsTrue(results.FullPathsAndFlightCollections.Count > 0);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            chromeDriver.Quit();
        }
    }
}
