using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Workers;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.JourneyRetrievers
{
    public class GoogleFlightsWorker
    {
        private JourneyRetrieverComponents c;
        private bool stopsSet;

        public GoogleFlightsWorker(JourneyRetrieverComponents c)
        {
            this.c = c;
        }

        public void Initialise()
        {
            SetUpSearch();
        }

        public Task<JourneyCollection> GetJourneysForDates(List<DirectPath> paths, List<DateTime> allDates)
        {
            var results = new List<Journey>();
            var lastResultCount = 0;
            var remainingPaths = paths.Select(x => x).ToList();
            var collectedFlightsSet = new HashSet<string>();

            while (remainingPaths.Any())
            {
                var remainingPathsSet = remainingPaths.Select(x => x.Path.ToString()).ToHashSet();
                
                //Origin must be done in singles, otherwise it repeating in both search fields will wipe it from destination
                var groups = remainingPaths.GroupBy(x => x.GetStart(), y => y.GetEnd());

                var origins = new List<string>();
                var destinations = new List<string>();
                foreach (var group in groups)
                {
                    var origin = group.Key;
                    if (!destinations.Contains(origin) && origins.Count < 7)
                    {
                        origins.Add(origin);
                        foreach (var dest in group)
                        {
                            if (!origins.Contains(dest) && !destinations.Contains(dest) && destinations.Count < 7)
                            {
                                destinations.Add(dest);
                            }
                        }
                    }
                }

                var searchDesc = $"{origins.ConcatenateListOfStringsToCommaAndSpaceString()} to {destinations.ConcatenateListOfStringsToCommaAndSpaceString()}";
                c.Log(
                    $"Looking for {searchDesc}"
                );

                for (int i = 0; i < allDates.Count; i++)
                {
                    DateTime date = allDates[i];
                    if (i == 0)
                    {
                        c.Log($"Populating locations...");
                        PopulateSearchField(origins, "Origin");
                        PopulateSearchField(destinations, "Destination");
                    }

                    c.Log($"Date is {date.ToShortDateString()}");
                    PopulateDateAndHitDone(date);
                    if (!stopsSet) SetStopsToNone();
                    List<Journey> flightsForDate = GetFlightsForDate(date, collectedFlightsSet, remainingPathsSet);
                    results.AddRange(flightsForDate);
                }

                foreach (string origin in origins)
                {
                    foreach (var destination in destinations)
                    {
                        var path = new DirectPath(origin, destination);
                        remainingPaths = remainingPaths.Where(x => x.ToString() != path.ToString()).ToList();
                    }
                }

                c.Log(
                    $"Collected data for paths from {searchDesc} ({results.Count - lastResultCount} journeys) ({Globals.GetPercentageAndCountString(paths.Count - remainingPaths.Count, paths.Count)})"
                );
                lastResultCount = results.Count;
            }

            return Task.FromResult(new JourneyCollection(results.OrderBy(j => j.ToString()).ToList()));
        }

        private List<Journey> GetFlightsForDate(DateTime date, HashSet<string> collectedFlightsSet, HashSet<string> remainingPathsSet)
        {
            var results = new List<Journey>();

            ReadOnlyCollection<IWebElement> flights = c.FindElements(
                GetCssSelectorParam("div[aria-label$='Select flight']")
            );
            foreach (IWebElement flight in flights)
            {
                IWebElement parent = (IWebElement) c.ExecuteScript(
                    "return arguments[0].parentElement",
                    flight
                );
                string text = parent.GetAttribute("innerText");
                string[] flightText = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                if (!flightText[6].Trim().Equals("Nonstop")) continue;
                Journey item = GetJourneyFromText(date, flightText);
                if (remainingPathsSet.Contains(item.Path) && !collectedFlightsSet.Contains(item.ToString()))
                {
                    results.Add(item);
                    collectedFlightsSet.Add(item.ToString());
                }
            }

            return results;
        }

        private static Journey GetJourneyFromText(DateTime date, string[] flightText)
        {
            string departingText = flightText[0].Replace(" ", "").Trim();
            string arrivingText = flightText[2].Replace(" ", "").Trim();
            string airlineText = flightText[3].Trim();

            string durationText = flightText[4];
            var arrivesAfterDays = 0;
            var regexMatches = Regex.Matches(arrivingText, "\\+(\\d)");
            if (regexMatches.Any())
            {
                arrivesAfterDays = int.Parse(regexMatches[0].Groups[1].Value);
                arrivingText = arrivingText.Replace(regexMatches[0].Groups[0].Value, "").Trim();
            }

            int hours = ParseTimeUnit(durationText, "hr");
            int minutes = ParseTimeUnit(durationText, "min");

            Match pathMatch = Regex.Match(flightText[5].Trim(), @"(\w+)\W+(\w+)");
            string pathText = $"{pathMatch.Groups[1].Value}-{pathMatch.Groups[2].Value}";
            string costText = Regex.Replace(flightText[flightText.Length - 1], "\\D", "").Trim();
            double.TryParse(costText, out double cost);
            Journey item = new(
                DateTime.Parse($"{date.Day}-{date.Month}-{date.Year} {departingText}"),
                DateTime.Parse($"{date.Day}-{date.Month}-{date.Year} {arrivingText}").AddDays(arrivesAfterDays),
                airlineText,
                new TimeSpan(hours, minutes, 0),
                pathText,
                cost
            );
            return item;
        }

        private static int ParseTimeUnit(string durationText, string unitType)
        {
            var regexMatches = Regex.Matches(durationText, @$"(\d+) {unitType}");
            return regexMatches.Any() ? int.Parse(regexMatches[0].Groups[1].Value) : 0;
        }

        private void WaitForProgressBarToBeGone()
        {
            IWebElement loadingBar = c.FindElement(GetCssSelectorParam("[data-buffervalue='1']"));
            try
            {
                c.Until(_ => loadingBar.GetAttribute("aria-hidden") == null, 1);
                c.Until(_ => loadingBar.GetAttribute("aria-hidden") != null);
            }
            catch (Exception)
            {
                //loaded
            }
        }

        private void SetStopsToNone()
        {
            c.FindElementAndClickIt(GetCssSelectorParam("[aria-label='Stops, Not selected']"));
            Sleep(500);
            c.FindElementAndClickIt(GetCssSelectorParam("[aria-label*='Nonstop only']"));
            Sleep();

            c.FindElementAndClickIt(GetCssSelectorParam("[data-filtertype*='10'] [aria-label*='Close dialog']"));
            stopsSet = true;
            WaitForProgressBarToBeGone();
        }

        private void PopulateSearchField(IEnumerable<string> locations, string keyword)
        {
            var keywordLower = keyword.ToLower();

            c.FindElementAndClickIt(GetCssSelectorParam($"[aria-placeholder*='Where {(keyword == "Origin" ? "from" : "to")}'] input"));
            Sleep();

            FindElementParameters checkmarkDoneButtonParam = GetCssSelectorParam($"[aria-label*='Enter your {keywordLower}'] [aria-label*='Done']");
            var el = c.FindElement(checkmarkDoneButtonParam);
            el = (IWebElement) c.ExecuteScript("return arguments[0].parentElement.parentElement", el);
            var displayStyle = (string) c.ExecuteScript("return arguments[0].style.display", el);
            if (displayStyle == "none")
            {
                c.FindElementAndClickIt(GetCssSelectorParam($"[aria-label='{keyword}, Select multiple airports']"));
            }
            else
            {
                c.FindElementAndSendKeysToIt(
                    GetCssSelectorParam($"[aria-label*='Enter your {keywordLower}'] input"),
                    true,
                    Keys.Backspace + Keys.Backspace + Keys.Backspace + Keys.Backspace + Keys.Backspace +
                    Keys.Backspace + Keys.Backspace
                );
            }

            while (c.FindElements(GetCssSelectorParam("div[data-code]")).Any(x => x.Displayed))
            {
                c.FindElementAndSendKeysToIt(
                    GetCssSelectorParam($"[aria-label*='Enter your {keywordLower}'] input"),
                    false,
                    Keys.Backspace
                );
            }

            foreach (string location in locations)
            {
                c.FindElementAndSendKeysToIt(
                    GetCssSelectorParam($"[aria-label*='Enter your {keywordLower}'] input"),
                    false,
                    location
                );

                c.FindElementAndClickIt(GetCssSelectorParam($"[data-code='{location}'] input"));
                Sleep();
            }

            c.FindElementAndClickIt(checkmarkDoneButtonParam);

            Sleep();
        }

        private void PopulateDateAndHitDone(DateTime date)
        {
            const string format = "ddd, MMM dd";

            c.FindElementAndClickIt(GetCssSelectorParam("[aria-label='Departure']"));

            Sleep();

            c.FindElementAndSendKeysToIt(
                GetCssSelectorParam("[data-same-day-selection] [aria-label='Departure']"),
                false,
                date.ToString(format) + Keys.Return
            );

            c.FindElementAndClickIt(
                GetCssSelectorParam("[aria-label^='Done. Search for one-way flights']")
            );

            if (!stopsSet)
            {
                c.FindElementAndClickIt(
                    GetCssSelectorParam("[aria-label='Search']")
                );
            }

            WaitForProgressBarToBeGone();
        }

        private void Sleep(int milliseconds = 200)
        {
            c.Sleep(milliseconds);
        }

        private static FindElementParameters GetCssSelectorParam(string cssSelector)
        {
            return FindElementParameters.WithSelector(By.CssSelector(cssSelector));
        }

        private void SetUpSearch()
        {
            c.NavigateToUrl("https://www.google.com/travel/flights?curr=GBP");
            try
            {
                c.FindElementAndClickIt(
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
            FindElementParameters param = GetCssSelectorParam("[aria-label*='Change ticket type']");
            IWebElement el = c.FindElement(param);
            IWebElement parentEl = (IWebElement) c.ExecuteScript("return arguments[0].parentElement", el);
            parentEl.Click();

            c.FindElementAndClickIt(GetCssSelectorParam("[aria-label*='Select your ticket type'] [data-value='2']"));
        }
    }
}