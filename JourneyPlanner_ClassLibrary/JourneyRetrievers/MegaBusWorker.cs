using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Interfaces;
using JourneyPlanner_ClassLibrary.Workers;
using Newtonsoft.Json.Linq;

namespace JourneyPlanner_ClassLibrary.JourneyRetrievers
{
    public class MegaBusWorker : IJourneyRetriever
    {
        private JourneyRetrieverComponents C { get; set; }
        private JourneyRetrieverData JourneyRetrieverData { get; set; }

        public MegaBusWorker(JourneyRetrieverComponents c)
        {
            C = c;
        }

        public void Initialise(JourneyRetrieverData data)
        {
            JourneyRetrieverData = data;
        }

        public async Task<JourneyCollection> GetJourneysForDates(string origin, string destination, List<DateTime> allDates)
        {
            List<Task<HttpResponseMessage>> tasks = new();
            foreach (DateTime date in allDates)
            {
                string uri = $"https://uk.megabus.com/journey-planner/api/journeys?originId=" +
                    JourneyRetrieverData.GetTranslation(origin) +
                    "&destinationId=" +
                    JourneyRetrieverData.GetTranslation(destination) + "&departureDate=" +
                    date.ToString("yyyy-MM-dd") + "&totalPassengers=1&concessionCount=0&nusCount=0&otherDisabilityCount=0&wheelchairSeated=0&pcaCount=0&days=1";
                HttpRequestMessage request = new(HttpMethod.Get, uri);
                Task<HttpResponseMessage> responseTask = C.HttpClient.SendRequest(request);
                tasks.Add(responseTask);
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
                    string path = $"{origin}-{destination}";
                    Journey journey = new(departure, arrival, "Mega Bus", span, path, cost, nameof(MegaBusWorker));
                    journeys.Add(journey);
                }
            }
            return new(journeys.OrderBy(j => j.ToString()).ToList());
        }
    }
}