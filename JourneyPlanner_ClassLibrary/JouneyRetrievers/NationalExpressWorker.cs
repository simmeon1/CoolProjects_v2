﻿using Common_ClassLibrary;
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

            ReadOnlyCollection<IWebElement> cookieButtons = await C.FindElementsAndWait(By.CssSelector(".fa-close"));
            if (cookieButtons.Count == 2) await C.ClickAndWait(cookieButtons[1]);

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
            while (true)
            {
                while (true)
                {
                    try
                    {
                        await C.FindElementAndWait(By.CssSelector("[title=Continue]"));
                        break;
                    }
                    catch (NoSuchElementException)
                    {
                        //try again
                    }
                }

                C.Delay(1000);

                ReadOnlyCollection<IWebElement> journeyElements;
                try
                {
                    journeyElements = await C.FindElementsAndWait(By.CssSelector("app-journey-item"));
                }
                catch (NoSuchElementException)
                {
                    return new JourneyCollection(results);
                }

                bool allFlightsRetrieved = false;
                foreach (IWebElement journeyElement in journeyElements)
                {
                    await C.ClickAndWait(journeyElement);
                    IWebElement journeyInfoElement = await C.FindElementAndWait(By.Id("nx-journey-info"));
                    string journeyInfo = journeyInfoElement.GetAttribute("innerText").Trim();
                    string[] journeyPieces = journeyInfo.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                    DateTime departing = DateTime.Parse(journeyPieces[0]);
                    if (departing.CompareTo(extraDates[extraDates.Count - 1]) == 1)
                    {
                        allFlightsRetrieved = true;
                        break;
                    }

                    string times = journeyPieces[3];
                    Match departingTimeMatch = Regex.Match(times, @"^(\d\d):(\d\d)");
                    string departingTime = departingTimeMatch.Groups[1].Value;
                    departing = departing.AddHours(int.Parse(Regex.Match(times, @"^(\d\d):(\d\d)").Groups[1].Value));
                    departing = departing.AddMinutes(int.Parse(Regex.Match(times, @"^(\d\d):(\d\d)").Groups[2].Value));

                    string durationText = journeyPieces[1];
                    durationText = Regex.Match(durationText, "(\\d+)\\D+(\\d+).*").Success
                        ? Regex.Replace(durationText, "(\\d+).*?(\\d+).*", "$1:$2").Trim()
                        : Regex.Match(durationText, "(\\d+).*hr").Success
                        ? Regex.Replace(durationText, "(\\d+).*", "$1:00").Trim()
                        : Regex.Replace(durationText, "(\\d+).*", "0:$1").Trim();
                    TimeSpan duration = TimeSpan.Parse(durationText);
                    DateTime arriving = departing + duration;

                    IWebElement costElement = await C.FindElementAndWait(journeyElement, By.CssSelector(".nx-price"));
                    string costText = costElement.GetAttribute("innerText").Trim().Replace("£", "");

                    Journey journey = new(departing, arriving, "National Express", duration, $"{Origin}-{Destination}", Convert.ToInt32(double.Parse(costText)), JourneyType.Local);
                    results.Add(journey);
                }
                if (allFlightsRetrieved) return new JourneyCollection(results);

                ReadOnlyCollection<IWebElement> expandButtons = await C.FindElementsAndWait(By.CssSelector(".nx-earlier-later-journey"));
                foreach (IWebElement expandButton in expandButtons)
                {
                    string text = expandButton.GetAttribute("innerText");
                    if (text.ToLower().Contains("later"))
                    {
                        await C.ClickAndWait(expandButton);
                        break;
                    }
                }
            }
        }

        private async Task PopulateControls(string origin, string target, DateTime date)
        {
            if (OriginInput != null)
            {
                ReadOnlyCollection<IWebElement> showControlButtons = await C.FindElementsAndWait(By.CssSelector("strong"));
                foreach (IWebElement showControlButton in showControlButtons)
                {
                    if (showControlButton.GetAttribute("innerText").Contains("Change Journey"))
                    {
                        await C.ClickAndWait(showControlButton);
                        break;
                    }
                }
            }
            await GetControls();
            await InputOrigin(origin);
            await InputTarget(target);
            await PopulateDateAndHitDone(date);
        }

        private async Task InputOrigin(string origin)
        {
            await C.ClickAndWait(OriginInput);
            OriginInput.Clear();
            OriginInput.SendKeys(JourneyRetrieverData.GetTranslation(origin));
            await C.Delay(1000);
            ReadOnlyCollection<IWebElement> popups = await C.FindElementsAndWait(By.CssSelector("app-station-results"));
            IWebElement firstOption = await C.FindElementAndWait(popups[0], By.CssSelector("li"));
            await C.ClickAndWait(firstOption);
        }

        private async Task InputTarget(string target)
        {
            await C.ClickAndWait(DestinationInput);
            DestinationInput.Clear();
            DestinationInput.SendKeys(JourneyRetrieverData.GetTranslation(target));
            await C.Delay(1000);
            ReadOnlyCollection<IWebElement> popups = await C.FindElementsAndWait(By.CssSelector("app-station-results"));
            IWebElement firstOption = await C.FindElementAndWait(popups[1], By.CssSelector("li"));
            await C.ClickAndWait(firstOption);
        }

        private async Task GetControls()
        {
            ReadOnlyCollection<IWebElement> webElements = await C.FindElementsAndWait(By.Id("nx-from-station"));
            OriginInput = webElements[1];
            webElements = await C.FindElementsAndWait(By.Id("nx-to-station"));
            DestinationInput = webElements[1];
            DateInput = await C.FindElementAndWait(By.CssSelector(".nx-date-input"));
        }

        private async Task PopulateDateAndHitDone(DateTime date)
        {
            string dateInInputText = DateInput.GetAttribute("innerText");
            DateTime dateInInput = DateTime.ParseExact(dateInInputText, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            bool dateIsInputIsLaterThanTarget = dateInInput.CompareTo(date) == 1;
            await C.ClickAndWait(DateInput);

            IWebElement calendar = await C.FindElementAndWait(By.CssSelector("mat-calendar"));
            IWebElement monthElement = await C.FindElementAndWait(calendar, By.CssSelector("calendar-header"));

            ReadOnlyCollection<IWebElement> monthButtons = await C.FindElementsAndWait(calendar, By.CssSelector("button"));
            IWebElement changeMonthButton;

            string monthAndYearText = monthElement.GetAttribute("innerText").Trim();
            string monthText = Regex.Match(monthAndYearText, @"^(\w+)").Groups[1].Value;

            while (!monthText.Equals(date.ToString("MMMM")))
            {
                changeMonthButton = dateIsInputIsLaterThanTarget ? monthButtons[0] : monthButtons[1];
                await C.ClickAndWait(changeMonthButton);
                monthElement = await C.FindElementAndWait(calendar, By.CssSelector("calendar-header"));
                monthAndYearText = monthElement.GetAttribute("innerText").Trim();
                monthText = Regex.Match(monthAndYearText, @"^(\w+)").Groups[1].Value;
            }

            ReadOnlyCollection<IWebElement> dates = await C.FindElementsAndWait(calendar, By.CssSelector(".mat-calendar-body-cell"));
            foreach (IWebElement dateElement in dates)
            {
                string day = dateElement.GetAttribute("innerText").Trim();
                if (int.Parse(day) == date.Day)
                {
                    await C.ClickAndWait(dateElement);
                    break;
                }
            }
            IWebElement submit = await C.FindElementAndWait(By.Id("nx-find-journey-button"));
            await C.ClickAndWait(submit);
        }
    }
}