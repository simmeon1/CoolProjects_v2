using Common_ClassLibrary;
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
        }

        public async Task<JourneyCollection> GetJourneysForDates(string origin, string destination, List<DateTime> allDates)
        {
            await PopulateControls(origin, destination, allDates[0]);

            List<Journey> journeys = new();
            DateTime firstDate = allDates[0];
            DateTime dateToUse = new(firstDate.Year, firstDate.Month, firstDate.Day, 0, 0, 0);
            DateTime lastDate = allDates[allDates.Count - 1];
            DateTime lastDateAndLastSecond = new(lastDate.Year, lastDate.Month, lastDate.Day, 23, 59, 59);
            bool pathComplete = false;

            while (true)
            {
                object result = SendRequest(dateToUse);
                Dictionary<string, object> resultParsed = (Dictionary<string, object>)result;
                ReadOnlyCollection<object> journeysInResponse = (ReadOnlyCollection<object>)resultParsed["journeyCommand"];
                if (journeysInResponse == null) break;
                foreach (Dictionary<string, object> journeyInResponse in journeysInResponse)
                {
                    DateTime departure = DateTime.Parse(journeyInResponse["departureDateTime"].ToString());
                    if (departure.CompareTo(lastDateAndLastSecond) == 1)
                    {
                        pathComplete = true;
                        break;
                    }
                    else dateToUse = departure.AddMinutes(1);

                    DateTime arrival = DateTime.Parse(journeyInResponse["arrivalDateTime"].ToString());
                    TimeSpan span = arrival - departure;
                    Dictionary<string, object> costData = (Dictionary<string, object>)journeyInResponse["fare"];
                    double cost = double.Parse(costData["grossAmountInPennies"].ToString()) / 100;
                    string path = $"{origin}-{destination}";
                    Journey journey = new(departure, arrival, "National Express", span, path, cost, nameof(NationalExpressWorker));
                    journeys.Add(journey);
                }
                if (pathComplete) break;
            }
            return new(journeys.OrderBy(j => j.ToString()).ToList());
        }

        private object SendRequest(DateTime dateToUse)
        {
            //document.querySelector('#nx-expandable-journey-search > app-journey-planner').__ngContext__[30].searchTerms
            string appJourneyPlannerJs = @"document.querySelector('#nx-expandable-journey-search > app-journey-planner').__ngContext__[30]";
            C.JavaScriptExecutor.ExecuteScript(appJourneyPlannerJs + ".searchTerms.date = " + appJourneyPlannerJs + ".calendarHelper.getSearchTermsDate();");
            Dictionary<string, object> searchTerms = (Dictionary<string, object>)C.JavaScriptExecutor.ExecuteScript($@"return {appJourneyPlannerJs}.searchTerms");
            C.JavaScriptExecutor.ExecuteScript(appJourneyPlannerJs + $".searchTerms.date.leaving.time.selected = '{dateToUse.ToString("HH:mm")}';");
            C.JavaScriptExecutor.ExecuteScript(appJourneyPlannerJs + ".searchTermsManagementService.setSearchterms(" + appJourneyPlannerJs + ".searchTerms);" +
                appJourneyPlannerJs + ".searchTermsHelperService.getTotalCoachCards() > 0 && " + appJourneyPlannerJs + ".broadcastService.emit('gaCoachcard', {type: 'coachcardSearch'});");
            string jsToGetResponse = "var callback = arguments[arguments.length - 1];" +
                appJourneyPlannerJs + @".journeyService.newJourneySearch('OUT').subscribe({next(response) {callback(response);}});";
            object result = C.JavaScriptExecutor.ExecuteAsyncScript(jsToGetResponse);
            return result;
        }

        private async Task PopulateControls(string origin, string destination, DateTime date)
        {
            if (C.FindElementByAttribute(By.Id("nx-expandable-journey-search")).GetAttribute("hidden") != null) C.FindElementByAttributeAndClickIt(By.Id("editMyJourney"));
            await InputLocation(origin, 0);
            await InputLocation(destination, 1);
            PopulateDate(date);
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

        private bool PopupsHidden(int popupIndex)
        {
            ReadOnlyCollection<IWebElement> popups = C.FindElementsNew(By.CssSelector("app-station-popup"));
            IWebElement popupToQuery = popups[popupIndex];
            string isHidden = popupToQuery.GetAttribute("hidden");
            return isHidden == null ? false : true;
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

        private void PopulateDate(DateTime date)
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
        }

        private void PickCalendarTimes()
        {
            C.FindElementByAttributeAndClickIt(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(1) > nx-time-condition > select"));
            C.FindElementByAttributeAndClickIt(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(1) > nx-time-condition > select > option:nth-child(1)"));
            C.FindElementByAttributeAndClickIt(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(2) > nx-time > select"));
            C.FindElementByAttributeAndClickIt(By.CssSelector("#nx-datetime-picker > nx-time-picker > div > div.nx-display-flex > div:nth-child(2) > nx-time > select > option:nth-child(1)"));
        }
    }
}