using Common_ClassLibrary;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public class NationalExpressWorker : IJourneyRetriever
    {
        private JourneyRetrieverComponents C { get; set; }
        private JourneyCollection CollectedJourneys { get; set; }
        private JourneyRetrieverData JourneyRetrieverData { get; set; }
        private int PathsToSearch { get; set; }
        private int PathsCollected { get; set; }
        private string Origin { get; set; }
        private string Destination { get; set; }

        public NationalExpressWorker(JourneyRetrieverComponents c)
        {
            C = c;
        }

        public Task<JourneyCollection> CollectJourneys(JourneyRetrieverData data, DateTime dateFrom, DateTime dateTo, JourneyCollection existingJourneys)
        {
            Initialise(data, existingJourneys);
            SetUpSearch();
            WriteInitialLog(data);
            LoopThroughPathsAndCollectJourneys(data, dateFrom, dateTo);
            return Task.FromResult(CollectedJourneys);
        }

        private void WriteInitialLog(JourneyRetrieverData data)
        {
            foreach (DirectPath directPath in data.DirectPaths) PathsToSearch++;
            C.Log($"Starting search for {PathsToSearch} paths.");
        }

        private void Initialise(JourneyRetrieverData data, JourneyCollection existingJourneys)
        {
            CollectedJourneys = existingJourneys;
            JourneyRetrieverData = data;
            PathsToSearch = 0;
            PathsCollected = 0;
        }

        private void SetUpSearch()
        {
            C.NavigateToUrl("https://book.nationalexpress.com/coach/#/choose-journey");
            C.FindElementByAttributeAndClickIt(By.CssSelector(".fa-close"), indexOfElement: 1);
        }

        private void LoopThroughPathsAndCollectJourneys(JourneyRetrieverData data, DateTime dateFrom, DateTime dateTo)
        {
            foreach (DirectPath directPath in data.DirectPaths) GetPathJourneys(directPath, dateFrom, dateTo);
        }

        private void GetPathJourneys(DirectPath directPath, DateTime dateFrom, DateTime dateTo)
        {
            Origin = directPath.GetStart();
            Destination = directPath.GetEnd();

            string pathName = directPath.ToString();
            C.Log($"Collecting data for {pathName}.");
            C.Log($"Initial population of controls for {pathName}, date {dateFrom}.");
            PopulateControls(Origin, Destination, dateFrom);

            List<DateTime> listOfExtraDates = new() { };
            DateTime tempDate = dateFrom.AddDays(1);
            while (DateTime.Compare(tempDate, dateTo) <= 0)
            {
                listOfExtraDates.Add(tempDate);
                tempDate = tempDate.AddDays(1);
            }
            C.Log($"Getting journeys for {pathName} from {dateFrom} to {dateTo}.");
            CollectedJourneys.AddRange(GetFlightsForDates(dateFrom, listOfExtraDates));
            C.JourneyRetrieverEventHandler.InformOfPathDataFullyCollected(directPath.ToString());
            PathsCollected++;
            C.Log($"Collected data for {pathName} ({Globals.GetPercentageAndCountString(PathsCollected, PathsToSearch)})");
        }

        private JourneyCollection GetFlightsForDates(DateTime date, List<DateTime> extraDates)
        {
            List<Journey> results = new();
            HashSet<string> addedJourneys = new();
            bool allEarlierFlightsRetrieved = false;
            bool allFlightsRetrieved = false;
            int retryCounter = 0;
            while (true)
            {
                IWebElement loadingPage = C.FindElementByAttribute(By.Id("loadingResultPage"));
                C.WebDriverWaitProvider.Until(d => loadingPage.Displayed == false);
                ReadOnlyCollection<IWebElement> journeyGroups = GetJourneyGroups();
                if (journeyGroups.Count == 0)
                {
                    if (retryCounter >= 3)
                    {
                        C.Log($"Couldn't retrieve journeys for path {Origin}-{Destination}.");
                        return new JourneyCollection(results.OrderBy(j => j.ToString()).ToList());
                    }

                    ClickFindJourney();
                    retryCounter++;
                    continue;
                }
                retryCounter = 0;

                journeyGroups = GetJourneyGroups();
                foreach (IWebElement journeyGroup in journeyGroups)
                {
                    IWebElement groupDateElement = C.FindElementByAttribute(By.XPath(".."), container: journeyGroup);
                    string groupDateText = C.FindElementByAttribute(By.CssSelector("h5"), container: groupDateElement).GetAttribute("innerText").Trim();
                    DateTime departing = DateTime.Parse(groupDateText);
                    DateTime lastDate = extraDates.Count == 0 ? date : extraDates[extraDates.Count - 1];
                    int dateComparison = departing.CompareTo(lastDate);
                    if (allEarlierFlightsRetrieved && dateComparison == 1)
                    {
                        allFlightsRetrieved = true;
                        break;
                    }
                    else if (!allEarlierFlightsRetrieved && departing.CompareTo(date) == -1)
                    {
                        allEarlierFlightsRetrieved = true;
                        continue;
                    }

                    string journeysText = journeyGroup.GetAttribute("innerText").Trim();
                    string[] journeysTextLines = journeysText.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                    List<string> journeysTextLinesList = new(journeysTextLines);

                    const string timeRegex = @"^(\d\d):(\d\d)";
                    while (journeysTextLinesList.Count > 0)
                    {
                        string departingTime = GetFirstMatchFromLinesOfTextWhileRemovingLines(journeysTextLinesList, timeRegex);
                        if (departingTime == null) break;

                        DateTime updatedDeparting = departing.AddHours(int.Parse(Regex.Match(departingTime, timeRegex).Groups[1].Value));
                        updatedDeparting = updatedDeparting.AddMinutes(int.Parse(Regex.Match(departingTime, timeRegex).Groups[2].Value));

                        string durationText = GetFirstMatchFromLinesOfTextWhileRemovingLines(journeysTextLinesList, @"^\d+h \d+m");
                        if (durationText == null) break;

                        durationText = Regex.Match(durationText, "(\\d+)\\D+(\\d+).*").Success
                                    ? Regex.Replace(durationText, "(\\d+).*?(\\d+).*", "$1:$2").Trim()
                                    : Regex.Match(durationText, "(\\d+).*hr").Success
                                    ? Regex.Replace(durationText, "(\\d+).*", "$1:00").Trim()
                                    : Regex.Replace(durationText, "(\\d+).*", "0:$1").Trim();
                        TimeSpan duration = TimeSpan.Parse(durationText);
                        DateTime arriving = updatedDeparting + duration;

                        string costText = GetFirstMatchFromLinesOfTextWhileRemovingLines(journeysTextLinesList, @"\d+\.\d\d");
                        if (costText == null) break;

                        int cost = Convert.ToInt32(double.Parse(costText));
                        Journey journey = new(updatedDeparting, arriving, "National Express", duration, $"{Origin}-{Destination}", double.Parse(costText), GetRetrieverName());
                        if (!addedJourneys.Contains(journey.ToString()))
                        {
                            results.Add(journey);
                            addedJourneys.Add(journey.ToString());
                        }
                    }
                }
                if (allFlightsRetrieved) return new JourneyCollection(results.OrderBy(j => j.ToString()).ToList());
                C.FindElementByAttributeAndClickIt(By.CssSelector(".nx-earlier-later-journey"), indexOfElement: allEarlierFlightsRetrieved ? 1 : 0);
            }
        }

        private ReadOnlyCollection<IWebElement> GetJourneyGroups()
        {
            return C.FindElementsNew(By.CssSelector(".nx-leaving-section.ng-star-inserted"));
        }

        private static string GetFirstMatchFromLinesOfTextWhileRemovingLines(List<string> journeysTextLinesList, string pattern)
        {
            while (journeysTextLinesList.Count > 0)
            {
                Match match = Regex.Match(journeysTextLinesList[0], pattern);
                journeysTextLinesList.RemoveAt(0);
                if (match.Success) return match.Value;
            }
            return null;
        }

        private void PopulateControls(string origin, string target, DateTime date)
        {
            if (PathsCollected > 0) ClickChangeJourneyButton();
            InputLocation(origin, 0);
            InputLocation(target, 1);
            PopulateDateAndHitDone(date);
        }

        private void ClickChangeJourneyButton()
        {
            C.FindElementByAttributeAndClickIt(By.Id("editMyJourney"));
        }

        private void InputLocation(string location, int popupIndex)
        {
            By selector = By.CssSelector(popupIndex == 0 ? "#nx-from-station input" : "#nx-to-station input");
            string translatedLocation = JourneyRetrieverData.GetTranslation(location);
            C.FindElementByAttributeAndClickIt(selector);
            C.FindElementByAttributeAndSendKeysToIt(selector, keys: new() { translatedLocation });
            C.FindElementByAttributeAndClickIt(By.CssSelector("li"), text: translatedLocation);
        }

        private void PopulateDateAndHitDone(DateTime date)
        {
            By selector = By.CssSelector(".nx-date-input");
            IWebElement dateInput = C.FindElementByAttribute(selector);
            string dateInInputText = dateInput.GetAttribute("innerText");
            DateTime dateInInput = DateTime.ParseExact(dateInInputText, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            bool dateIsInputIsLaterThanTarget = dateInInput.CompareTo(date) == 1;
            C.FindElementByAttributeAndClickIt(selector);

            IWebElement calendar = C.FindElementByAttribute(By.CssSelector("mat-calendar"));
            IWebElement monthElement = C.FindElementByAttribute(By.CssSelector("calendar-header"), container: calendar);

            string monthAndYearText = monthElement.GetAttribute("innerText").Trim();
            string monthText = Regex.Match(monthAndYearText, @"^(\w+)").Groups[1].Value;

            while (!monthText.Equals(date.ToString("MMMM")))
            {
                C.FindElementByAttributeAndClickIt(By.CssSelector("button"), container: calendar, indexOfElement: dateIsInputIsLaterThanTarget ? 0 : 1);
                monthElement = C.FindElementByAttribute(By.CssSelector("calendar-header"), container: calendar);
                monthAndYearText = monthElement.GetAttribute("innerText").Trim();
                monthText = Regex.Match(monthAndYearText, @"^(\w+)").Groups[1].Value;
            }

            C.FindElementByAttributeAndClickIt(By.CssSelector(".mat-calendar-body-cell"), container: calendar, text: date.Day.ToString());
            PickCalendarTimes();
            ClickFindJourney();
        }

        private void PickCalendarTimes()
        {
            C.FindElementByAttributeAndClickIt(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(1) > nx-time-condition > select"));
            C.FindElementByAttributeAndClickIt(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(1) > nx-time-condition > select > option:nth-child(1)"));
            C.FindElementByAttributeAndClickIt(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(2) > nx-time > select"));
            C.FindElementByAttributeAndClickIt(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(2) > nx-time > select > option:nth-child(1)"));
        }

        private void ClickFindJourney()
        {
            C.FindElementByAttributeAndClickIt(By.Id("nx-find-journey-button"));
        }

        public string GetRetrieverName()
        {
            return nameof(NationalExpressWorker);
        }
    }
}