using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Interfaces;
using JourneyPlanner_ClassLibrary.Workers;

namespace JourneyPlanner_ClassLibrary.JourneyRetrievers
{
    public class ScheduledWorker
    {
        public JourneyCollection GetJourneysForDates(
            DateTime startDate,
            DateTime endDate,
            DateTime startTime,
            DateTime endTime,
            TimeSpan interval,
            TimeSpan duration,
            string origin,
            string destination,
            string companyName,
            string workerName,
            double cost
        )
        {
            DateTime date = startDate;
            List<Journey> journeys = new();
            while (date.CompareTo(endDate) < 1)
            {
                DateTime time = startTime;
                while (true)
                {
                    DateTime departure = new(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                    DateTime arrival = departure.AddTicks(duration.Ticks);
                    string path = $"{origin}-{destination}";
                    Journey journey = new(
                        departure,
                        arrival,
                        companyName,
                        duration,
                        path,
                        cost,
                        workerName
                    );
                    journeys.Add(journey);
                    time += interval;
                    if ((endTime - time).Ticks < 0) break;
                }
                date = date.AddDays(1);
            }
            return new JourneyCollection(journeys.OrderBy(j => j.ToString()).ToList());
        }
    }
}