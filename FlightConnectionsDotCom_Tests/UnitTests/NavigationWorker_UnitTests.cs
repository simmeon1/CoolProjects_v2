using FlightConnectionsDotCom_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class NavigationWorker_UnitTests
    {
        Mock<IJavaScriptExecutorWithDelayer> jsExecutorWithDelayerMock;
        Mock<IWebDriver> webDriverMock;
        IDelayer delayerMock;
        INavigation navigationMock;
        IWebElement buttonMock;
        ClosePrivacyPopupCommands closePrivacyPopupCommands;
        private void InitialiseMocks()
        {
            jsExecutorWithDelayerMock = new();
            webDriverMock = new();
            delayerMock = new Mock<IDelayer>().Object;
            navigationMock = new Mock<INavigation>().Object;
            buttonMock = new Mock<IWebElement>().Object;
            closePrivacyPopupCommands = new();
            jsExecutorWithDelayerMock.Setup(x => x.GetDelayer()).Returns(delayerMock);
        }

        [TestMethod]
        public async Task ClosePrivacyPopup_CorrectlyClosed()
        {
            InitialiseMocks();
            ReadOnlyCollection<IWebElement> buttonsMock = new(new List<IWebElement>() { buttonMock });
            jsExecutorWithDelayerMock.Setup(x => x.ExecuteScriptAndWait(closePrivacyPopupCommands.GetAllButtonsOnPage).Result).Returns(buttonsMock);
            jsExecutorWithDelayerMock.Setup(x => x.ExecuteScriptAndWait(closePrivacyPopupCommands.GetButtonText, buttonMock).Result).Returns("AGREE");

            NavigationWorker navigationWorker = new(jsExecutorWithDelayerMock.Object, webDriverMock.Object, closePrivacyPopupCommands);
            await navigationWorker.GoToUrl(navigationMock, "test");
            Assert.IsTrue(navigationWorker.PolicyPopupHasBeenClosed);
        }

        [TestMethod]
        public async Task UnexpectedAlertIsHandled()
        {
            InitialiseMocks();
            jsExecutorWithDelayerMock.Setup(x => x.ExecuteScriptAndWait("return true")).Throws(new UnhandledAlertException());
            jsExecutorWithDelayerMock.Setup(x => x.ExecuteScriptAndWait(closePrivacyPopupCommands.GetAllButtonsOnPage).Result).Returns(new List<string>());

            IAlert alertMock = new Mock<IAlert>().Object;
            Mock<ITargetLocator> targetLocatorMock = new();
            targetLocatorMock.Setup(x => x.Alert()).Returns(alertMock);
            webDriverMock.Setup(x => x.SwitchTo()).Returns(targetLocatorMock.Object);

            NavigationWorker navigationWorker = new(jsExecutorWithDelayerMock.Object, webDriverMock.Object, closePrivacyPopupCommands);
            await navigationWorker.GoToUrl(navigationMock, "test");
            Assert.IsTrue(true);
        }
    }
}
