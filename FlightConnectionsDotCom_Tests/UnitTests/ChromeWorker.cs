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
        private IJavaScriptExecutor JSExecutor { get; set; }
        private ILogger Logger { get; set; }

        public ChromeWorker(IWebDriver driver, IJavaScriptExecutor jSExecutor)
        {
            Driver = driver;
            JSExecutor = jSExecutor;
        }

        public int OpenPaths(List<List<string>> paths, DateTime date)
        {
            int initialTabCount = Driver.WindowHandles.Count;
            bool consentAgreed = false;
            foreach (List<string> path in paths)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    JSExecutor.ExecuteScript("window.open();");
                    ITargetLocator targetLocator = Driver.SwitchTo();
                    targetLocator.Window(Driver.WindowHandles[Driver.WindowHandles.Count - 1]);

                    INavigation navigation = Driver.Navigate();
                    navigation.GoToUrl("https://www.google.com/travel/flights");

                    if (!consentAgreed)
                    {
                        ReadOnlyCollection<IWebElement> consentButtons = Driver.FindElements(By.CssSelector("button"));
                        foreach (IWebElement button in consentButtons)
                        {
                            string buttonText = button.Text;
                            if (!buttonText.Contains("I agree")) continue;
                            button.Click();
                            consentAgreed = true;
                            break;
                        }
                    }

                    ReadOnlyCollection<IWebElement> spans = Driver.FindElements(By.CssSelector("span"));
                    foreach (IWebElement span in spans)
                    {
                        string spanText = span.Text;
                        if (!spanText.Equals("Round trip")) continue;
                        span.Click();
                        ReadOnlyCollection<IWebElement> lis = Driver.FindElements(By.CssSelector("li"));
                        foreach (IWebElement li in lis)
                        {
                            string liText = li.Text;
                            if (!liText.Equals("One way")) continue;
                            li.Click();
                            break;
                        }
                        break;
                    }

                    ReadOnlyCollection<IWebElement> inputs = Driver.FindElements(By.CssSelector("input"));
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

                    ReadOnlyCollection<IWebElement> doneButtons = Driver.FindElements(By.CssSelector("button"));
                    foreach (IWebElement button in doneButtons)
                    {
                        string buttonText = button.GetAttribute("aria-label");
                        if (buttonText == null || !buttonText.Contains("Done. Search for")) continue;
                        button.Click();
                        break;
                    }
                }
            }
            return Driver.WindowHandles.Count - initialTabCount;
        }
    }
}