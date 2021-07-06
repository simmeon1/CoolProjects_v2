using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests_IntegrationTests
{
    [TestClass]
    public class NavigationWorker_IntegrationTests
    {

        ChromeDriver chromeDriver;
        NavigationWorker navigationWorker;
        Delayer delayer;

        [TestInitialize]
        public void TestInitialize()
        {
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");
            chromeDriver = new(chromeOptions);
            delayer = new Delayer();
            navigationWorker = new(chromeDriver, delayer, new ClosePrivacyPopupCommands());
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
