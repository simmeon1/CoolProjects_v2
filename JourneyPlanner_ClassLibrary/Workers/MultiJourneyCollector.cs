using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.JourneyRetrievers;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class MultiJourneyCollector
    {
        public async Task<JourneyCollection> GetJourneys(
            JourneyRetrieverComponents c,
            List<DirectPath> directPaths,
            DateTime dateFrom,
            DateTime dateTo
            )
        {
            JourneyCollection allJourneys = new();

            int totalPathsCollected = 0;
            int totalPathsToSearch = directPaths.Count;
            List<DateTime> allDates = GetAllDates(dateFrom, dateTo);
            
            c.Log($"Beginning search for {totalPathsToSearch} paths from {dateFrom.ToShortDateString()} to {dateTo.ToShortDateString()}");
            
            var retriever = new GoogleFlightsWorker(c);
            try
            {
                retriever.Initialise();
                JourneyCollection journeys = await retriever.GetJourneysForDates(directPaths, allDates);
                for (int i = 0; i < journeys.GetCount(); i++)
                {
                    Journey journey = journeys[i];
                    if (!allJourneys.AlreadyContainsJourney(journey))
                    {
                        allJourneys.Add(journey);
                    }
                }

                totalPathsCollected += directPaths.Count;

                c.Log($"Total progress ({Globals.GetPercentageAndCountString(totalPathsCollected, totalPathsToSearch)})");
            }
            catch (Exception ex)
            {
                c.Log("An exception was thrown while collecting flights and the results have been returned early.");
                c.Log($"Exception details: {ex}");
                return allJourneys;
            }
            
            c.Log($"Journey retrieval finished.");
            return allJourneys;
        }

        private static List<DateTime> GetAllDates(DateTime dateFrom, DateTime dateTo)
        {
            DateTime tempDate = dateFrom.AddDays(1);
            List<DateTime> listOfExtraDates = new();
            while (DateTime.Compare(tempDate, dateTo) <= 0)
            {
                listOfExtraDates.Add(tempDate);
                tempDate = tempDate.AddDays(1);
            }
            List<DateTime> allDates = new() { dateFrom };
            allDates.AddRange(listOfExtraDates);
            return allDates;
        }
    }
}