﻿using Common_ClassLibrary;
using OpenQA.Selenium;
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
        private IDelayer Delayer { get; set; }
        private DateTime Date { get; set; }
        private bool ConsentAgreed { get; set; }
        private int PagesToOpen { get; set; }
        private int PagesOpened { get; set; }

        public ChromeWorker(IWebDriver driver, IJavaScriptExecutor jSExecutor, ILogger logger, IDelayer delayer)
        {
            Driver = driver;
            JSExecutor = jSExecutor;
            Logger = logger;
            Delayer = delayer;
        }

        public async Task<List<KeyValuePair<List<string>, List<KeyValuePair<List<string>, List<Flight>>>>>> ProcessPaths(List<List<string>> paths, DateTime date)
        {
            List<KeyValuePair<List<string>, List<KeyValuePair<List<string>, List<Flight>>>>> results = new();
            Date = date;

            PagesToOpen = 0;
            PagesOpened = 0;
            foreach (List<string> path in paths) for (int i = 0; i < path.Count - 1; i++) PagesToOpen++;
            foreach (List<string> path in paths) results.Add(await ProcessPath(path));
            return results;
        }

        private async Task<KeyValuePair<List<string>, List<KeyValuePair<List<string>, List<Flight>>>>> ProcessPath(List<string> path)
        {
            List<KeyValuePair<List<string>, List<Flight>>> pathsAndFlights = new();
            for (int i = 0; i < path.Count - 1; i++)
            {
                string origin = path[i];
                string target = path[i + 1];
                OpenNewTab();
                NavigateToUrl();
                if (!ConsentAgreed) await AgreeToConsent();
                SetToOneWayTrip();
                await PopulateControls(origin, target);
                ClickButtonWithAriaLabelText("Done. Search for");
                await Delayer.Delay(1000);
                await SetStopsToNone();
                PagesOpened++;
                List<Flight> flights = await GetFlights();
                KeyValuePair<List<string>, List<Flight>> flightsForOriginToTarget = new(new List<string>() { origin, target }, flights);
                pathsAndFlights.Add(flightsForOriginToTarget);
                Logger.Log($"Populated page for {origin} to {target} ({GetPercentageAndCountString()})");
            }
            return new(path, pathsAndFlights);
        }

        private async Task<List<Flight>> GetFlights()
        {
            List<Flight> results = new();
            IWebElement flightList;
            ReadOnlyCollection<IWebElement> flights;

            while (true)
            {
                try
                {
                    await Delayer.Delay(1000);
                    flightList = Driver.FindElement(By.CssSelector("[role=list]"));
                }
                catch (NoSuchElementException)
                {
                    return new();
                }
                flights = flightList.FindElements(By.CssSelector("[role=listitem]"));
                if (flights[flights.Count - 1].Text.Contains("more flights")) flights[flights.Count - 1].Click();
                else break;
            }

            foreach (IWebElement flight in flights)
            {
                string text = flight.GetAttribute("innerText");
                if (text.Contains("flights")) continue;
                string[] flightText = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                string departingText = flightText[0].Replace(" ", "").Trim();
                string arrivingText = flightText[2].Replace(" ", "").Trim();

                bool arrivesNextDay = false;
                if (arrivingText.Contains("+1"))
                {
                    arrivesNextDay = true;
                    arrivingText = arrivingText.Replace("+1", "").Trim();
                }

                string airlineText = flightText[3].Trim();
                string durationText = Regex.Replace(flightText[4], "(\\d+).*?(\\d+).*", "$1:$2").Trim();
                string pathText = flightText[5].Trim();
                string costText = Regex.Replace(flightText[7], ".*?(\\d+).*", "$1").Trim();
                results.Add(
                    new Flight(
                        DateTime.Parse($"{Date.Day}-{Date.Month}-{Date.Year} {departingText}"),
                        DateTime.Parse($"{Date.Day}-{Date.Month}-{Date.Year} {arrivingText}").AddDays(arrivesNextDay ? 1 : 0),
                        airlineText,
                        TimeSpan.Parse(durationText),
                        pathText,
                        int.Parse(costText)
                    )
                );
            }
            return results;
        }

        private async Task SetStopsToNone()
        {
            ClickButtonWithAriaLabelText("Stops");
            await Delayer.Delay(1000);
            IWebElement radioGroup = Driver.FindElement(By.CssSelector("[role=radiogroup]"));
            ReadOnlyCollection<IWebElement> radioGroupChildren = radioGroup.FindElements(By.CssSelector("input"));
            radioGroupChildren[1].Click();
            ClickHeader();
        }

        private void ClickHeader()
        {
            Driver.FindElement(By.CssSelector("header")).Click();
        }

        private string GetPercentageAndCountString()
        {
            string percentageString = $"{PagesOpened / (double)PagesToOpen * 100}%";
            percentageString = $"{Regex.Match(percentageString, @"(.*?\.\d\d).*%").Groups[1].Value}%";
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

        private async Task AgreeToConsent()
        {
            await Delayer.Delay(1000);
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

        private async Task PopulateControls(string origin, string target)
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
            await Delayer.Delay(1000);
            dateInput2.SendKeys(Date.ToString("ddd, MMM dd"));
            dateInput2.SendKeys(Keys.Return);
        }

        private void ClickButtonWithAriaLabelText(string text)
        {
            ReadOnlyCollection<IWebElement> buttons = Driver.FindElements(By.CssSelector("button"));
            foreach (IWebElement button in buttons)
            {
                string buttonText = button.GetAttribute("aria-label");
                if (buttonText == null || !buttonText.Contains(text)) continue;
                button.Click();
                return;
            }
        }
    }
}