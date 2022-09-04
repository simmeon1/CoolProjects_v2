using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public Task<JourneyCollection> GetJourneysForDates(List<DirectPath> paths, List<DateTime> allDates)
        {
            Dictionary<string, List<string>> originAndTargetGroups = new();
            Dictionary<string, int> groupsAndDestinationsCounts = new();

            foreach (DirectPath path in paths)
            {
                string origin = $"{path.GetStart()}";
                int groupIndex = groupsAndDestinationsCounts.ContainsKey(origin) ? groupsAndDestinationsCounts[origin] / 7 : 0;
                string originGroup = $"{origin}_{groupIndex}";
                
                if (!originAndTargetGroups.ContainsKey(originGroup))
                {
                    originAndTargetGroups.Add(originGroup, new List<string>());
                    if (!groupsAndDestinationsCounts.ContainsKey(origin))
                    {
                        groupsAndDestinationsCounts.Add(origin, 0);
                    }
                }

                originAndTargetGroups[originGroup].Add(path.GetEnd());
                groupsAndDestinationsCounts[origin]++;
            }
            
            List<Journey> results = new();
            int counter = 0;
            foreach ((string origin, List<string> targets) in originAndTargetGroups)
            {
                int originalCount = results.Count;
                for (int i = 0; i < allDates.Count; i++)
                {
                    DateTime date = allDates[i];
                    if (i == 0) PopulateLocations(origin[..3], targets);
                    PopulateDateAndHitDone(date);
                    if (!StopsSet) SetStopsToNone();
                    GetFlightsForDate(date, results);
                }
                counter++;
                C.Logger.Log($"Collected data for paths from {origin} ({results.Count - originalCount} journeys) ({Globals.GetPercentageAndCountString(counter, originAndTargetGroups.Count)})");
            }
            return Task.FromResult(new JourneyCollection(results.OrderBy(j => j.ToString()).ToList()));
        }

        private void GetFlightsForDate(DateTime date, List<Journey> results)
        {
            ReadOnlyCollection<IWebElement> flights = C.Driver.FindElements(By.CssSelector("div[aria-label$='Select flight']"));
            foreach (IWebElement flight in flights)
            {
                IWebElement parent = (IWebElement) C.JavaScriptExecutor.ExecuteScript(
                    "return arguments[0].parentElement",
                    flight
                );
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
                cost
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

        private void PopulateLocations(
            string origin,
            List<string> destinations
        )
        {
            C.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("[role='combobox'][aria-autocomplete='inline']"),
                    Index = 0
                }
            );

            Sleep();

            C.FindElementAndSendKeysToIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("[role='combobox'][aria-autocomplete='both']"),
                    Index = 1
                },
                true,
                JourneyRetrieverData.GetTranslation(origin) + Keys.Return
            );
            
            Sleep();

            C.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("[role='combobox'][aria-autocomplete='inline']"),
                    Index = 1
                }
            );
            
            IWebElement addButton = C.FindElement(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("[aria-label='Destination, Select multiple airports']")
                }
            );
            if (addButton.Displayed) addButton.Click();
            
            while (C.Driver.FindElements(By.CssSelector("div[data-code]")).Any(x => x.Displayed))
            {
                C.FindElementAndSendKeysToIt(
                    new FindElementParameters
                    {
                        BySelector = By.CssSelector("[role='combobox'][aria-autocomplete='both']"),
                        Index = 1
                    },
                    false,
                    Keys.Backspace
                );
            }
            
            foreach (string destination in destinations)
            {
                string destTranslation = JourneyRetrieverData.GetTranslation(destination);
                C.FindElementAndSendKeysToIt(
                    new FindElementParameters
                    {
                        BySelector = By.CssSelector("[role='combobox'][aria-autocomplete='both']"),
                        Index = 1
                    },
                    false,
                    destTranslation
                );

                C.FindElementAndClickIt(
                    new FindElementParameters
                    {
                        BySelector = By.CssSelector($"[data-code='{destTranslation}'] input"),
                    }
                );
            }

            C.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector($"[aria-label='Done']"),
                    Index = 1
                }
            );
            
            Sleep();
        }

        private void PopulateDateAndHitDone(DateTime date)
        {
            const string format = "ddd, MMM dd";
            By selector = By.CssSelector("[aria-label='Departure']");

            C.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = selector,
                }
            );

            Sleep();

            C.FindElementAndSendKeysToIt(
                new FindElementParameters
                {
                    BySelector = selector,
                    Index = 1
                },
                false,
                date.ToString(format) + Keys.Return
            );

            C.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = By.CssSelector("[aria-label^='Done. Search for one-way flights']"),
                }
            );
            WaitForProgressBarToBeGone();
        }

        private void Sleep()
        {
            C.Delayer.Sleep(200);
        }
    }
}