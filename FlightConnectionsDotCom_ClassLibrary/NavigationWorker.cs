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
        public bool PolicyPopupHasBeenClosed { get; set; }
        private IJavaScriptExecutorWithDelayer JSExecutorWithDelayer { get; set; }
        private ClosePrivacyPopupCommands ClosePrivacyPopupCommands { get; set; }

        public NavigationWorker(IJavaScriptExecutorWithDelayer jSExecutorWithDelayer, ClosePrivacyPopupCommands closePrivacyPopupCommands)
        {
            JSExecutorWithDelayer = jSExecutorWithDelayer;
            ClosePrivacyPopupCommands = closePrivacyPopupCommands;
        }

        public async Task GoToUrl(INavigation navigation, string path)
        {
            navigation.GoToUrl(path);
            if (PolicyPopupHasBeenClosed) return;

            await JSExecutorWithDelayer.GetDelayer().Delay(1000);
            object scriptResult;
            try
            {
                scriptResult = await JSExecutorWithDelayer.ExecuteScriptAndWait(script: ClosePrivacyPopupCommands.GetAllButtonsOnPage);
            }
            catch (UnhandledAlertException ex)
            {
                throw;
            }
            scriptResult = await JSExecutorWithDelayer.ExecuteScriptAndWait(ClosePrivacyPopupCommands.GetAllButtonsOnPage);
            if (scriptResult is not ReadOnlyCollection<IWebElement>) return;

            ReadOnlyCollection<IWebElement> buttons = (ReadOnlyCollection<IWebElement>)scriptResult;
            foreach (IWebElement button in buttons)
            {
                string buttonText = (string) await JSExecutorWithDelayer.ExecuteScriptAndWait(ClosePrivacyPopupCommands.GetButtonText, button);
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
