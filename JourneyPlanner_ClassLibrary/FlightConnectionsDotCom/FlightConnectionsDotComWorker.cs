using Common_ClassLibrary;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace JourneyPlanner_ClassLibrary
{
    public class FlightConnectionsDotComWorker
    {
        public IWebDriver Driver { get; set; }
        public ILogger Logger { get; set; }
        public IWebDriverWaitProvider WebDriverWait { get; set; }

        public FlightConnectionsDotComWorker(ILogger logger, IWebDriver driver, IWebDriverWaitProvider webDriverWait)
        {
            Driver = driver;
            Logger = logger;
            WebDriverWait = webDriverWait;
        }

        public void GoToUrl(INavigation navigation, string link)
        {
            if (navigation == null) return;
            navigation.GoToUrl(link);

            try
            {
                IAlert result = WebDriverWait.WaitUntilAlertIsPresent();
                result.Accept();
            }
            catch (WebDriverTimeoutException)
            {
                //Do nothing
            }

            ReadOnlyCollection<IWebElement> buttons = Driver.FindElements(By.CssSelector("button"));
            foreach (IWebElement button in buttons)
            {
                string buttonText = button.Text;
                if (buttonText.Contains("AGREE"))
                {
                    button.Click();
                    break;
                }
            }
        }
    }
}