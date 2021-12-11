using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public class NationalExpressWorker : IJourneyRetriever
    {
        private JourneyRetrieverComponents C { get; set; }
        private JourneyRetrieverData JourneyRetrieverData { get; set; }
        private bool InitialPopulationDone { get; set; }

        public NationalExpressWorker(JourneyRetrieverComponents c)
        {
            C = c;
        }

        public void Initialise(JourneyRetrieverData data)
        {
            JourneyRetrieverData = data;
            SetUpSearch();
            InitialPopulationDone = false;
        }

        private void SetUpSearch()
        {
            C.NavigateToUrl("https://book.nationalexpress.com/coach/#/choose-journey");
        }

        public async Task<JourneyCollection> GetJourneysForDates(string origin, string destination, List<DateTime> allDates)
        {
            List<Journey> journeys = new();
            DateTime firstDate = allDates[0];
            DateTime dateToUse = new(firstDate.Year, firstDate.Month, firstDate.Day, 0, 0, 0);
            DateTime lastDate = allDates[allDates.Count - 1];
            DateTime lastDateAndLastSecond = new(lastDate.Year, lastDate.Month, lastDate.Day, 23, 59, 59);
            bool pathComplete = false;
            while (true)
            {
                string js = @"var callback = arguments[arguments.length - 1];
fetch('https://book.nationalexpress.com/nxrest/journey/search/OUT', {
  'headers': {
    'accept': 'application/json, text/plain, */*',
    'accept-language': 'en-US,en;q=0.9',
    'content-type': 'application/json',
    'sec-ch-ua': '\' Not A;Brand\';v=\'99\', \'Chromium\';v=\'96\', \'Google Chrome\';v=\'96\'',
    'sec-ch-ua-mobile': '?0',
    'sec-ch-ua-platform': '\'Windows\'',
    'sec-fetch-dest': 'empty',
    'sec-fetch-mode': 'cors',
    'sec-fetch-site': 'same-origin'
  },
  'referrer': 'https://book.nationalexpress.com/coach/',
  'referrerPolicy': 'strict-origin-when-cross-origin',
  'body': '{\'coachCard\':false,\'campaignId\':\'DEFAULT\',\'partnerId\':\'NX\',\'outboundArriveByOrDepartAfter\':\'DEPART_AFTER\',\'journeyType\':\'SINGLE\',\'operatorType\':\'DOMESTIC\',\'leaveDateTime\':{\'date\':\'dateForTravel\',\'time\':\'timeToUse\'},\'passengerNumbers\':{\'numberOfAdults\':1,\'numberOfBabies\':0,\'numberOfChildren\':0,\'numberOfDisabled\':0,\'numberOfSeniors\':0,\'numberOfEuroAdults\':0,\'numberOfEuroSeniors\':0,\'numberOfEuroYoungPersons\':0,\'numberOfEuroChildren\':0},\'coachCardNumbers\':{\'numberOnDisabledCoachcard\':0,\'numberOnSeniorCoachcard\':0,\'numberOnYouthCoachcard\':0},\'returnDateTime\':{\'date\':null,\'time\':null},\'fromToStation\':{\'fromStationId\':\'originTranslation\',\'toStationId\':\'targetTranslation\'},\'onDemand\':false,\'fromStationName\':\'HEATHROW Airport London T2,3 (LHR)\',\'toStationName\':\'GATWICK Airport London North (LGW)\',\'languageCode\':\'en\',\'channelsKey\':\'DESKTOP\',\'searchKey\':\'281160bd-c62c-4070-8f00-da3335299009\'}',
  'method': 'POST',
  'mode': 'cors',
  'credentials': 'omit'
}).then(response => callback(response.json()));";
                js = js.Replace("'", "\"");
                js = js.Replace("originTranslation", JourneyRetrieverData.GetTranslation(origin)); //57286
                js = js.Replace("targetTranslation", JourneyRetrieverData.GetTranslation(destination)); //63057
                js = js.Replace("dateForTravel", dateToUse.ToString("dd/MM/yyyy"));
                js = js.Replace("timeToUse", dateToUse.ToString("HH:mm"));

                //await C.Delayer.Delay(1000);
                object result;
                while (true)
                {
                    try
                    {
                        await C.Delayer.Delay(new Random().Next(10000, 20000));
                        result = C.JavaScriptExecutor.ExecuteAsyncScript(js);
                        break;
                    }
                    catch (Exception)
                    {
                        C.Log("Robot");
                    }
                }

                Dictionary<string, object> resultParsed = (Dictionary<string, object>)result;
                ReadOnlyCollection<object> journeysInResponse = (ReadOnlyCollection<object>)resultParsed["journeyCommand"];
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
    }
}