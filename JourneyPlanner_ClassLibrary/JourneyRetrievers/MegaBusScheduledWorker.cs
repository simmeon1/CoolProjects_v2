using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Interfaces;
using JourneyPlanner_ClassLibrary.Workers;

namespace JourneyPlanner_ClassLibrary.JourneyRetrievers
{
    public class MegaBusScheduledWorker : IJourneyRetriever
    {
        public MegaBusScheduledWorker(JourneyRetrieverComponents c)
        {
        }

        public void Initialise(JourneyRetrieverData data)
        {
        }

        public async Task<JourneyCollection> GetJourneysForDates(
            string origin,
            string destination,
            List<DateTime> allDates
        )
        {
            List<Journey> journeys = new();
            foreach (DateTime date in allDates)
            {
                for (int hour = 6; hour < 23; hour++)
                {
                    DateTime departure = new(date.Year, date.Month, date.Day, hour, 0, 0);

                    bool addDayToArrival = false;
                    int hourTemp = hour + 3;
                    if (hourTemp > 23)
                    {
                        hourTemp %= 24;
                        addDayToArrival = true;
                    }
                    
                    DateTime arrival = new(date.Year, date.Month, date.Day, hourTemp, 30, 0);
                    if (addDayToArrival) arrival = arrival.AddDays(1); 
                    TimeSpan span = arrival - departure;
                    double cost = 20;
                    string path = $"{origin}-{destination}";
                    Journey journey = new(
                        departure,
                        arrival,
                        "Mega Bus (Scheduled)",
                        span,
                        path,
                        cost,
                        nameof(MegaBusScheduledWorker)
                    );
                    journeys.Add(journey);
                }
            }
            return await Task.FromResult(new JourneyCollection(journeys.OrderBy(j => j.ToString()).ToList()));
        }
    }
}