using FlightConnectionsDotCom_ClassLibrary.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class ChromeWorker
    {
        private IWebDriver Driver { get; set; }
        private IJavaScriptExecutor JSExecutor { get; set; }
        private ILogger Logger { get; set; }
        private DateTime Date { get; set; }
        private bool ConsentAgreed { get; set; }
        private int PagesToOpen { get; set; }
        private int PagesOpened { get; set; }

        public ChromeWorker(IWebDriver driver, IJavaScriptExecutor jSExecutor, ILogger logger)
        {
            Driver = driver;
            JSExecutor = jSExecutor;
            Logger = logger;
        }

        public int OpenPaths(List<List<string>> paths, DateTime date)
        {
            Date = date;
            int initialTabCount = Driver.WindowHandles.Count;

            PagesToOpen = 0;
            PagesOpened = 0;
            foreach (List<string> path in paths) for (int i = 0; i < path.Count - 1; i++) PagesToOpen++;
            foreach (List<string> path in paths) ProcessPath(path);
            return Driver.WindowHandles.Count - initialTabCount;
        }

        private void ProcessPath(List<string> path)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                string origin = path[i];
                string target = path[i + 1];
                OpenNewTab();
                NavigateToUrl();
                AgreeToConsentIfItShows();
                SetToOneWayTrip();
                PopulateControls(origin, target);
                ClickDoneButton();
                PagesOpened++;
                Logger.Log($"Populated page for {origin} to {target} ({GetPercentageAndCountString()})");
            }
        }

        private string GetPercentageAndCountString()
        {
            string percentageString = $"{((double)PagesOpened / (double)PagesToOpen) * 100}%";
            Match match = Regex.Match(percentageString, @"(.*?\.\d\d).*%");
            if (match.Success) percentageString = $"{match.Groups[1].Value}%";
            return $"{PagesOpened}/{PagesToOpen} ({percentageString})";
        }

        private void OpenNewTab()
        {
            JSExecutor.ExecuteScript("window.open();");
            ITargetLocator targetLocator = Driver.SwitchTo();
            if (targetLocator != null) targetLocator.Window(Driver.WindowHandles[Driver.WindowHandles.Count - 1]);
        }

        private void NavigateToUrl()
        {
            INavigation navigation = Driver.Navigate();
            if (navigation != null) navigation.GoToUrl("https://www.google.com/travel/flights");
        }

        private void AgreeToConsentIfItShows()
        {
            if (ConsentAgreed) return;
            ReadOnlyCollection<IWebElement> consentButtons = Driver.FindElements(By.CssSelector("button"));
            foreach (IWebElement button in consentButtons)
            {
                string buttonText = button.Text;
                if (!buttonText.Contains("I agree")) continue;
                button.Click();
                ConsentAgreed = true;
                return;
            }
        }
        private void SetToOneWayTrip()
        {
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
                    return;
                }
            }
        }

        private void PopulateControls(string origin, string target)
        {
            ReadOnlyCollection<IWebElement> inputs = Driver.FindElements(By.CssSelector("input"));
            IWebElement originInput1 = inputs[0];
            IWebElement originInput2 = inputs[1];
            IWebElement destinationInput1 = inputs[2];
            IWebElement destinationInput2 = inputs[3];
            IWebElement dateInput1 = inputs[4];
            IWebElement dateInput2 = inputs[6];

            originInput1.Click();
            originInput2.SendKeys(origin);
            originInput2.SendKeys(Keys.Return);

            destinationInput1.Click();
            destinationInput2.SendKeys(target);
            destinationInput2.SendKeys(Keys.Return);

            dateInput1.Click();
            Thread.Sleep(1000);
            dateInput2.SendKeys(Date.ToString("ddd, MMM dd"));
            dateInput2.SendKeys(Keys.Return);
        }

        private void ClickDoneButton()
        {
            ReadOnlyCollection<IWebElement> doneButtons = Driver.FindElements(By.CssSelector("button"));
            foreach (IWebElement button in doneButtons)
            {
                string buttonText = button.GetAttribute("aria-label");
                if (buttonText == null || !buttonText.Contains("Done. Search for")) continue;
                button.Click();
                return;
            }
        }
    }
}