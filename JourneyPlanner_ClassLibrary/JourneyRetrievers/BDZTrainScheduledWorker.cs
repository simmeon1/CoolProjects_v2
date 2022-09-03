using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Interfaces;
using JourneyPlanner_ClassLibrary.Workers;

namespace JourneyPlanner_ClassLibrary.JourneyRetrievers
{
    public class BDZTrainScheduledWorker : IJourneyRetriever
    {
        private ScheduledWorker scheduledWorker = new ScheduledWorker();

        public BDZTrainScheduledWorker(JourneyRetrieverComponents c)
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
            return await Task.FromResult(
                scheduledWorker.GetJourneysForDates(
                    allDates.First(),
                    allDates.Last(),
                    new DateTime(1, 1, 1, 22, 30, 0),
                    new DateTime(1, 1, 1, 22, 30, 0),
                    new TimeSpan(1, 0, 0, 0),
                    new TimeSpan(8, 30, 0),
                    origin,
                    destination,
                    "BDZ Train (Scheduled)",
                    nameof(BDZTrainScheduledWorker),
                    15
                )
            );
        }
    }
}