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

            bool cookieButtonsExist = C.WebDriverWaitProvider.Until(d => d.FindElements(By.CssSelector(".fa-close")).Count == 2);
            ReadOnlyCollection<IWebElement> cookieButtons = C.Driver.FindElements(By.CssSelector(".fa-close"));
            C.ClickElementWhenClickable(cookieButtons[1]);
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
                C.WebDriverWaitProvider.Until(d => d.FindElement(By.Id("loadingResultPage")).Displayed == false);
                ReadOnlyCollection<IWebElement> journeyGroups = C.Driver.FindElements(By.CssSelector(".nx-leaving-section.ng-star-inserted"));
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

                foreach (IWebElement journeyGroup in journeyGroups)
                {
                    string groupDateText = journeyGroup.FindElement(By.XPath("..")).FindElement(By.CssSelector("h5")).GetAttribute("innerText").Trim();
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

                ReadOnlyCollection<IWebElement> earlierLaterCoachesButtons = C.Driver.FindElements(By.CssSelector(".nx-earlier-later-journey"));
                IWebElement buttonToClick = allEarlierFlightsRetrieved ? earlierLaterCoachesButtons[1] : earlierLaterCoachesButtons[0];
                C.ClickElementWhenClickable(buttonToClick);
            }
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
            if (CollectedJourneys.GetCount() > 0) ClickChangeJourneyButton();
            InputLocation(origin, 0);
            InputLocation(target, 1);
            PopulateDateAndHitDone(date);
        }

        private void ClickChangeJourneyButton()
        {
            IWebElement changeJourneyButton = C.Driver.FindElement(By.Id("editMyJourney"));
            C.ClickElementWhenClickable(changeJourneyButton);
        }

        private void InputLocation(string location, int popupIndex)
        {
            By selector = By.CssSelector(popupIndex == 0 ? "#nx-from-station input" : "#nx-to-station input");
            IWebElement input = C.WebDriverWaitProvider.Until(ExpectedConditions.ElementToBeClickable(selector));
            C.ClickElementWhenClickable(input);
            input.Clear();
            string translatedLocation = JourneyRetrieverData.GetTranslation(location);
            input.SendKeys(translatedLocation);
            C.WebDriverWaitProvider.Until(d => d.FindElements(By.CssSelector("app-station-results")).Count == popupIndex + 1);
            AttemptToClickFirstOptionOfVisibleDropdown(translatedLocation, popupIndex);
        }

        private void AttemptToClickFirstOptionOfVisibleDropdown(string translatedLocation, int popupIndex)
        {
            while (true)
            {
                try
                {
                    ReadOnlyCollection<IWebElement> locationDropdowns = C.Driver.FindElements(By.CssSelector("app-station-results"));
                    IWebElement dropdown = locationDropdowns[popupIndex];
                    if (!dropdown.Displayed) break;
                    IWebElement firstResult = C.WebDriverWaitProvider.Until(d => dropdown.FindElement(By.CssSelector("li")));
                    if (!firstResult.GetAttribute("innerText").ToLower().Contains(translatedLocation.ToLower())) throw new Exception("Wait more.");
                    C.ClickElementWhenClickable(firstResult);
                    if (!dropdown.Displayed) break;
                }
                catch (Exception ex)
                {
                    //try again
                }
            }
        }

        private void PopulateDateAndHitDone(DateTime date)
        {
            IWebElement dateInput = C.Driver.FindElement(By.CssSelector(".nx-date-input"));
            string dateInInputText = dateInput.GetAttribute("innerText");
            DateTime dateInInput = DateTime.ParseExact(dateInInputText, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            bool dateIsInputIsLaterThanTarget = dateInInput.CompareTo(date) == 1;
            C.ClickElementWhenClickable(dateInput);

            IWebElement calendar = C.Driver.FindElement(By.CssSelector("mat-calendar"));
            IWebElement monthElement = calendar.FindElement(By.CssSelector("calendar-header"));

            ReadOnlyCollection<IWebElement> monthButtons = calendar.FindElements(By.CssSelector("button"));
            IWebElement changeMonthButton;

            string monthAndYearText = monthElement.GetAttribute("innerText").Trim();
            string monthText = Regex.Match(monthAndYearText, @"^(\w+)").Groups[1].Value;

            while (!monthText.Equals(date.ToString("MMMM")))
            {
                changeMonthButton = dateIsInputIsLaterThanTarget ? monthButtons[0] : monthButtons[1];
                C.ClickElementWhenClickable(changeMonthButton);
                monthElement = calendar.FindElement(By.CssSelector("calendar-header"));
                monthAndYearText = monthElement.GetAttribute("innerText").Trim();
                monthText = Regex.Match(monthAndYearText, @"^(\w+)").Groups[1].Value;
            }

            ReadOnlyCollection<IWebElement> dates = calendar.FindElements(By.CssSelector(".mat-calendar-body-cell"));
            foreach (IWebElement dateElement in dates)
            {
                string day = dateElement.GetAttribute("innerText").Trim();
                if (int.Parse(day) == date.Day)
                {
                    C.ClickElementWhenClickable(dateElement);
                    break;
                }
            }

            PickCalendarTimes();
            ClickFindJourney();
        }

        private void PickCalendarTimes()
        {
            IWebElement dropdownTimeCondition = C.Driver.FindElement(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(1) > nx-time-condition > select"));
            C.ClickElementWhenClickable(dropdownTimeCondition);

            IWebElement firstOptionTimeCondition = C.Driver.FindElement(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(1) > nx-time-condition > select > option:nth-child(1)"));
            C.ClickElementWhenClickable(firstOptionTimeCondition);

            IWebElement dropdownTime = C.Driver.FindElement(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(2) > nx-time > select"));
            C.ClickElementWhenClickable(dropdownTime);

            IWebElement firstOptionTime = C.Driver.FindElement(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(2) > nx-time > select > option:nth-child(1)"));
            C.ClickElementWhenClickable(firstOptionTime);
        }

        private void ClickFindJourney()
        {
            IWebElement submit = C.WebDriverWaitProvider.Until(ExpectedConditions.ElementToBeClickable(By.Id("nx-find-journey-button")));
            C.ClickElementWhenClickable(submit);
        }

        public string GetRetrieverName()
        {
            return nameof(NationalExpressWorker);
        }
    }
}