using OpenQA.Selenium;
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
        private JourneyRetrieverData JourneyRetrieverData { get; set; }

        public NationalExpressWorker(JourneyRetrieverComponents c)
        {
            C = c;
        }

        public void Initialise(JourneyRetrieverData data)
        {
            JourneyRetrieverData = data;
            SetUpSearch();
        }

        private void SetUpSearch()
        {
            C.NavigateToUrl("https://book.nationalexpress.com/coach/#/choose-journey");
            C.FindElementByAttributeAndClickIt(By.CssSelector(".fa-close"), indexOfElement: 1);
        }

        public async Task<JourneyCollection> GetJourneysForDates(string origin, string destination, List<DateTime> allDates)
        {
            await PopulateControlsAndSearch(origin, destination, allDates[0]);
            List<Journey> results = new();
            HashSet<string> addedJourneys = new();
            bool allEarlierFlightsRetrieved = false;
            bool allFlightsRetrieved = false;
            int retryCounter = 0;
            while (true)
            {
                while (true)
                {
                    IWebElement loadingPage = C.FindElementByAttribute(By.CssSelector(".hidden"));
                    if (!C.Driver.Url.Contains("session-expired")) break;
                    C.FindElementByAttributeAndClickIt(By.CssSelector(".nx-error-button"));
                }

                ReadOnlyCollection<IWebElement> journeyGroups = GetJourneyGroups();
                if (journeyGroups.Count == 0)
                {
                    if (retryCounter >= 1)
                    {
                        C.Log($"Couldn't retrieve journeys for path {origin}-{destination}.");
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
                    DateTime lastDate = allDates[allDates.Count - 1];
                    int dateComparison = departing.CompareTo(lastDate);
                    if (allEarlierFlightsRetrieved && dateComparison == 1)
                    {
                        allFlightsRetrieved = true;
                        break;
                    }
                    else if (!allEarlierFlightsRetrieved && departing.CompareTo(allDates[0]) == -1)
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
                        Journey journey = new(updatedDeparting, arriving, "National Express", duration, $"{origin}-{destination}", double.Parse(costText), nameof(NationalExpressWorker));
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
            try
            {
                C.WebDriverWaitProvider.Until(d => C.FindElementsNew(By.CssSelector(".nx-leaving-section.ng-star-inserted")).Count > 0, 2);
                return C.FindElementsNew(By.CssSelector(".nx-leaving-section.ng-star-inserted"));
            }
            catch (Exception)
            {
                return new ReadOnlyCollection<IWebElement>(new List<IWebElement>());
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

        private async Task PopulateControlsAndSearch(string origin, string destination, DateTime date)
        {
            if (C.FindElementByAttribute(By.Id("nx-expandable-journey-search")).GetAttribute("hidden") != null) ClickChangeJourneyButton();
            await InputLocation(origin, 0);
            await InputLocation(destination, 1);
            PopulateDateAndSearch(date);
        }

        private void ClickChangeJourneyButton()
        {
            C.FindElementByAttributeAndClickIt(By.Id("editMyJourney"));
        }

        private async Task InputLocation(string location, int popupIndex)
        {
            By selector = By.CssSelector(popupIndex == 0 ? "#nx-from-station input" : "#nx-to-station input");
            string translatedLocation = JourneyRetrieverData.GetTranslation(location);
            C.FindElementByAttributeAndClickIt(selector);
            
            C.FindElementByAttributeAndSendKeysToIt(selector, keys: new() { translatedLocation });
            await C.Delayer.Delay(1000);
            WaitUntilOneStopIsShownForPopup(popupIndex);
            while (!PopupsHidden(popupIndex))
            {
                try
                {
                    IWebElement firstLi = C.FindElementByAttribute(By.CssSelector("li"), text: translatedLocation);
                    firstLi.Click();
                    await C.Delayer.Delay(1000);
                }
                catch (Exception ex)
                {
                    //Incorrect li, retrieve again
                }
            }
        }

        private void WaitUntilOneStopIsShownForPopup(int popupIndex)
        {
            while (true)
            {
                try
                {
                    IWebElement infoMessage = C.FindElementsNew(By.CssSelector(".nx-info"))[popupIndex];
                    if (infoMessage.GetAttribute("innerText").Contains("1 stops matching your search")) return;
                }
                catch (Exception)
                {
                    //try again
                }
            }
        }

        private bool PopupsHidden(int popupIndex)
        {
            ReadOnlyCollection<IWebElement> popups = C.FindElementsNew(By.CssSelector("app-station-popup"));
            IWebElement popupToQuery = popups[popupIndex];
            string isHidden = popupToQuery.GetAttribute("hidden");
            return isHidden == null ? false : true;
        }

        private void PopulateDateAndSearch(DateTime date)
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
    }
}