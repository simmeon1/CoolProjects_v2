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
        private readonly ScheduledWorker scheduledWorker = new();
        public MegaBusScheduledWorker(JourneyRetrieverComponents c)
        {
        }

        public void Initialise(JourneyRetrieverData data)
        {
        }
        
        public async Task<JourneyCollection> GetJourneysForDates(
            List<DirectPath> paths,
            List<DateTime> allDates
        )
        {
            return await Task.FromResult(
                scheduledWorker.GetJourneysForDates(
                    allDates.First(),
                    allDates.Last(),
                    new DateTime(1, 1, 1, 6, 0, 0),
                    new DateTime(1, 1, 1, 23, 0, 0),
                    new TimeSpan(1, 0, 0),
                    new TimeSpan(3, 30, 0),
                    paths,
                    "Mega Bus (Scheduled)",
                    nameof(MegaBusScheduledWorker),
                    20
                )
            );
        }
    }
}