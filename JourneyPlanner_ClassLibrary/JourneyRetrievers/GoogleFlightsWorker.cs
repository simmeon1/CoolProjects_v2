using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Workers;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.JourneyRetrievers;

public class GoogleFlightsWorker
{
    private readonly JourneyRetrieverComponents c;
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
        try
        {
            DoWork(paths, allDates, results);
            return Task.FromResult(new JourneyCollection(results.OrderBy(j => j.ToString()).ToList()));
        }
        catch (Exception ex)
        {
            throw new GoogleFlightsWorkerException(
                new JourneyCollection(results.OrderBy(j => j.ToString()).ToList())
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
        var flights = c.driver.FindElements(ByCssSelector("[role='tabpanel']"))
            .SelectMany(p => p.FindElements(ByCssSelector("li")));

        foreach (var flight in flights)
        {
            var text = flight.GetAttribute("innerText");
            if (text == null)
            {
                c.logger.Log("Missing text for flight. Continuing.");
                continue;
            }
            var flightText = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            if (!flightText[6].Trim().Equals("Nonstop"))
            {
                c.logger.Log("Flight is not nonstop. Continuing.");
                continue;
            }
            var item = GetJourneyFromText(date, flightText);
            if (remainingPathsSet.Contains(item.Path) && !collectedFlightsSet.Contains(item.ToString()))
            {
                results.Add(item);
                collectedFlightsSet.Add(item.ToString());
            }
        }

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
        var success = double.TryParse(costText, out var cost);
        if (!success)
        {
            c.logger.Log("Could not get cost for flight.");
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

    private static int ParseTimeUnit(string durationText, string unitType)
    {
        var regexMatches = Regex.Matches(durationText, @$"(\d+) {unitType}");
        return regexMatches.Any() ? int.Parse(regexMatches[0].Groups[1].Value) : 0;
    }

    private void WaitForProgressBarToBeGone()
    {
        var loadingBar = FindElement("[data-buffervalue='1']");
        try
        {
            c.wait.Until(_ => loadingBar.GetAttribute("aria-hidden") == null, 5);
            c.wait.Until(_ => loadingBar.GetAttribute("aria-hidden") != null);
        }
        catch (Exception)
        {
            //loaded
        }
    }

    private IWebElement FindElement(string cssSelectorToFind)
    {
        return c.driver.FindElement(ByCssSelector(cssSelectorToFind));
    }

    private void SetStopsToNone()
    {
        c.driver.FindElementAndClickIt(ByCssSelector("[aria-label='Stops, Not selected']"));
        Sleep(500);
        var el = FindElement("div[aria-label*='Stops']");
        var p = ByCssSelector("input");
        p.Container = el;
        p.Index = 1;
        c.driver.FindElementAndClickIt(p);
        Sleep();

        c.driver.FindElementAndClickIt(ByCssSelector("[data-filtertype*='10'] [aria-label*='Close dialog']"));
        stopsSet = true;
        WaitForProgressBarToBeGone();
    }

    private void PopulateSearchField(IEnumerable<string> locations, string keyword)
    {
        var keywordLower = keyword.ToLower();

        c.driver.FindElementAndClickIt(
            ByCssSelector($"[data-placeholder*='Where {(keyword == "Origin" ? "from" : "to")}'] input")
        );
        Sleep();

        var checkmarkDoneButtonParam =
            ByCssSelector($"[aria-label*='Enter your {keywordLower}'] [aria-label*='Done']");
        var el = c.driver.FindElement(checkmarkDoneButtonParam);
        el = (IWebElement) c.ExecuteScript("return arguments[0].parentElement.parentElement", el);
        var displayStyle = (string) c.ExecuteScript("return arguments[0].style.display", el);
        if (displayStyle == "none")
        {
            c.driver.FindElementAndClickIt(ByCssSelector($"[aria-label='{keyword}, Select multiple airports']"));
        }
        else
        {
            c.driver.FindElementAndSendKeysToIt(
                ByCssSelector($"[aria-label*='Enter your {keywordLower}'] input"),
                true,
                Keys.Backspace + Keys.Backspace + Keys.Backspace + Keys.Backspace + Keys.Backspace +
                Keys.Backspace + Keys.Backspace
            );
        }

        while (c.driver.FindElements(ByCssSelector("div[data-code]")).Any(x => x.Displayed))
        {
            c.driver.FindElementAndSendKeysToIt(
                ByCssSelector($"[aria-label*='Enter your {keywordLower}'] input"),
                false,
                Keys.Backspace
            );
        }

        foreach (var location in locations)
        {
            c.driver.FindElementAndSendKeysToIt(
                ByCssSelector($"[aria-label*='Enter your {keywordLower}'] input"),
                false,
                location
            );

            c.driver.FindElementAndClickIt(ByCssSelector($"[data-code='{location}'] input"));
            Sleep();
        }

        c.driver.FindElementAndClickIt(checkmarkDoneButtonParam);

        Sleep();
    }

    private void PopulateDateAndHitDone(DateTime date)
    {
        const string format = "ddd, MMM dd";

        c.driver.FindElementAndClickIt(ByCssSelector("[aria-label='Departure']"));

        Sleep();

        c.driver.FindElementAndSendKeysToIt(
            ByCssSelector("[data-same-day-selection] [aria-label='Departure']"),
            false,
            date.ToString(format) + Keys.Return
        );

        c.driver.FindElementAndClickIt(
            ByCssSelector("[aria-label^='Done. Search for one-way flights']")
        );

        if (!stopsSet)
        {
            c.driver.FindElementAndClickIt(
                ByCssSelector("[aria-label='Search']")
            );
        }

        WaitForProgressBarToBeGone();
    }

    private void Sleep(int milliseconds = 200)
    {
        c.Sleep(milliseconds);
    }
    
    private void SetUpSearch()
    {
        c.NavigateToUrl("https://www.google.com/travel/flights?curr=GBP");
        try
        {
            c.driver.FindElementAndClickIt(
                new FindElementParameters
                {
                    BySelector = ByCssSelector("button"),
                    Matcher = x =>
                    {
                        var innerText = x.GetAttribute("innerText");
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
        var param = ByCssSelector("[aria-label*='Change ticket type']");
        var el = c.driver.FindElement(param);
        var parentEl = (IWebElement) c.ExecuteScript("return arguments[0].parentElement", el);
        parentEl.Click();

        c.driver.FindElementAndClickIt(ByCssSelector("[aria-label*='Select your ticket type'] [data-value='2']"));
    }
}

public class GoogleFlightsWorkerException : Exception
{
    public GoogleFlightsWorkerException(JourneyCollection results)
    {
        Results = results;
    }

    public JourneyCollection Results { get; }
}