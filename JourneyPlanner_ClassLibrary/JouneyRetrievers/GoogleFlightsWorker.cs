using Common_ClassLibrary;
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
        private int PathsToSearch { get; set; }
        private int PathsCollected { get; set; }
        private JourneyCollection CollectedJourneys { get; set; }
        private IWebElement OriginInput1 { get; set; }
        private IWebElement OriginInput2 { get; set; }
        private IWebElement DestinationInput1 { get; set; }
        private IWebElement DestinationInput2 { get; set; }
        private IWebElement DateInput1 { get; set; }
        private IWebElement DateInput2 { get; set; }
        private bool ControlsKnown { get; set; }
        private bool StopsSet { get; set; }
        private string LastTypedOrigin { get; set; }

        public GoogleFlightsWorker(JourneyRetrieverComponents c)
        {
            C = c;
        }

        public async Task<JourneyCollection> CollectJourneys(JourneyRetrieverData data, DateTime dateFrom, DateTime dateTo, JourneyCollection existingJourneys)
        {
            CollectedJourneys = existingJourneys;
            JourneyRetrieverData = data;
            PathsToSearch = 0;
            PathsCollected = 0;
            LastTypedOrigin = "";
            StopsSet = false;

            C.NavigateToUrl("https://www.google.com/travel/flights");
            await AgreeToConsent();
            await SetToOneWayTrip();

            foreach (DirectPath directPath in data.DirectPaths) PathsToSearch++;
            Log($"Starting search for {PathsToSearch} paths.");

            foreach (DirectPath directPath in data.DirectPaths) await GetPathJourneys(directPath, dateFrom, dateTo);
            return CollectedJourneys;
        }

        private void Log(string m)
        {
            C.Log(m);
        }

        private async Task Delay(int m)
        {
            await C.Delay(m);
        }

        private async Task GetPathJourneys(DirectPath directPath, DateTime dateFrom, DateTime dateTo)
        {
            string origin = directPath.GetStart();
            string target = directPath.GetEnd();

            string pathName = directPath.ToString();
            Log($"Collecting data for {pathName}.");
            Log($"Initial population of controls for {pathName}, date {dateFrom}.");
            await PopulateControls(origin, target, dateFrom);
            if (!StopsSet) await SetStopsToNone();

            List<DateTime> listOfExtraDates = new() { };
            DateTime tempDate = dateFrom.AddDays(1);
            while (DateTime.Compare(tempDate, dateTo) <= 0)
            {
                listOfExtraDates.Add(tempDate);
                tempDate = tempDate.AddDays(1);
            }
            Log($"Getting flights for {pathName} from {dateFrom} to {dateTo}.");
            CollectedJourneys.AddRange(await GetFlightsForDates(dateFrom, listOfExtraDates));
            C.JourneyRetrieverEventHandler.InformOfPathDataFullyCollected(directPath.ToString());
            PathsCollected++;
            Log($"Collected data for {pathName} ({Globals.GetPercentageAndCountString(PathsCollected, PathsToSearch)})");
        }

        private async Task<JourneyCollection> GetFlightsForDates(DateTime date, List<DateTime> extraDates)
        {
            List<Journey> results = new();
            await GetFlightsForDate(date, results);
            foreach (DateTime extraDate in extraDates)
            {
                await PopulateDateAndHitDone(extraDate);
                await GetFlightsForDate(extraDate, results);
            }
            return new JourneyCollection(results);
        }

        private async Task GetFlightsForDate(DateTime date, List<Journey> results)
        {
            ReadOnlyCollection<IWebElement> flightLists;
            ReadOnlyCollection<IWebElement> flights;
            List<IWebElement> allFlights;

            while (true)
            {
                try
                {
                    await Delay(2000);
                    flightLists = await C.FindElementsAndWait(By.CssSelector("[role=list]"));
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
                        flights = await C.FindElementsAndWait(flightList, By.CssSelector("[role=listitem]"));
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
                results.Add(item);
            }
        }

        private async Task SetStopsToNone()
        {
            await ClickButtonWithAriaLabelText("Stops");
            IWebElement radioGroup = await C.FindElementAndWait(By.CssSelector("[role=radiogroup]"));
            ReadOnlyCollection<IWebElement> radioGroupChildren = await C.FindElementsAndWait(radioGroup, By.CssSelector("input"));
            await C.ClickAndWait(radioGroupChildren[1]);
            await ClickHeader();
            StopsSet = true;
        }

        private async Task ClickHeader()
        {
            await C.ClickAndWait(await C.FindElementAndWait(By.CssSelector("header")));
        }

        private async Task AgreeToConsent()
        {
            await Delay(1000);
            ReadOnlyCollection<IWebElement> consentButtons = await C.FindElementsAndWait(By.CssSelector("button"));
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
                await C.ClickAndWait(button);
                return;
            }
        }

        private async Task SetToOneWayTrip()
        {
            ReadOnlyCollection<IWebElement> spans = await C.FindElementsAndWait(By.CssSelector("span"));
            foreach (IWebElement span in spans)
            {
                string spanText = span.Text;
                if (!spanText.Equals("Round trip")) continue;
                await C.ClickAndWait(span);
                ReadOnlyCollection<IWebElement> lis = await C.FindElementsAndWait(By.CssSelector("li"));
                foreach (IWebElement li in lis)
                {
                    string liText = li.Text;
                    if (!liText.Equals("One way")) continue;
                    await C.ClickAndWait(li);
                    return;
                }
            }
        }

        private async Task PopulateControls(string origin, string target, DateTime date)
        {
            bool getControlsAgain = false;
            if (!ControlsKnown)
            {
                await GetControls();
                getControlsAgain = true;
            }

            if (!LastTypedOrigin.Equals(target))
            {
                await InputTarget(target);
                await InputOrigin(origin);
            }
            else
            {
                await InputOrigin(origin);
                await InputTarget(target);
            }
            LastTypedOrigin = origin;

            await PopulateDateAndHitDone(date);
            if (getControlsAgain) await GetControls();
            ControlsKnown = true;
        }

        private async Task InputOrigin(string origin)
        {
            await C.ClickAndWait(OriginInput1);
            OriginInput2.Clear();
            OriginInput2.SendKeys(JourneyRetrieverData.GetTranslation(origin));
            OriginInput2.SendKeys(Keys.Return);
        }

        private async Task InputTarget(string target)
        {
            await C.ClickAndWait(DestinationInput1);
            DestinationInput2.Clear();
            DestinationInput2.SendKeys(JourneyRetrieverData.GetTranslation(target));
            DestinationInput2.SendKeys(Keys.Return);
        }

        private async Task GetControls()
        {
            ReadOnlyCollection<IWebElement> inputs = await C.FindElementsAndWait(By.CssSelector("input"));
            OriginInput1 = inputs[0];
            OriginInput2 = inputs[1];
            DestinationInput1 = inputs[2];
            DestinationInput2 = inputs[3];
            DateInput1 = inputs[4];
            DateInput2 = inputs[6];
        }

        private async Task PopulateDateAndHitDone(DateTime date)
        {
            await C.ClickAndWait(DateInput1);
            const string format = "ddd, MMM dd";
            foreach (char ch in format) DateInput2.SendKeys(Keys.Backspace);
            DateInput2.SendKeys(date.ToString(format));
            DateInput2.SendKeys(Keys.Return);
            await ClickButtonWithAriaLabelText("Done. Search for");
        }

        private async Task ClickButtonWithAriaLabelText(string text)
        {
            ReadOnlyCollection<IWebElement> buttons = await C.FindElementsAndWait(By.CssSelector("button"));
            foreach (IWebElement button in buttons)
            {
                string buttonText = button.GetAttribute("aria-label");
                if (buttonText == null || !buttonText.Contains(text)) continue;
                await C.ClickAndWait(button);
                return;
            }
        }
    }
}