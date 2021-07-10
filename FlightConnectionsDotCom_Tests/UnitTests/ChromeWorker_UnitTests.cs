using FlightConnectionsDotCom_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    public class ChromeWorker_UnitTests
    {
        [TestClass]
        public class FlightConnectionsDotComParser_UnitTests
        {
            Mock<IWebDriver> driverMock;
            Mock<IJavaScriptExecutorWithDelayer> jsExecutorWithDelayerMock;
            Mock<INavigationWorker> navigationWorkerMock;
            Mock<IDelayer> delayerMock;
            Mock<IWebElementWorker> webElementWorker;
            Mock<ILogger> logger;

            private void InitialiseMockObjects()
            {
                driverMock = new();
                jsExecutorWithDelayerMock = new();
                navigationWorkerMock = new();
                delayerMock = new();
                webElementWorker = new();
                logger = new();
            }

            [TestMethod]
            public async Task OpenLinks()
            {
                //List<string> path1 = new() { "ABZ", "SOF" };
                //List<string> path2 = new() { "ABZ", "EDI", "SOF" };
                //List<List<string>> paths = new List<List<string>>() { path1, path2 };
                //ChromeWorker chromeWorker = new();
                //chromeWorker.OpenPaths(paths);
                //Assert.IsTrue(true);
            }
        }
    }
}
