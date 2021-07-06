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
        private IJavaScriptExecutor JSExecutor { get; set; }
        private IDelayer Delayer { get; set; }
        private ClosePrivacyPopupCommands ClosePrivacyPopupCommands { get; set; }

        public NavigationWorker(IJavaScriptExecutor jSExecutor, IDelayer delayer, ClosePrivacyPopupCommands closePrivacyPopupCommands)
        {
            JSExecutor = jSExecutor;
            Delayer = delayer;
            ClosePrivacyPopupCommands = closePrivacyPopupCommands;
        }

        public async Task GoToUrl(INavigation navigation, string path)
        {
            navigation.GoToUrl(path);
            if (PolicyPopupHasBeenClosed) return;

            await Delayer.Delay(1000);
            object scriptResult = JSExecutor.ExecuteScript(ClosePrivacyPopupCommands.GetAllButtonsOnPage);
            if (scriptResult is not ReadOnlyCollection<IWebElement>) return;

            ReadOnlyCollection<IWebElement> buttons = (ReadOnlyCollection<IWebElement>)scriptResult;
            foreach (IWebElement button in buttons)
            {
                string buttonText = (string)JSExecutor.ExecuteScript(ClosePrivacyPopupCommands.GetButtonText, button);
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
