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
    private const string PoliteCssSelector = "div[aria-live='polite']";

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
        var flights = DoUntil(
            () =>
            {
                var els = FindElements("[role='tabpanel']:not([style]) li");
                if (els.Count > 0)
                {
                    c.jsExecutor.ExecuteScript(
                        "return arguments[0].setAttribute('alreadyChecked', 'true')",
                        els.First()
                    );
                }
                else
                {
                    // Check for reload
                    var button = FindElements("button").FirstOrDefault(x => x.Text == "Reload");
                    if (button != null)
                    {
                        button.Click();
                        throw new NoSuchElementException("No flights due to reload button");
                    }
                }
                return els // duplicate tabpanel that has displayed: none
                    .Select(f => f.GetAttribute("innerText")!.Split(
                            "\r\n",
                            StringSplitOptions.RemoveEmptyEntries
                        )
                    )
                    .ToList();
            },
            30
        );


        for (var i = 0; i < flights.Count; i++)
        {
            var flight = flights[i];
            // Log for help with diagnosis
            if (i == 0)
            {
                c.logger.Log("First flight on page is " + GetFlightTextFromArray(flight));
            }
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

    private static string TrimCostText(string[] flightText)
    {
        var cost = flightText.FirstOrDefault(t => t.Trim().StartsWith("£")) ?? "";
        return Regex.Replace(cost, "\\D", "").Trim();
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
        var costText = TrimCostText(flightText);
        var success = double.TryParse(costText, out var cost);
        if (!success)
        {
            c.logger.Log(GetFlightTextFromArray(flightText) + " has 0 cost.");
        }
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

    private static string GetFlightTextFromArray(string[] flightText)
    {
        return string.Join(", ", flightText);
    }

    private static int ParseTimeUnit(string durationText, string unitType)
    {
        var regexMatches = Regex.Matches(durationText, @$"(\d+) {unitType}");
        return regexMatches.Any() ? int.Parse(regexMatches[0].Groups[1].Value) : 0;
    }

    private IWebElement FindElement(string cssSelectorToFind, ISearchContext? parentEl = null)
    {
        var searchContext = parentEl ?? c.driver;
        return searchContext.FindElement(ByCssSelector(cssSelectorToFind));
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
        WaitForNewResults();
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

        var cssSelector = $"[aria-label*='Enter your {keywordLower}'] input";
        while (FindElements("div[data-code]").Any(x => x.Displayed))
        {
            SendKeysToElement(cssSelector, Keys.Backspace);
        }

        foreach (var location in locations)
        {
            SendKeysToElement(cssSelector, location);
            ClickElement($"[data-code='{location}'] input");
        }
        ClickElement(cssSelectorToFind);
    }

    private void PopulateDateAndHitDone(DateTime date)
    {
        const string format = "ddd, MMM dd";

        while (true)
        {
            try
            {
                ClickElement("[data-state] [aria-label='Departure']");
                break;
            }
            catch (WebDriverTimeoutException ex)
            {
                if (ex.InnerException is not ElementClickInterceptedException)
                {
                    throw;
                }
                c.logger.Log("Click intercepted. Retrying.");
            }
        }

        SendKeysToElement(
            "[data-same-day-selection] [aria-label='Departure']",
            date.ToString(format) + Keys.Return
        );

        ClickElement("[aria-label^='Done. Search for one-way flights']");

        if (!stopsSet)
        {
            ClickElement("[aria-label='Search']");
            // If polite is visible, results should be loaded.
            DoUntil(() =>
                {
                    var banner = FindElement(PoliteCssSelector);
                    ChangePoliteToImpolite(banner);
                    return true;
                }
            );
        }
        else
        {
            WaitForNewResults();
        }
    }

    private bool WaitForNewResults()
    {
        return DoUntil(
            () =>
            {
                if (FindElements("[alreadyChecked='true']").Count > 0)
                {
                    // c.logger.Log("Has already checked.");
                    return false;
                }
                var main = FindElement("[role='main']");
                var banner = FindElement(PoliteCssSelector, main);
                ChangePoliteToImpolite(banner);
                return true;
            },
            30
        );
    }

    private void ChangePoliteToImpolite(IWebElement banner)
    {
        c.jsExecutor.ExecuteScript("return arguments[0].setAttribute('aria-live', 'impolite')", banner);
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