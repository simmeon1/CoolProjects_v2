using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public class GoogleFlightsWorker : IJourneyRetriever
    {
        private JourneyRetrieverComponents C { get; set; }
        private JourneyRetrieverData JourneyRetrieverData { get; set; }
        private bool StopsSet { get; set; }
        private string LastTypedOrigin { get; set; }

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
                C.FindElementByAttributeAndClickIt(By.CssSelector("button"), text: "I agree");
            }
            catch (Exception)
            {
                //Doesn't show
            }
            SetToOneWayTrip();
        }

        private void SetToOneWayTrip()
        {
            C.FindElementByAttributeAndClickIt(By.CssSelector("span"), text: "Round trip");
            C.FindElementByAttributeAndClickIt(By.CssSelector("li"), text: "One way");
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
            ReadOnlyCollection<IWebElement> flightLists;
            ReadOnlyCollection<IWebElement> flights;
            List<IWebElement> allFlights;

            while (true)
            {
                try
                {
                    flightLists = C.FindElementsNew(By.CssSelector("[role=list]"));
                }
                catch (NoSuchElementException)
                {
                    return;
                }

                allFlights = new();
                bool showMoreFlightsButtonClicked = false;
                foreach (IWebElement flightList in flightLists)
                {
                    By selector = By.CssSelector("[role=listitem]");
                    flights = C.FindElementsNew(selector, container: flightList);
                    allFlights.AddRange(flights);
                    if (flights[flights.Count - 1].GetAttribute("innerText").Contains("more flights"))
                    {
                        C.FindElementByAttributeAndClickIt(selector, container: flightList, indexOfElement: flights.Count - 1);
                        showMoreFlightsButtonClicked = true;
                        break;
                    }
                }
                if (!showMoreFlightsButtonClicked) break;
            }

            foreach (IWebElement flight in allFlights)
            {
                string text = flight.GetAttribute("innerText");
                if (text.Contains("flights")) continue;
                Journey item = GetJourneyFromText(date, text);
                results.Add(item);
            }
        }

        private static Journey GetJourneyFromText(DateTime date, string text)
        {
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
            IWebElement loadingBar = C.FindElementByAttribute(By.CssSelector("[data-buffervalue='1']"));
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
            C.FindElementByAttributeAndClickIt(By.CssSelector("button"), text: "Stops");
            C.FindElementByAttributeAndClickIt(By.CssSelector("label"), text: "Non-stop only");
            C.FindElementByAttributeAndClickIt(By.CssSelector("header"));
            StopsSet = true;
            WaitForProgressBarToBeGone();
        }

        private void PopulateControlsAndSearch(string origin, string destination, DateTime dateFrom, bool skipLocationInput)
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
            C.FindElementByAttributeAndClickIt(By.CssSelector("input"), indexOfElement: isTarget ? 2 : 0);
            C.FindElementByAttributeAndSendKeysToIt(By.CssSelector("input"), indexOfElement: 3, keys: new() { JourneyRetrieverData.GetTranslation(origin), Keys.Return });
        }

        private void PopulateDateAndHitDone(DateTime date)
        {
            const string format = "ddd, MMM dd";

            By selector = By.CssSelector("[aria-label='Departure date']");
            IWebElement resetButton = C.FindElementByAttribute(By.CssSelector("button"), text: "Reset");
            while (!resetButton.Displayed)
            {
                C.FindElementByAttributeAndClickIt(selector);
                resetButton = C.FindElementByAttribute(By.CssSelector("button"), text: "Reset");
            }

            List<string> keysToSend = new();
            foreach (char ch in format) keysToSend.Add(Keys.Backspace);
            keysToSend.Add(date.ToString(format));
            keysToSend.Add(Keys.Return);
            C.FindElementByAttributeAndSendKeysToIt(selector, indexOfElement: 1, keys: keysToSend, doClearFirst: false);
            C.FindElementByAttributeAndClickIt(By.CssSelector("button"), attribute: "aria-label", text: "Done. Search for");
            WaitForProgressBarToBeGone();
        }
    }
}