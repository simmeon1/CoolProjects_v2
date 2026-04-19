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
using OpenQA.Selenium.Support.UI;

namespace JourneyPlanner_ClassLibrary.JourneyRetrievers;

public class GoogleFlightsWorker(JourneyRetrieverComponents c)
{
    private bool stopsSet;

    public void Initialise()
    {
        SetUpSearch();
    }

    public Task<JourneyCollection> GetJourneysForDates(List<DirectPath> paths, List<DateTime> allDates)
    {
        var results = new List<Journey>();
        try
        {
            DoWork(paths, allDates, results);
            return Task.FromResult(new JourneyCollection(results.OrderBy(j => j.ToString()).ToList()));
        }
        catch (Exception ex)
        {
            throw new GoogleFlightsWorkerException(
                new JourneyCollection(results.OrderBy(j => j.ToString()).ToList()),
                ex
            );
        }
    }

    private void DoWork(List<DirectPath> paths, List<DateTime> allDates, List<Journey> results)
    {
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

            var searchDesc =
                $"{origins.ConcatenateListOfStringsToCommaAndSpaceString()} to {destinations.ConcatenateListOfStringsToCommaAndSpaceString()}";
            c.logger.Log($"Looking for {searchDesc}");

            var dateResults = new List<Journey>();
            for (var i = 0; i < allDates.Count; i++)
            {
                var date = allDates[i];
                if (i == 0)
                {
                    c.logger.Log("Populating locations...");
                    PopulateSearchField(origins, "Origin");
                    PopulateSearchField(destinations, "Destination");
                }

                c.logger.Log($"Date is {date.ToShortDateString()}");
                PopulateDateAndHitDone(date);
                if (!stopsSet)
                {
                    SetStopsToNone();
                }
                var flightsForDate = GetFlightsForDate(date, collectedFlightsSet, remainingPathsSet);
                c.logger.Log($"Found {flightsForDate.Count} flights for {date.ToShortDateString()}");
                dateResults.AddRange(flightsForDate);
            }
            results.AddRange(dateResults);

            foreach (var origin in origins)
            {
                foreach (var destination in destinations)
                {
                    var path = new DirectPath(origin, destination);
                    remainingPaths = remainingPaths.Where(x => x.ToString() != path.ToString()).ToList();
                }
            }

            c.logger.Log(
                $"Collected data for paths from {searchDesc} ({results.Count - lastResultCount} journeys) ({Globals.GetPercentageAndCountString(paths.Count - remainingPaths.Count, paths.Count)})"
            );
            lastResultCount = results.Count;
        }
    }

    private List<Journey> GetFlightsForDate(
        DateTime date,
        HashSet<string> collectedFlightsSet,
        HashSet<string> remainingPathsSet
    )
    {
        var results = new List<Journey>();
        var flights = DoUntil(() => FindElements("[role='tabpanel'] li")
            .Select(f => (f.GetAttribute("innerText") ?? "").Split("\n", StringSplitOptions.RemoveEmptyEntries))
            .Where(f => f.Length > 1) // Every flight looks duplicated
            .ToList()
        );

        foreach (var flight in flights)
        {
            if (!flight[6].Trim().Equals("Nonstop"))
            {
                c.logger.Log("Flight is not nonstop. Continuing.");
                continue;
            }
            var item = GetJourneyFromText(date, flight);
            if (remainingPathsSet.Contains(item.Path) && !collectedFlightsSet.Contains(item.ToString()))
            {
                results.Add(item);
                collectedFlightsSet.Add(item.ToString());
            }
        }

        c.logger.Log($"Page has {flights.Count} flights, only {results.Count} are relevant.");
        return results;
    }

    private static By ByCssSelector(string cssSelectorToFind)
    {
        return By.CssSelector(cssSelectorToFind);
    }

    private Journey GetJourneyFromText(DateTime date, string[] flightText)
    {
        var departingText = flightText[0].Replace(" ", "").Trim();
        var arrivingText = flightText[2].Replace(" ", "").Trim();
        var airlineText = flightText[3].Trim();

        var durationText = flightText[4];
        var arrivesAfterDays = 0;
        var regexMatches = Regex.Matches(arrivingText, @"\+(\d)");
        if (regexMatches.Any())
        {
            arrivesAfterDays = int.Parse(regexMatches[0].Groups[1].Value);
            arrivingText = arrivingText.Replace(regexMatches[0].Groups[0].Value, "").Trim();
        }

        var hours = ParseTimeUnit(durationText, "hr");
        var minutes = ParseTimeUnit(durationText, "min");

        var pathMatch = Regex.Match(flightText[5].Trim(), @"(\w+)\W+(\w+)");
        var pathText = $"{pathMatch.Groups[1].Value}-{pathMatch.Groups[2].Value}";
        var costText = Regex.Replace(flightText[^1], "\\D", "").Trim();
        double.TryParse(costText, out var cost);
        Journey item = new (
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
        var loadingBar = DoUntil(() => FindElement("[data-buffervalue='1']"), 3);
        try
        {
            DoUntil(() => loadingBar.GetAttribute("aria-hidden") == null);
            DoUntil(() => loadingBar.GetAttribute("aria-hidden") != null);
        }
        catch (WebDriverTimeoutException e)
        {
            // loaded
        }
    }

    private IWebElement FindElement(string cssSelectorToFind)
    {
        return c.driver.FindElement(ByCssSelector(cssSelectorToFind));
    }

    private ReadOnlyCollection<IWebElement> FindElements(string cssSelectorToFind)
    {
        return c.driver.FindElements(ByCssSelector(cssSelectorToFind));
    }

    private void ClickElement(string cssSelector)
    {
        DoUntil(() =>
            {
                var el = FindElement(cssSelector);
                el.Click();
                return true;
            }
        );
    }

    private void SendKeysToElement(string cssSelector, string keys)
    {
        DoUntil(() =>
            {
                var el = FindElement(cssSelector);
                el.SendKeys(keys);
                return true;
            }
        );
    }

    private TResult DoUntil<TResult>(Func<TResult?> condition, int seconds = 10)
    {
        var wait = new WebDriverWait(c.driver, TimeSpan.FromSeconds(seconds));
        wait.IgnoreExceptionTypes(
            typeof(NoSuchElementException),
            typeof(StaleElementReferenceException),
            typeof(ElementNotInteractableException)
        );
        wait.PollingInterval = TimeSpan.FromMilliseconds(100);
        return wait.Until(_ => condition());
    }

    private void SetStopsToNone()
    {
        ClickElement("[aria-label='Stops, Not selected']");
        ClickElement("div[aria-label*='Stops'] > div:nth-child(2) input");
        ClickElement("[data-filtertype*='10'] [aria-label*='Close dialog']");
        stopsSet = true;
        WaitForProgressBarToBeGone();
    }

    private void PopulateSearchField(IEnumerable<string> locations, string keyword)
    {
        var keywordLower = keyword.ToLower();
        var cssSelectorToFind = $"[aria-label*='Enter your {keywordLower}'] [aria-label*='Done']";

        ClickElement($"[data-placeholder*='Where {(keyword == "Origin" ? "from" : "to")}'] input");

        var el = DoUntil(() => FindElement(cssSelectorToFind));
        var parent = (IWebElement) c.jsExecutor.ExecuteScript("return arguments[0].parentElement.parentElement", el);
        var displayStyle = (string) c.jsExecutor.ExecuteScript("return arguments[0].style.display", parent);
        if (displayStyle == "none")
        {
            ClickElement($"[aria-label='{keyword}, Select multiple airports']");
        }
        else
        {
            DoUntil(() =>
                {
                    var chips = FindElements(
                        $"[aria-label*='Enter your {keywordLower}'] div[role='listbox'] [aria-label='Remove']"
                    );
                    foreach (var chip in chips)
                    {
                        chip.Click();
                    }
                    return true;
                }
            );
        }

        while (FindElements("div[data-code]").Any(x => x.Displayed))
        {
            SendKeysToElement(
                $"[aria-label*='Enter your {keywordLower}'] input",
                Keys.Backspace
            );
        }

        foreach (var location in locations)
        {
            SendKeysToElement(
                $"[aria-label*='Enter your {keywordLower}'] input",
                location
            );
            ClickElement($"[data-code='{location}'] input");
        }
        ClickElement(cssSelectorToFind);
    }

    private void PopulateDateAndHitDone(DateTime date)
    {
        const string format = "ddd, MMM dd";

        ClickElement("[aria-label='Departure']");

        SendKeysToElement(
            "[data-same-day-selection] [aria-label='Departure']",
            date.ToString(format) + Keys.Return
        );

        ClickElement("[aria-label^='Done. Search for one-way flights']");

        if (!stopsSet)
        {
            ClickElement("[aria-label='Search']");
        }
        WaitForProgressBarToBeGone();
    }

    private void SetUpSearch()
    {
        c.driver.Navigate().GoToUrl("https://www.google.com/travel/flights?curr=GBP");
        try
        {
            ClickElement("[aria-label='Reject all']");
        }
        catch (WebDriverTimeoutException e)
        {
            if (e.InnerException is not NoSuchElementException)
            {
                throw;
            }
        }
        SetToOneWayTrip();
    }

    private void SetToOneWayTrip()
    {
        DoUntil(() =>
            {
                var el = FindElement("[aria-label*='Change ticket type']");
                var parentEl = (IWebElement) c.jsExecutor.ExecuteScript("return arguments[0].parentElement", el);
                parentEl.Click();
                return true;
            }
        );
        ClickElement("[aria-label*='Select your ticket type'] [data-value='2']");
    }
}

public class GoogleFlightsWorkerException(JourneyCollection results, Exception innerEx) : Exception(
    "GoogleFlightsWorkerException",
    innerEx
)
{
    public JourneyCollection Results { get; } = results;
}