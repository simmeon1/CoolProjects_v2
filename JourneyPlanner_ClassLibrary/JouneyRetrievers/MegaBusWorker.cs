using Common_ClassLibrary;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public class MegaBusWorker : IJourneyRetriever
    {
        private JourneyRetrieverComponents C { get; set; }
        private JourneyCollection CollectedJourneys { get; set; }
        private JourneyRetrieverData JourneyRetrieverData { get; set; }
        private int PathsToSearch { get; set; }
        private int PathsCollected { get; set; }

        public MegaBusWorker(JourneyRetrieverComponents c)
        {
            C = c;
        }

        public async Task<JourneyCollection> CollectJourneys(JourneyRetrieverData data, DateTime dateFrom, DateTime dateTo, JourneyCollection existingJourneys)
        {
            Initialise(data, existingJourneys);
            WriteInitialLog(data);
            await LoopThroughPathsAndCollectJourneys(data, dateFrom, dateTo);
            return CollectedJourneys;
        }

        private void Initialise(JourneyRetrieverData data, JourneyCollection existingJourneys)
        {
            CollectedJourneys = existingJourneys;
            JourneyRetrieverData = data;
            PathsToSearch = 0;
            PathsCollected = 0;
        }

        private void WriteInitialLog(JourneyRetrieverData data)
        {
            foreach (DirectPath directPath in data.DirectPaths) PathsToSearch++;
            C.Log($"Starting search for {PathsToSearch} paths.");
        }

        private async Task LoopThroughPathsAndCollectJourneys(JourneyRetrieverData data, DateTime dateFrom, DateTime dateTo)
        {
            C.Log($"Getting {data.DirectPaths.Count} paths from {dateFrom} to {dateTo}.");
            List<Task<HttpResponseMessage>> tasks = new();
            foreach (DirectPath directPath in data.DirectPaths)
            {
                string origin = directPath.GetStart();
                string target = directPath.GetEnd();

                string pathName = directPath.ToString();

                List<DateTime> listOfExtraDates = new() { };
                DateTime tempDate = dateFrom.AddDays(1);
                while (DateTime.Compare(tempDate, dateTo) <= 0)
                {
                    listOfExtraDates.Add(tempDate);
                    tempDate = tempDate.AddDays(1);
                }

                List<DateTime> allDates = new() { dateFrom };
                allDates.AddRange(listOfExtraDates);

                foreach (DateTime date in allDates)
                {
                    string uri = $"https://uk.megabus.com/journey-planner/api/journeys?originId=" +
                        data.GetTranslation(origin) +
                        "&destinationId=" +
                        data.GetTranslation(target) + "&departureDate=" +
                        date.ToString("yyyy-MM-dd") + "&totalPassengers=1&concessionCount=0&nusCount=0&otherDisabilityCount=0&wheelchairSeated=0&pcaCount=0&days=1";
                    HttpRequestMessage request = new(HttpMethod.Get, uri);
                    Task<HttpResponseMessage> responseTask = C.HttpClient.SendRequest(request);
                    tasks.Add(responseTask);
                }
            }

            await Task.WhenAll(tasks);

            List<Journey> journeys = new();
            foreach (Task<HttpResponseMessage> task in tasks)
            {
                HttpResponseMessage response = task.Result;
                string responseText = await response.Content.ReadAsStringAsync();
                JObject jo = JObject.Parse(responseText);

                JToken journeysFromResponse = jo["journeys"];
                foreach (JToken journeyFromResponse in journeysFromResponse)
                {
                    DateTime departure = DateTime.Parse(journeyFromResponse["departureDateTime"].ToString());
                    DateTime arrival = DateTime.Parse(journeyFromResponse["arrivalDateTime"].ToString());
                    TimeSpan span = arrival - departure;
                    double cost = double.Parse(journeyFromResponse["price"].ToString());
                    string origin = data.GetKeyFromTranslation(journeyFromResponse["origin"]["cityId"].ToString());
                    string destination = data.GetKeyFromTranslation(journeyFromResponse["destination"]["cityId"].ToString());
                    string path = $"{origin}-{destination}";
                    Journey journey = new(departure, arrival, "Mega Bus", span, path, cost, GetRetrieverName());
                    journeys.Add(journey);
                }
            }
            CollectedJourneys.AddRange(new(journeys.OrderBy(j => j.ToString()).ToList()));

            foreach (DirectPath directPath in data.DirectPaths)
            {
                C.JourneyRetrieverEventHandler.InformOfPathDataFullyCollected(directPath.ToString());
                PathsCollected++;
            }

            C.Log($"Collected data for {data.DirectPaths.Count} paths ({Globals.GetPercentageAndCountString(PathsCollected, PathsToSearch)})");
        }

        public string GetRetrieverName()
        {
            return nameof(MegaBusWorker);
        }
    }
}