using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests.IntegrationTests
{
    [TestClass]
    public class NavigationWorker_IntegrationTests
    {

        ChromeDriver chromeDriver;
        NavigationWorker navigationWorker;
        Delayer delayer;
        JavaScriptExecutorWithDelayer javaScriptExecutorWithDelayer;

        [TestInitialize]
        public void TestInitialize()
        {
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");
            chromeDriver = new(chromeOptions);
            delayer = new Delayer();
            javaScriptExecutorWithDelayer = new JavaScriptExecutorWithDelayer(chromeDriver, delayer, 10);
            navigationWorker = new(javaScriptExecutorWithDelayer, chromeDriver, new ClosePrivacyPopupCommands());
        }

        [TestMethod]
        public async Task UnexpectedAlertIsHandled()
        {
            //Dismiss privacy message
            INavigation navigation = chromeDriver.Navigate();
            await navigationWorker.GoToUrl(navigation, "https://www.flightconnections.com/");

            INavigation navigation2 = chromeDriver.Navigate();
            await navigationWorker.GoToUrl(navigation2, "https://www.flightconnections.com/flights-to-qurghonteppa-kqt");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task ClosePrivacyPopup_NoExceptions()
        {
            INavigation navigation = chromeDriver.Navigate();
            await navigationWorker.GoToUrl(navigation, "https://www.flightconnections.com/");
            Assert.IsTrue(navigationWorker.PolicyPopupHasBeenClosed);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            chromeDriver.Quit();
        }
    }
}
