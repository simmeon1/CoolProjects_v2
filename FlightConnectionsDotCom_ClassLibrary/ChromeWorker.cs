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
        private Dictionary<string, FlightCollection> CollectedPathFlights { get; set; }

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
            CollectedPathFlights = new();
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
                Path pathName = new(new List<string> { origin, target });

                FlightCollection flights;
                if (CollectedPathFlights.ContainsKey(pathName.ToString()))
                {
                    flights = CollectedPathFlights[pathName.ToString()];
                }
                else
                {
                    OpenNewTab();
                    NavigateToUrl();
                    if (!ConsentAgreed) await AgreeToConsent();
                    await SetToOneWayTrip();
                    await PopulateControls(origin, target, dateFrom);
                    await SetStopsToNone();

                    List<DateTime> listOfExtraDates = new() { };
                    DateTime tempDate = dateFrom.AddDays(1);
                    while (DateTime.Compare(tempDate, dateTo) <= 0)
                    {
                        listOfExtraDates.Add(tempDate);
                        tempDate = tempDate.AddDays(1);
                    }
                    flights = await GetFlights(dateFrom, listOfExtraDates);
                }
                KeyValuePair<Path, FlightCollection> flightsForOriginToTarget = new(pathName, flights);
                if (!CollectedPathFlights.ContainsKey(pathName.ToString())) CollectedPathFlights.Add(pathName.ToString(), flights);
                PagesOpened++;
                pathsAndFlights.Add(flightsForOriginToTarget);
                Logger.Log($"Populated page for {origin} to {target} ({GetProgressString()})");
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
                    flightList = await FindElementAndWait(By.CssSelector("[role=list]"));
                    if (flightList == null) return;
                }
                catch (NoSuchElementException)
                {
                    return;
                }
                flights = await FindElementsAndWait(flightList, By.CssSelector("[role=listitem]"));
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
                int.TryParse(costText, out int cost);
                Flight item = new(
                                        DateTime.Parse($"{date.Day}-{date.Month}-{date.Year} {departingText}"),
                                        DateTime.Parse($"{date.Day}-{date.Month}-{date.Year} {arrivingText}").AddDays(arrivesNextDay ? 1 : 0),
                                        airlineText,
                                        TimeSpan.Parse(durationText),
                                        pathText,
                                        cost
                                    );
                results.Add(item);
            }
        }

        private async Task<ReadOnlyCollection<IWebElement>> FindElementsAndWait(IWebElement element, By by)
        {
            ReadOnlyCollection<IWebElement> result = element.FindElements(by);
            await Delayer.Delay(1000);
            return result;
        }

        private async Task<ReadOnlyCollection<IWebElement>> FindElementsAndWait(By by)
        {
            ReadOnlyCollection<IWebElement> result = Driver.FindElements(by);
            await Delayer.Delay(1000);
            return result;
        }

        private async Task<IWebElement> FindElementAndWait(By by)
        {
            IWebElement result = Driver.FindElement(by);
            await Delayer.Delay(1000);
            return result;
        }

        private async Task SetStopsToNone()
        {
            await ClickButtonWithAriaLabelText("Stops");
            IWebElement radioGroup = await FindElementAndWait(By.CssSelector("[role=radiogroup]"));
            ReadOnlyCollection<IWebElement> radioGroupChildren = await FindElementsAndWait(radioGroup, By.CssSelector("input"));
            await ClickAndWait(radioGroupChildren[1]);
            await ClickHeader();
        }

        private async Task ClickHeader()
        {
            await ClickAndWait(await FindElementAndWait(By.CssSelector("header")));
        }

        private string GetProgressString()
        {
            string percentageString = $"{PagesOpened / (double)PagesToOpen * 100}";
            if (Regex.IsMatch(percentageString, @"^\d+$")) percentageString += ".00";
            else if (Regex.IsMatch(percentageString, @"^\d+\.\d$")) percentageString += "0";
            percentageString += "%";
            percentageString = $"{Regex.Match(percentageString, @"(.*?\.\d\d).*%").Groups[1].Value}%";
            return $"{PagesOpened}/{PagesToOpen} ({percentageString}) at {DateTime.Now}";
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
            ReadOnlyCollection<IWebElement> consentButtons = await FindElementsAndWait(By.CssSelector("button"));
            foreach (IWebElement button in consentButtons)
            {
                string buttonText;
                try
                {
                    buttonText = button.Text;

                }
                catch (StaleElementReferenceException)
                {
                    continue;
                }
                if (!buttonText.Contains("I agree")) continue;
                await ClickAndWait(button);
                ConsentAgreed = true;
                return;
            }
        }

        private async Task SetToOneWayTrip()
        {
            ReadOnlyCollection<IWebElement> spans = await FindElementsAndWait(By.CssSelector("span"));
            foreach (IWebElement span in spans)
            {
                string spanText = span.Text;
                if (!spanText.Equals("Round trip")) continue;
                await ClickAndWait(span);
                ReadOnlyCollection<IWebElement> lis = await FindElementsAndWait(By.CssSelector("li"));
                foreach (IWebElement li in lis)
                {
                    string liText = li.Text;
                    if (!liText.Equals("One way")) continue;
                    await ClickAndWait(li);
                    return;
                }
            }
        }

        private async Task PopulateControls(string origin, string target, DateTime date)
        {
            ReadOnlyCollection<IWebElement> inputs = await FindElementsAndWait(By.CssSelector("input"));
            IWebElement originInput1 = inputs[0];
            IWebElement originInput2 = inputs[1];
            IWebElement destinationInput1 = inputs[2];
            IWebElement destinationInput2 = inputs[3];
            IWebElement dateInput1 = inputs[4];
            IWebElement dateInput2 = inputs[6];

            await ClickAndWait(originInput1);
            originInput2.SendKeys(origin);
            originInput2.SendKeys(Keys.Return);

            await ClickAndWait(destinationInput1);
            destinationInput2.SendKeys(target);
            destinationInput2.SendKeys(Keys.Return);

            await PopulateDate(dateInput1, dateInput2, date);
        }

        private async Task ClickAndWait(IWebElement element)
        {
            element.Click();
            await Delayer.Delay(1000);
        }

        private async Task PopulateDate(DateTime date)
        {
            ReadOnlyCollection<IWebElement> inputs = await FindElementsAndWait(By.CssSelector("input"));
            IWebElement dateInput1 = inputs[4];
            IWebElement dateInput2 = inputs[6];
            await PopulateDate(dateInput1, dateInput2, date);
        }

        private async Task PopulateDate(IWebElement dateInput1, IWebElement dateInput2, DateTime date)
        {
            await ClickAndWait(dateInput1);
            const string format = "ddd, MMM dd";
            foreach (char ch in format) dateInput2.SendKeys(Keys.Backspace);
            dateInput2.SendKeys(date.ToString(format));
            dateInput2.SendKeys(Keys.Return);
            await ClickButtonWithAriaLabelText("Done. Search for");
        }

        private async Task ClickButtonWithAriaLabelText(string text)
        {
            ReadOnlyCollection<IWebElement> buttons = await FindElementsAndWait(By.CssSelector("button"));
            foreach (IWebElement button in buttons)
            {
                string buttonText = button.GetAttribute("aria-label");
                if (buttonText == null || !buttonText.Contains(text)) continue;
                await ClickAndWait(button);
                return;
            }
        }
    }
}