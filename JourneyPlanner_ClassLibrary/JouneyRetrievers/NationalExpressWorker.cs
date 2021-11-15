using Common_ClassLibrary;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
        private IWebElement OriginInput { get; set; }
        private IWebElement DestinationInput { get; set; }
        private IWebElement DateInput { get; set; }
        private string Origin { get; set; }
        private string Destination { get; set; }

        public NationalExpressWorker(JourneyRetrieverComponents c)
        {
            C = c;
        }

        public async Task<JourneyCollection> CollectJourneys(JourneyRetrieverData data, DateTime dateFrom, DateTime dateTo, JourneyCollection existingJourneys)
        {
            CollectedJourneys = existingJourneys;
            JourneyRetrieverData = data;
            PathsToSearch = 0;
            PathsCollected = 0;

            C.NavigateToUrl("https://book.nationalexpress.com/coach/#/choose-journey");

            ReadOnlyCollection<IWebElement> cookieButtons = C.Driver.FindElements(By.CssSelector(".fa-close"));
            if (cookieButtons.Count == 2) cookieButtons[1].Click();

            foreach (DirectPath directPath in data.DirectPaths) PathsToSearch++;
            C.Log($"Starting search for {PathsToSearch} paths.");

            foreach (DirectPath directPath in data.DirectPaths) await GetPathJourneys(directPath, dateFrom, dateTo);
            return CollectedJourneys;
        }

        private async Task GetPathJourneys(DirectPath directPath, DateTime dateFrom, DateTime dateTo)
        {
            Origin = directPath.GetStart();
            Destination = directPath.GetEnd();

            string pathName = directPath.ToString();
            C.Log($"Collecting data for {pathName}.");
            C.Log($"Initial population of controls for {pathName}, date {dateFrom}.");
            await PopulateControls(Origin, Destination, dateFrom);

            List<DateTime> listOfExtraDates = new() { };
            DateTime tempDate = dateFrom.AddDays(1);
            while (DateTime.Compare(tempDate, dateTo) <= 0)
            {
                listOfExtraDates.Add(tempDate);
                tempDate = tempDate.AddDays(1);
            }
            C.Log($"Getting journeys for {pathName} from {dateFrom} to {dateTo}.");
            CollectedJourneys.AddRange(await GetFlightsForDates(dateFrom, listOfExtraDates));
            C.JourneyRetrieverEventHandler.InformOfPathDataFullyCollected(directPath.ToString());
            PathsCollected++;
            C.Log($"Collected data for {pathName} ({Globals.GetPercentageAndCountString(PathsCollected, PathsToSearch)})");
        }

        private async Task<JourneyCollection> GetFlightsForDates(DateTime date, List<DateTime> extraDates)
        {
            List<Journey> results = new();
            HashSet<string> addedJourneys = new();
            bool allEarlierFlightsRetrieved = false;
            bool allFlightsRetrieved = false;
            while (true)
            {
                await WaitUntilContinueButtonIsAvailable();

                ReadOnlyCollection<IWebElement> journeyGroups;
                try
                {
                    journeyGroups = C.Driver.FindElements(By.CssSelector(".nx-leaving-section.ng-star-inserted"));
                }
                catch (NoSuchElementException)
                {
                    return new JourneyCollection(results);
                }

                foreach (IWebElement journeyGroup in journeyGroups)
                {
                    string groupDateText = journeyGroup.FindElement(By.XPath("..")).FindElement(By.CssSelector("h5")).GetAttribute("innerText").Trim();
                    DateTime departing = DateTime.Parse(groupDateText);
                    if (departing.CompareTo(extraDates[extraDates.Count - 1]) == 1)
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
                        Journey journey = new(updatedDeparting, arriving, "National Express", duration, $"{Origin}-{Destination}", Convert.ToInt32(double.Parse(costText)), GetRetrieverName());
                        if (!addedJourneys.Contains(journey.ToString()))
                        {
                            results.Add(journey);
                            addedJourneys.Add(journey.ToString());
                        }
                    }
                }
                if (allFlightsRetrieved) return new JourneyCollection(results);

                PressEarlierOrLaterCoachesButton(allEarlierFlightsRetrieved);
            }
        }

        private void PressEarlierOrLaterCoachesButton(bool allEarlierFlightsRetrieved)
        {
            while (true)
            {
                try
                {
                    ReadOnlyCollection<IWebElement> expandButtons = C.Driver.FindElements(By.CssSelector(".nx-earlier-later-journey"));
                    foreach (IWebElement expandButton in expandButtons)
                    {
                        string text = expandButton.GetAttribute("innerText");
                        if ((text.ToLower().Contains("later") && allEarlierFlightsRetrieved) || (text.ToLower().Contains("earlier") && !allEarlierFlightsRetrieved))
                        {
                            expandButton.Click();
                            return;
                        }
                    }
                }
                catch (Exception)
                {
                    //try again
                }
            }
        }

        private async Task WaitUntilContinueButtonIsAvailable()
        {
            while (C.Driver.FindElement(By.Id("loadingResultPage")).Displayed) { }
            await C.Delay(1000);
        }

        private string GetFirstMatchFromLinesOfTextWhileRemovingLines(List<string> journeysTextLinesList, string pattern)
        {
            while (journeysTextLinesList.Count > 0)
            {
                Match match = Regex.Match(journeysTextLinesList[0], pattern);
                journeysTextLinesList.RemoveAt(0);
                if (match.Success) return match.Value;
            }
            return null;
        }

        private async Task PopulateControls(string origin, string target, DateTime date)
        {
            if (OriginInput != null) await PressShowJourneyOptions();
            GetControls();
            await InputLocation(origin, 0);
            await InputLocation(target, 1);
            await PopulateDateAndHitDone(date);
        }

        private async Task PressShowJourneyOptions()
        {
            while (true)
            {
                try
                {
                    ReadOnlyCollection<IWebElement> showControlButtons = C.Driver.FindElements(By.CssSelector("strong"));
                    foreach (IWebElement showControlButton in showControlButtons)
                    {
                        if (showControlButton.GetAttribute("innerText").Contains("Change Journey"))
                        {
                            await C.ClickAndWait(showControlButton);
                            return;
                        }
                        else if (showControlButton.GetAttribute("innerText").Contains("Hide Journey"))
                        {
                            return;
                        }
                    }
                }
                catch (Exception)
                {
                    //try again
                }
            }
        }

        private async Task InputLocation(string origin, int popupIndex)
        {
            IWebElement input = popupIndex == 0 ? OriginInput : DestinationInput;
            input.Click();
            input.Clear();
            input.SendKeys(JourneyRetrieverData.GetTranslation(origin));
            await C.Delay(1000);

            while (true)
            {
                try
                {
                    ReadOnlyCollection<IWebElement> popups = C.Driver.FindElements(By.CssSelector("app-station-results"));
                    IWebElement firstOption = popups[popupIndex].FindElement(By.CssSelector("li"));
                    firstOption.Click();
                    break;
                }
                catch (Exception)
                {
                    //try again
                }
            }
        }

        private void GetControls()
        {
            ReadOnlyCollection<IWebElement> fromElements = C.Driver.FindElements(By.Id("nx-from-station"));
            OriginInput = fromElements[1];
            ReadOnlyCollection<IWebElement> toElements = C.Driver.FindElements(By.Id("nx-to-station"));
            DestinationInput = toElements[1];
            DateInput = C.Driver.FindElement(By.CssSelector(".nx-date-input"));
        }

        private async Task PopulateDateAndHitDone(DateTime date)
        {
            string dateInInputText = DateInput.GetAttribute("innerText");
            DateTime dateInInput = DateTime.ParseExact(dateInInputText, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            bool dateIsInputIsLaterThanTarget = dateInInput.CompareTo(date) == 1;
            await C.ClickAndWait(DateInput);

            IWebElement calendar = C.Driver.FindElement(By.CssSelector("mat-calendar"));
            IWebElement monthElement = calendar.FindElement(By.CssSelector("calendar-header"));

            ReadOnlyCollection<IWebElement> monthButtons = calendar.FindElements(By.CssSelector("button"));
            IWebElement changeMonthButton;

            string monthAndYearText = monthElement.GetAttribute("innerText").Trim();
            string monthText = Regex.Match(monthAndYearText, @"^(\w+)").Groups[1].Value;

            while (!monthText.Equals(date.ToString("MMMM")))
            {
                changeMonthButton = dateIsInputIsLaterThanTarget ? monthButtons[0] : monthButtons[1];
                await C.ClickAndWait(changeMonthButton);
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
                    await C.ClickAndWait(dateElement);
                    break;
                }
            }

            IWebElement dropdownTimeCondition = C.Driver.FindElement(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(1) > nx-time-condition > select"));
            await C.ClickAndWait(dropdownTimeCondition);

            IWebElement firstOptionTimeCondition = C.Driver.FindElement(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(1) > nx-time-condition > select > option:nth-child(1)"));
            await C.ClickAndWait(firstOptionTimeCondition);

            IWebElement dropdownTime = C.Driver.FindElement(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(2) > nx-time > select"));
            await C.ClickAndWait(dropdownTime);

            IWebElement firstOptionTime = C.Driver.FindElement(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(2) > nx-time > select > option:nth-child(1)"));
            await C.ClickAndWait(firstOptionTime);

            IWebElement submit = C.Driver.FindElement(By.Id("nx-find-journey-button"));
            await C.ClickAndWait(submit);
        }

        public string GetRetrieverName()
        {
            return nameof(NationalExpressWorker);
        }
    }
}