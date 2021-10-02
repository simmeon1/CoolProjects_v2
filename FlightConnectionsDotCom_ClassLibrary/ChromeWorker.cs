using Common_ClassLibrary;
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

        public async Task<List<KeyValuePair<Path, List<KeyValuePair<Path, FlightCollection>>>>> ProcessPaths(List<Path> paths, DateTime dateFrom, DateTime dateTo)
        {
            List<KeyValuePair<Path, List<KeyValuePair<Path, FlightCollection>>>> results = new();

            PagesToOpen = 0;
            PagesOpened = 0;
            foreach (Path path in paths) for (int i = 0; i < path.Count() - 1; i++) PagesToOpen++;
            foreach (Path path in paths) results.Add(await ProcessPath(path, dateFrom, dateTo));
            return results;
        }

        private async Task<KeyValuePair<Path, List<KeyValuePair<Path, FlightCollection>>>> ProcessPath(Path path, DateTime dateFrom, DateTime dateTo)
        {
            List<KeyValuePair<Path, FlightCollection>> pathsAndFlights = new();
            for (int i = 0; i < path.Count() - 1; i++)
            {
                string origin = path[i];
                string target = path[i + 1];
                OpenNewTab();
                NavigateToUrl();
                if (!ConsentAgreed) await AgreeToConsent();
                SetToOneWayTrip();
                await PopulateControls(origin, target, dateFrom);
                await SetStopsToNone();
                PagesOpened++;

                List<DateTime> listOfExtraDates = new() { };
                DateTime tempDate = dateFrom.AddDays(1);
                while (DateTime.Compare(tempDate, dateTo) <= 0)
                {
                    listOfExtraDates.Add(tempDate);
                    tempDate = tempDate.AddDays(1);
                }
                FlightCollection flights = await GetFlights(dateFrom, listOfExtraDates);

                KeyValuePair<Path, FlightCollection> flightsForOriginToTarget = new(new Path(new List<string> { origin, target }), flights);
                pathsAndFlights.Add(flightsForOriginToTarget);
                Logger.Log($"Populated page for {origin} to {target} ({GetPercentageAndCountString()})");
            }
            return new(path, pathsAndFlights);
        }

        private async Task<FlightCollection> GetFlights(DateTime date, List<DateTime> extraDates)
        {
            List<Flight> results = new();
            await GetFlightsForDate(date, results);
            foreach (DateTime extraDate in extraDates)
            {
                await PopulateDate(extraDate);
                await GetFlightsForDate(extraDate, results);
            }
            return new FlightCollection(results);
        }

        private async Task GetFlightsForDate(DateTime date, List<Flight> results)
        {
            IWebElement flightList;
            ReadOnlyCollection<IWebElement> flights;

            while (true)
            {
                try
                {
                    await Delayer.Delay(1000);
                    flightList = Driver.FindElement(By.CssSelector("[role=list]"));
                    if (flightList == null) return;
                }
                catch (NoSuchElementException)
                {
                    return;
                }
                flights = flightList.FindElements(By.CssSelector("[role=listitem]"));
                if (flights == null) return;
                if (flights[flights.Count - 1].GetAttribute("innerText").Contains("more flights")) flights[flights.Count - 1].Click();
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
                Flight item = new(
                                        DateTime.Parse($"{date.Day}-{date.Month}-{date.Year} {departingText}"),
                                        DateTime.Parse($"{date.Day}-{date.Month}-{date.Year} {arrivingText}").AddDays(arrivesNextDay ? 1 : 0),
                                        airlineText,
                                        TimeSpan.Parse(durationText),
                                        pathText,
                                        int.Parse(costText)
                                    );
                results.Add(item);
            }
        }

        private async Task SetStopsToNone()
        {
            ClickButtonWithAriaLabelText("Stops");
            await Delayer.Delay(500);
            IWebElement radioGroup = Driver.FindElement(By.CssSelector("[role=radiogroup]"));
            ReadOnlyCollection<IWebElement> radioGroupChildren = radioGroup.FindElements(By.CssSelector("input"));
            await Delayer.Delay(1000);
            radioGroupChildren[1].Click();
            await Delayer.Delay(1000);
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

        private async Task PopulateControls(string origin, string target, DateTime date)
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

            await PopulateDate(dateInput1, dateInput2, date);
        }
        private async Task PopulateDate(DateTime date)
        {
            ReadOnlyCollection<IWebElement> inputs = Driver.FindElements(By.CssSelector("input"));
            IWebElement dateInput1 = inputs[4];
            IWebElement dateInput2 = inputs[6];
            await PopulateDate(dateInput1, dateInput2, date);
        }

        private async Task PopulateDate(IWebElement dateInput1, IWebElement dateInput2, DateTime date)
        {
            dateInput1.Click();
            await Delayer.Delay(1000);
            const string format = "ddd, MMM dd";
            foreach (char ch in format) dateInput2.SendKeys(Keys.Backspace);
            dateInput2.SendKeys(date.ToString(format));
            dateInput2.SendKeys(Keys.Return);
            ClickButtonWithAriaLabelText("Done. Search for");
            await Delayer.Delay(1000);
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