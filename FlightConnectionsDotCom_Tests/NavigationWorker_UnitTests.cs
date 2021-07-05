using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests
{
    [TestClass]
    public class NavigationWorker_UnitTests
    {



        [TestMethod]
        public async Task ClosePrivacyPopup_CorrectlyClosedAsync()
        {
            Mock<IWebDriver> driverMock = new();
            Mock<IJavaScriptExecutor> jsExecutorMock = new();
            Mock<INavigationWorker> navigationWorkerMock = new();
            Mock<IDelayer> delayerMock = new();
            Mock<IWebElementWorker> webElementWorker = new();
            ClosePrivacyPopupCommands closePrivacyPopupCommands = new();
            NavigationWorker navigationWorker = new(jsExecutorMock.Object, delayerMock.Object, closePrivacyPopupCommands);

            INavigation navigationMock = new Mock<INavigation>().Object;
            IWebElement buttonMock = new Mock<IWebElement>().Object;
            ReadOnlyCollection<IWebElement> buttonsMock = new(new List<IWebElement>() { buttonMock });
            jsExecutorMock.Setup(x => x.ExecuteScript(closePrivacyPopupCommands.GetAllButtonsOnPage)).Returns(buttonsMock);
            jsExecutorMock.Setup(x => x.ExecuteScript(closePrivacyPopupCommands.GetButtonText, buttonMock)).Returns("AGREE");
            await navigationWorker.GoToUrl(navigationMock, "test");
            Assert.IsTrue(navigationWorker.PolicyPopupHasBeenClosed);
        }
    }
}
