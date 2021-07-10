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

        public ChromeWorker(IWebDriver driver, IJavaScriptExecutorWithDelayer jSExecutorWithDelayer)
        {
            Driver = driver;
            JSExecutorWithDelayer = jSExecutorWithDelayer;
            Delayer = jSExecutorWithDelayer.GetDelayer();
        }

        public async Task OpenPaths(List<List<string>> paths, DateTime date)
        {
            foreach (List<string> path in paths)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    await JSExecutorWithDelayer.RunScript("window.open();");
                    ITargetLocator targetLocator = Driver.SwitchTo();
                    targetLocator.Window(Driver.WindowHandles[Driver.WindowHandles.Count - 1]);

                    INavigation navigation = Driver.Navigate();
                    navigation.GoToUrl("https://www.google.com/travel/flights");
                    await Delayer.Delay(1000);

                    ReadOnlyCollection<IWebElement> consentButtons = await JSExecutorWithDelayer.RunScriptAndGetElements("return document.querySelectorAll('button');");
                    foreach (IWebElement button in consentButtons)
                    {
                        string buttonText = await JSExecutorWithDelayer.RunScriptAndGetString("return arguments[0].textContent;", button);
                        if (!buttonText.Contains("I agree")) continue;
                        button.Click();
                        break;
                    }


                    ReadOnlyCollection<IWebElement> spans = await JSExecutorWithDelayer.RunScriptAndGetElements("return document.querySelectorAll('span');");
                    foreach (IWebElement span in spans)
                    {
                        string spanText = await JSExecutorWithDelayer.RunScriptAndGetString("return arguments[0].textContent;", span);
                        if (!spanText.Equals("Round trip")) continue;
                        span.Click();
                        ReadOnlyCollection<IWebElement> lis = await JSExecutorWithDelayer.RunScriptAndGetElements("return document.querySelectorAll('li');");
                        foreach (IWebElement li in lis)
                        {
                            string liText = await JSExecutorWithDelayer.RunScriptAndGetString("return arguments[0].textContent;", li);
                            if (!liText.Equals("One way")) continue;
                            li.Click();
                            break;
                        }
                        break;
                    }

                    ReadOnlyCollection<IWebElement> inputs = await JSExecutorWithDelayer.RunScriptAndGetElements("return document.querySelectorAll('input');");
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

                    ReadOnlyCollection<IWebElement> doneButtons = await JSExecutorWithDelayer.RunScriptAndGetElements("return document.querySelectorAll('button');");
                    foreach (IWebElement button in doneButtons)
                    {
                        string buttonText = await JSExecutorWithDelayer.RunScriptAndGetString("return arguments[0].getAttribute('aria-label');", button);
                        if (buttonText == null || !buttonText.Contains("Done. Search for")) continue;
                        button.Click();
                        break;
                    }
                }
            }
        }
    }
}