using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class NavigationWorker : INavigationWorker
    {
        private IJavaScriptExecutorWithDelayer JSExecutorWithDelayer { get; set; }
        private IWebDriver WebDriver { get; set; }
        private ClosePrivacyPopupCommands ClosePrivacyPopupCommands { get; set; }
        public bool PolicyPopupHasBeenClosed { get; set; }

        public NavigationWorker(IJavaScriptExecutorWithDelayer jSExecutorWithDelayer, IWebDriver webDriver, ClosePrivacyPopupCommands closePrivacyPopupCommands)
        {
            JSExecutorWithDelayer = jSExecutorWithDelayer;
            WebDriver = webDriver;
            ClosePrivacyPopupCommands = closePrivacyPopupCommands;
        }

        public async Task GoToUrl(INavigation navigation, string path)
        {
            navigation.GoToUrl(path);
            await JSExecutorWithDelayer.GetDelayer().Delay(500);

            try
            {
                await JSExecutorWithDelayer.ExecuteScriptAndWait("return true");
            }
            catch (UnhandledAlertException)
            {
                await JSExecutorWithDelayer.GetDelayer().Delay(1000);
            }

            if (PolicyPopupHasBeenClosed) return;
            object scriptResult = await JSExecutorWithDelayer.ExecuteScriptAndWait(ClosePrivacyPopupCommands.GetAllButtonsOnPage);
            if (scriptResult is not ReadOnlyCollection<IWebElement>) return;

            ReadOnlyCollection<IWebElement> buttons = (ReadOnlyCollection<IWebElement>)scriptResult;
            foreach (IWebElement button in buttons)
            {
                string buttonText = (string)await JSExecutorWithDelayer.ExecuteScriptAndWait(ClosePrivacyPopupCommands.GetButtonText, button);
                if (buttonText.Contains("AGREE"))
                {
                    button.Click();
                    PolicyPopupHasBeenClosed = true;
                    return;
                }
            }
        }
    }
}
