using FlightConnectionsDotCom_ClassLibrary.Interfaces;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    public class ChromeWorker
    {
        private IWebDriver Driver { get; set; }
        private IJavaScriptExecutorWithDelayer JSExecutorWithDelayer { get; set; }
        private INavigationWorker NavigationWorker { get; set; }
        private IDelayer Delayer { get; set; }
        private IWebElementWorker WebElementWorker { get; set; }
        private ILogger Logger { get; set; }

        public ChromeWorker(IWebDriver driver, IJavaScriptExecutorWithDelayer jSExecutorWithDelayer, INavigationWorker navigationWorker, IDelayer delayer, IWebElementWorker webElementWorker, ILogger logger)
        {
            Driver = driver;
            JSExecutorWithDelayer = jSExecutorWithDelayer;
            NavigationWorker = navigationWorker;
            Delayer = delayer;
            WebElementWorker = webElementWorker;
            Logger = logger;
        }

        public async Task OpenPaths(List<List<string>> paths, DateTime date)
        {
            foreach (List<string> path in paths)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    INavigation navigation = Driver.Navigate();
                    await NavigationWorker.GoToUrl(navigation, "https://www.google.com/travel/flights", openInNewTab: true);

                    ReadOnlyCollection<IWebElement> spans = (ReadOnlyCollection<IWebElement>)await JSExecutorWithDelayer.ExecuteScriptAndWait("return document.querySelectorAll('span');");
                    foreach (IWebElement span in spans)
                    {
                        string spanText = (string)await JSExecutorWithDelayer.ExecuteScriptAndWait("return arguments[0].textContent;", span);
                        if (!spanText.Equals("Round trip")) continue;
                        span.Click();
                        ReadOnlyCollection<IWebElement> lis = (ReadOnlyCollection<IWebElement>)await JSExecutorWithDelayer.ExecuteScriptAndWait("return document.querySelectorAll('li');");
                        foreach (IWebElement li in lis)
                        {
                            string liText = (string)await JSExecutorWithDelayer.ExecuteScriptAndWait("return arguments[0].textContent;", li);
                            if (!liText.Equals("One way")) continue;
                            li.Click();
                            break;
                        }
                        break;
                    }

                    ReadOnlyCollection<IWebElement> inputs = (ReadOnlyCollection<IWebElement>)await JSExecutorWithDelayer.ExecuteScriptAndWait("return document.querySelectorAll('input');");
                    IWebElement originInput1 = inputs[0];
                    IWebElement originInput2 = inputs[1];
                    IWebElement destinationInput1 = inputs[2];
                    IWebElement destinationInput2 = inputs[3];
                    IWebElement dateInput1 = inputs[4];
                    IWebElement dateInput2 = inputs[6];

                    string origin = path[i];
                    originInput1.Click();
                    originInput2.SendKeys(origin);
                    originInput2.SendKeys(Keys.Return);

                    string target = path[i + 1];
                    destinationInput1.Click();
                    destinationInput2.SendKeys(target);
                    destinationInput2.SendKeys(Keys.Return);

                    dateInput1.Click();
                    dateInput2.SendKeys(date.ToString("ddd, MMM dd"));
                    dateInput2.SendKeys(Keys.Return);

                    ReadOnlyCollection<IWebElement> buttons = (ReadOnlyCollection<IWebElement>)await JSExecutorWithDelayer.ExecuteScriptAndWait("return document.querySelectorAll('button');");
                    foreach (IWebElement button in buttons)
                    {
                        string buttonText = (string)await JSExecutorWithDelayer.ExecuteScriptAndWait("return arguments[0].getAttribute('aria-label');", button);
                        if (buttonText == null || !buttonText.Contains("Done. Search for")) continue;
                        button.Click();
                        break;
                    }
                }
            }
        }
    }
}