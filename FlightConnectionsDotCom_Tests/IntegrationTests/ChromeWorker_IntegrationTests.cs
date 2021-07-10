using FlightConnectionsDotCom_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary.Interfaces;
using FlightConnectionsDotCom_Tests.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests.IntegrationTests
{
    [TestClass]
    public class ChromeWorker_IntegrationTests
    {

        ChromeDriver chromeDriver;
        NavigationWorker navigationWorker;
        Delayer delayer;
        WebElementWorker webElementWorker;
        CollectAirportCommands collectAirportCommands;
        GetAirportsAndTheirConnectionsCommands getAirportsAndTheirConnectionsCommands;
        FlightConnectionsDotComParser siteParser;
        Logger_Debug logger;
        JavaScriptExecutorWithDelayer jsExecutorWithDelay;

        [TestInitialize]
        public void TestInitialize()
        {
            ChromeOptions chromeOptions = new();
            //chromeOptions.AddArgument("headless");
            chromeDriver = new(chromeOptions);
            delayer = new();
            webElementWorker = new();
            collectAirportCommands = new();
            getAirportsAndTheirConnectionsCommands = new();
            logger = new();
            jsExecutorWithDelay = new(chromeDriver, delayer, 10);
            navigationWorker = new(jsExecutorWithDelay, chromeDriver, new ClosePrivacyPopupCommands());
            siteParser = new(chromeDriver, jsExecutorWithDelay, navigationWorker, delayer, webElementWorker, logger);
        }

        [TestMethod]
        public async Task OpenFlights()
        {
            List<string> path1 = new() { "ABZ", "SOF" };
            List<string> path2 = new() { "ABZ", "EDI", "SOF" };
            List<List<string>> paths = new() { path1, path2 };

            ChromeWorker chromeWorker = new(chromeDriver, jsExecutorWithDelay);
            await chromeWorker.OpenPaths(paths, DateTime.Today);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            chromeDriver.Quit();
        }
    }
}
