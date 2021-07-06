using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests_UnitTests
{
    [TestClass]
    public class NavigationWorker_UnitTests
    {
        [TestMethod]
        public async Task ClosePrivacyPopup_CorrectlyClosed()
        {
            Mock<IJavaScriptExecutorWithDelayer> jsExecutorWithDelayerMock = new();
            IDelayer delayerMock = new Mock<IDelayer>().Object;
            INavigation navigationMock = new Mock<INavigation>().Object;
            IWebElement buttonMock = new Mock<IWebElement>().Object;
            ClosePrivacyPopupCommands closePrivacyPopupCommands = new();

            ReadOnlyCollection<IWebElement> buttonsMock = new(new List<IWebElement>() { buttonMock });
            jsExecutorWithDelayerMock.Setup(x => x.GetDelayer()).Returns(delayerMock);
            jsExecutorWithDelayerMock.Setup(x => x.ExecuteScriptAndWait(closePrivacyPopupCommands.GetAllButtonsOnPage).Result).Returns(buttonsMock);
            jsExecutorWithDelayerMock.Setup(x => x.ExecuteScriptAndWait(closePrivacyPopupCommands.GetButtonText, buttonMock).Result).Returns("AGREE");

            NavigationWorker navigationWorker = new(jsExecutorWithDelayerMock.Object, closePrivacyPopupCommands);
            await navigationWorker.GoToUrl(navigationMock, "test");
            Assert.IsTrue(navigationWorker.PolicyPopupHasBeenClosed);
        }
    }
}
