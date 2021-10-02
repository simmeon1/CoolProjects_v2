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
            chromeDriver = new(chromeOptions);
            logger = new();
        }

        [TestMethod]
        public async Task OpenFlights_ExpectedTabsOpenedWithNoErrors()
        {
            List<string> path1 = new() { "ABZ", "LTN", "VAR" };
            List<string> path2 = new() { "VAR", "LTN", "ABZ" };
            List<Path> paths = new() { new Path(path1), new Path(path2) };

            ChromeWorker chromeWorker = new(chromeDriver, chromeDriver, logger, new RealDelayer());
            List<KeyValuePair<Path, List<KeyValuePair<Path, FlightCollection>>>> results = await chromeWorker.ProcessPaths(paths, new DateTime(2022, 1, 13));
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results[0].Value[0].Value.Count() > 0);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            chromeDriver.Quit();
        }
    }
}
