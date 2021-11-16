using Common_ClassLibrary;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace JourneyPlanner_ClassLibrary
{
    public class GoogleFlightsWorker : IJourneyRetriever
    {
        private JourneyRetrieverComponents C { get; set; }
        private JourneyRetrieverData JourneyRetrieverData { get; set; }
        private int PathsToSearch { get; set; }
        private int PathsCollected { get; set; }
        private JourneyCollection CollectedJourneys { get; set; }
        private IWebElement OriginInput1 { get; set; }
        private IWebElement OriginInput2 { get; set; }
        private IWebElement DestinationInput1 { get; set; }
        private IWebElement DestinationInput2 { get; set; }
        private IWebElement DateInput1 { get; set; }
        private IWebElement DateInput2 { get; set; }
        private bool StopsSet { get; set; }
        private string LastTypedOrigin { get; set; }

        public GoogleFlightsWorker(JourneyRetrieverComponents c)
        {
            C = c;
        }

        public JourneyCollection CollectJourneys(JourneyRetrieverData data, DateTime dateFrom, DateTime dateTo, JourneyCollection existingJourneys)
        {
            CollectedJourneys = existingJourneys;
            JourneyRetrieverData = data;
            PathsToSearch = 0;
            PathsCollected = 0;
            LastTypedOrigin = "";
            StopsSet = false;

            C.NavigateToUrl("https://www.google.com/travel/flights");

            SetToOneWayTrip();

            foreach (DirectPath directPath in data.DirectPaths) PathsToSearch++;
            Log($"Starting search for {PathsToSearch} paths.");

            foreach (DirectPath directPath in data.DirectPaths) GetPathJourneys(directPath, dateFrom, dateTo);
            return CollectedJourneys;
        }

        private void SetToOneWayTrip()
        {
            C.FindElementWithAttribute(By.CssSelector("button"), text: "I agree");
            C.FindElementWithAttribute(By.CssSelector("span"), text: "Round trip");
            C.FindElementWithAttribute(By.CssSelector("li"), text: "One way");
        }

        private void Log(string m)
        {
            C.Log(m);
        }

        private void GetPathJourneys(DirectPath directPath, DateTime dateFrom, DateTime dateTo)
        {
            string origin = directPath.GetStart();
            string target = directPath.GetEnd();

            string pathName = directPath.ToString();
            Log($"Collecting data for {pathName}.");
            Log($"Initial population of controls for {pathName}, date {dateFrom}.");
            PopulateControls(origin, target, dateFrom);
            if (!StopsSet) SetStopsToNone();

            List<DateTime> listOfExtraDates = new() { };
            DateTime tempDate = dateFrom.AddDays(1);
            while (DateTime.Compare(tempDate, dateTo) <= 0)
            {
                listOfExtraDates.Add(tempDate);
                tempDate = tempDate.AddDays(1);
            }
            Log($"Getting flights for {pathName} from {dateFrom} to {dateTo}.");
            CollectedJourneys.AddRange(GetFlightsForDates(dateFrom, listOfExtraDates));
            C.JourneyRetrieverEventHandler.InformOfPathDataFullyCollected(directPath.ToString());
            PathsCollected++;
            Log($"Collected data for {pathName} ({Globals.GetPercentageAndCountString(PathsCollected, PathsToSearch)})");
        }

        private JourneyCollection GetFlightsForDates(DateTime date, List<DateTime> extraDates)
        {
            List<Journey> results = new();
            GetFlightsForDate(date, results);
            foreach (DateTime extraDate in extraDates)
            {
                GetControls();
                PopulateDateAndHitDone(extraDate);
                GetFlightsForDate(extraDate, results);
            }
            return new JourneyCollection(results);
        }

        private void GetFlightsForDate(DateTime date, List<Journey> results)
        {
            ReadOnlyCollection<IWebElement> flightLists;
            ReadOnlyCollection<IWebElement> flights;
            List<IWebElement> allFlights;

            WaitForProgressBarToBeGone();

            while (true)
            {
                try
                {
                    flightLists = C.Driver.FindElements(By.CssSelector("[role=list]"));
                }
                catch (NoSuchElementException)
                {
                    return;
                }

                try
                {
                    allFlights = new();
                    bool showMoreFlightsButtonClicked = false;
                    foreach (IWebElement flightList in flightLists)
                    {
                        flights = flightList.FindElements(By.CssSelector("[role=listitem]"));
                        allFlights.AddRange(flights);
                        if (flights[flights.Count - 1].GetAttribute("innerText").Contains("more flights"))
                        {
                            flights[flights.Count - 1].Click();
                            showMoreFlightsButtonClicked = true;
                            break;
                        }
                    }
                    if (!showMoreFlightsButtonClicked) break;
                }
                catch (StaleElementReferenceException)
                {
                    //Repeat
                }
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
            int.TryParse(costText, out int cost);
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
            IWebElement loadingBar = C.FindElementWithAttribute(By.CssSelector("[role=progressbar]"), clickElement: false);

            try
            {
                C.WebDriverWaitProvider.Until(d => !loadingBar.GetAttribute("aria-hidden").Equals("true"), 1);
                C.WebDriverWaitProvider.Until(d => loadingBar.GetAttribute("aria-hidden").Equals("true"));
            }
            catch (Exception)
            {
                //all loaded
            }
        }

        private void SetStopsToNone()
        {
            C.FindElementWithAttribute(By.CssSelector("button"), text: "Stops");
            C.FindElementWithAttribute(By.CssSelector("label"), text: "Non-stop only");
            C.FindElementWithAttribute(By.CssSelector("header"));
            StopsSet = true;
        }

        private void PopulateControls(string origin, string target, DateTime date)
        {
            GetControls();
            if (!LastTypedOrigin.Equals(target))
            {
                InputTarget(target);
                InputOrigin(origin);
            }
            else
            {
                InputOrigin(origin);
                InputTarget(target);
            }
            LastTypedOrigin = origin;
            PopulateDateAndHitDone(date);
        }

        private void InputOrigin(string origin)
        {
            C.ClickElementWhenClickable(OriginInput1);
            C.ClickElementWhenClickable(OriginInput2);
            OriginInput2.Clear();
            OriginInput2.SendKeys(JourneyRetrieverData.GetTranslation(origin));
            OriginInput2.SendKeys(Keys.Return);
        }

        private void InputTarget(string target)
        {
            C.ClickElementWhenClickable(DestinationInput1);
            C.ClickElementWhenClickable(DestinationInput2);
            DestinationInput2.Clear();
            DestinationInput2.SendKeys(JourneyRetrieverData.GetTranslation(target));
            DestinationInput2.SendKeys(Keys.Return);
        }

        private void GetControls()
        {
            ReadOnlyCollection<IWebElement> inputs = C.Driver.FindElements(By.CssSelector("input"));
            OriginInput1 = inputs[0];
            OriginInput2 = inputs[1];
            DestinationInput1 = inputs[2];
            DestinationInput2 = inputs[3];
            DateInput1 = inputs[4];
            DateInput2 = inputs[6];
        }

        private void PopulateDateAndHitDone(DateTime date)
        {
            const string format = "ddd, MMM dd";
            C.ClickElementWhenClickable(DateInput1);
            C.ClickElementWhenClickable(DateInput2);
            foreach (char ch in format) DateInput2.SendKeys(Keys.Backspace);
            DateInput2.SendKeys(date.ToString(format));
            DateInput2.SendKeys(Keys.Return);
            C.FindElementWithAttribute(By.CssSelector("button"), attribute: "aria-label", text: "Done. Search for");
        }

        public string GetRetrieverName()
        {
            return nameof(GoogleFlightsWorker);
        }
    }
}