using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Interfaces;
using JourneyPlanner_ClassLibrary.Workers;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.JourneyRetrievers
{
    public class GoogleFlightsWorker : IJourneyRetriever
    {
        private JourneyRetrieverComponents C { get; set; }
        private JourneyRetrieverData JourneyRetrieverData { get; set; }
        private bool StopsSet { get; set; }
        private string LastTypedOrigin { get; set; }
        private bool InitialSearchDone { get; set; }

        public GoogleFlightsWorker(JourneyRetrieverComponents c)
        {
            C = c;
        }

        public void Initialise(JourneyRetrieverData data)
        {
            JourneyRetrieverData = data;
            LastTypedOrigin = "";
            StopsSet = false;
            SetUpSearch();
        }

        private void SetUpSearch()
        {
            C.NavigateToUrl("https://www.google.com/travel/flights?curr=GBP");
            try
            {
                C.FindElementAndClickIt(
                    new FindElementParameters
                    {
                        BySelector = By.CssSelector("button"),
                        Matcher = x =>
                        {
                            string innerText = x.GetAttribute("innerText");
                            return !innerText.IsNullOrEmpty() && innerText.Equals("Reject all");
                        }
                    }
                );
            }
            catch (Exception)
            {
                //Doesn't show
            }
            SetToOneWayTrip();
        }

        private void SetToOneWayTrip()
        {
            C.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("[aria-label='Round trip, Change ticket type.']"),
                }
            );
            IWebElement list = C.FindElement(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("[aria-label='Select your ticket type.']"),
                }
            );

            C.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("li"),
                    Matcher = x =>
                    {
                        string innerText = x.GetAttribute("innerText");
                        return !innerText.IsNullOrEmpty() && innerText.Equals("One way");
                    },
                    Container = list
                }
            );
        }

        public Task<JourneyCollection> GetJourneysForDates(string origin, string destination, List<DateTime> allDates)
        {
            List<Journey> results = new();
            for (int i = 0; i < allDates.Count; i++)
            {
                DateTime date = allDates[i];
                PopulateControlsAndSearch(origin, destination, date, i != 0);
                GetFlightsForDate(date, results);
            }

            return Task.FromResult(new JourneyCollection(results));
        }

        private void GetFlightsForDate(DateTime date, List<Journey> results)
        {
            ReadOnlyCollection<IWebElement> flights = C.Driver.FindElements(By.CssSelector("[aria-label^='From']"));
            foreach (IWebElement flight in flights)
            {
                IWebElement parent = (IWebElement) C.JavaScriptExecutor.ExecuteScript("return arguments[0].parentElement", flight); 
                string text = parent.GetAttribute("innerText");
                string[] flightText = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                if (!flightText[6].Trim().Equals("Nonstop")) continue;
                Journey item = GetJourneyFromText(date, flightText);
                results.Add(item);
            }
        }

        private static Journey GetJourneyFromText(DateTime date, string[] flightText)
        {
            string departingText = flightText[0].Replace(" ", "").Trim();
            string arrivingText = flightText[2].Replace(" ", "").Trim();

            bool arrivesNextDay = false;
            if (arrivingText.Contains("+1"))
            {
                arrivesNextDay = true;
                arrivingText = arrivingText.Replace("+1", "").Trim();
            }

            string airlineText = flightText[3].Trim();

            string durationText = flightText[4];
            durationText = Regex.Match(durationText, "(\\d+)\\D+(\\d+).*").Success
                ? Regex.Replace(durationText, "(\\d+).*?(\\d+).*", "$1:$2").Trim()
                : Regex.Match(durationText, "(\\d+).*hr").Success
                    ? Regex.Replace(durationText, "(\\d+).*", "$1:00").Trim()
                    : Regex.Replace(durationText, "(\\d+).*", "0:$1").Trim();

            Match pathMatch = Regex.Match(flightText[5].Trim(), @"(\w+)\W+(\w+)");
            string pathText = $"{pathMatch.Groups[1].Value}-{pathMatch.Groups[2].Value}";
            string costText = Regex.Replace(flightText[flightText.Length - 1], "\\D", "").Trim();
            double.TryParse(costText, out double cost);
            Journey item = new(
                DateTime.Parse($"{date.Day}-{date.Month}-{date.Year} {departingText}"),
                DateTime.Parse($"{date.Day}-{date.Month}-{date.Year} {arrivingText}").AddDays(arrivesNextDay ? 1 : 0),
                airlineText,
                TimeSpan.Parse(durationText),
                pathText,
                cost,
                nameof(GoogleFlightsWorker)
            );
            return item;
        }

        private void WaitForProgressBarToBeGone()
        {
            IWebElement loadingBar = C.FindElement(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("[data-buffervalue='1']"),
                    Matcher = x => x.GetProperty("clientHeight").Equals("4")
                }
            );
            
            try
            {
                C.WebDriverWaitProvider.Until(d => loadingBar.GetAttribute("aria-hidden") == null, 1);
                C.WebDriverWaitProvider.Until(d => loadingBar.GetAttribute("aria-hidden") != null);
            }
            catch (Exception)
            {
                //loaded
            }
        }

        private void SetStopsToNone()
        {
            C.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("[aria-label='Stops, Not selected']")
                }
            );
            
            C.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("label"),
                    Matcher = x =>
                    {
                        string innerText = x.GetAttribute("innerText");
                        return !innerText.IsNullOrEmpty() && innerText.Equals("Nonstop only");
                    }
                }
            );
            
            C.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("header")
                }
            );
            StopsSet = true;
            WaitForProgressBarToBeGone();
        }

        private void PopulateControlsAndSearch(
            string origin,
            string destination,
            DateTime dateFrom,
            bool skipLocationInput
        )
        {
            if (!skipLocationInput)
            {
                if (!LastTypedOrigin.Equals(destination))
                {
                    InputLocation(destination, true);
                    InputLocation(origin, false);
                }
                else
                {
                    InputLocation(origin, false);
                    InputLocation(destination, true);
                }
                LastTypedOrigin = origin;
            }
            PopulateDateAndHitDone(dateFrom);
            if (!StopsSet) SetStopsToNone();
        }

        private void InputLocation(string origin, bool isTarget)
        {
            C.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("[role='combobox'][aria-autocomplete='inline']"),
                    Index = isTarget ? 1 : 0
                }
            );
            C.FindElementAndSendKeysToIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("[role='combobox'][aria-autocomplete='both']"),
                    Index = 1
                },
                false,
                new List<string> {JourneyRetrieverData.GetTranslation(origin), Keys.Return}
            );
        }

        private void PopulateDateAndHitDone(DateTime date)
        {
            const string format = "ddd, MMM dd";
            By selector = By.CssSelector("[aria-label='Departure']");
            List<string> keysToSend = new();
            foreach (char ch in format) keysToSend.Add(Keys.Backspace);
            keysToSend.Add(date.ToString(format));
            keysToSend.Add(Keys.Return);

            C.FindElementAndSendKeysToIt(
                new FindElementParameters
                {
                    BySelector = selector,
                },
                false,
                keysToSend
            );

            if (!InitialSearchDone)
            {
                C.FindElementAndClickIt(
                    new FindElementParameters
                    {
                        BySelector = By.CssSelector("button"),
                        Matcher = x =>
                        {
                            string innerText = x.GetAttribute("innerText");
                            return !innerText.IsNullOrEmpty() && innerText.Equals("Search");
                        }
                    }
                );
                InitialSearchDone = true;
            }
            WaitForProgressBarToBeGone();
        }
    }
}