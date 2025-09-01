using System;
using System.Collections.Generic;
using System.Linq;
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
            DateTime dateTo,
            JourneyCollection existingJourneyCollection
        )
        {
            JourneyCollection allJourneys = new();
            allJourneys.AddRange(existingJourneyCollection);
            var collectedPaths = allJourneys.Journeys.Select(x => x.Path).Distinct();
            directPaths = directPaths.Where(x => !collectedPaths.Contains(x.ToString())).ToList();

            var totalPathsCollected = 0;
            var totalPathsToSearch = directPaths.Count;
            var allDates = GetAllDates(dateFrom, dateTo);

            c.Log(
                $"Beginning search for {totalPathsToSearch} paths from {dateFrom.ToShortDateString()} to {dateTo.ToShortDateString()}"
            );

            var retriever = new GoogleFlightsWorker(c);
            try
            {
                retriever.Initialise();
                var journeys = await retriever.GetJourneysForDates(directPaths, allDates);
                allJourneys.AddRange(journeys);

                totalPathsCollected += directPaths.Count;

                c.Log(
                    $"Total progress ({Globals.GetPercentageAndCountString(totalPathsCollected, totalPathsToSearch)})"
                );
                c.Log("Journey retrieval finished.");
                return allJourneys;
            }
            catch (GoogleFlightsWorkerException ex)
            {
                c.Log("An exception was thrown while collecting flights and the results have been returned early.");
                c.Log($"Exception details: {ex}");
                allJourneys.AddRange(ex.Results);
                return allJourneys;
            }
        }

        private static List<DateTime> GetAllDates(DateTime dateFrom, DateTime dateTo)
        {
            var tempDate = dateFrom.AddDays(1);
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