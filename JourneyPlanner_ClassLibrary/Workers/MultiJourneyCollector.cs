using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Interfaces;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class MultiJourneyCollector : IMultiJourneyCollector
    {
        private IJourneyRetrieverInstanceCreator InstanceCreator { get; set; }
        public MultiJourneyCollector(IJourneyRetrieverInstanceCreator instanceCreator)
        {
            InstanceCreator = instanceCreator;
        }

        public async Task<MultiJourneyCollectorResults> GetJourneys(
            JourneyRetrieverComponents c,
            Dictionary<string, JourneyRetrieverData> retrieversAndData,
            DateTime dateFrom,
            DateTime dateTo,
            MultiJourneyCollectorResults existingResults = null)
        {
            JourneyCollection allJourneys = existingResults == null ? new JourneyCollection() : existingResults.JourneyCollection;
            Dictionary<string, Dictionary<string, bool>> progress = GetProgressDict(retrieversAndData);
            if (existingResults != null) UpdateJourneysAndDictWithDataFromExistingProgress(allJourneys, progress, existingResults.Progress);

            int totalPathsCollected = 0;
            int totalPathsToSearch = 0;
            List<DateTime> allDates = GetAllDates(dateFrom, dateTo);

            foreach (KeyValuePair<string, Dictionary<string, bool>> workerAndPathCompletion in progress)
            {
                foreach (KeyValuePair<string, bool> pathAndCompletion in workerAndPathCompletion.Value)
                {
                    if (!pathAndCompletion.Value) totalPathsToSearch++;
                }
            }

            c.Log($"Beginning search for {totalPathsToSearch} paths from {dateFrom.ToShortDateString()} to {dateTo.ToShortDateString()}");
            foreach (KeyValuePair<string, JourneyRetrieverData> data in retrieversAndData)
            {
                string retrieverName = data.Key;
                JourneyRetrieverData retrieverData = data.Value;

                RemoveAlreadyCompletedPathsForRetriever(retrieverData, retrieverName, progress);
                if (retrieverData.GetCountOfDirectPaths() == 0) continue;

                string fullClassName = $"{Assembly.GetExecutingAssembly().GetName().Name}.JourneyRetrievers.{retrieverName}";
                IJourneyRetriever retriever = InstanceCreator.CreateInstance(fullClassName, c);
                try
                {
                    retriever.Initialise(retrieverData);
                    int pathsCollected = 0;
                    int pathsToSearch = retrieverData.DirectPaths.Count;
                    c.Log($"Beginning search for {pathsToSearch} paths using {retrieverName}.");
                    foreach (DirectPath directPath in retrieverData.DirectPaths)
                    {
                        string origin = directPath.GetStart();
                        string destination = directPath.GetEnd();
                        string pathName = directPath.ToString();
                        c.Log($"Getting journeys for {pathName}.");
                        JourneyCollection journeys = await retriever.GetJourneysForDates(origin, destination, allDates);
                        for (int i = 0; i < journeys.GetCount(); i++)
                        {
                            Journey journey = journeys[i];
                            if (!allJourneys.AlreadyContainsJourney(journey))
                            {
                                allJourneys.Add(journey);
                            }
                        }
                        progress[retrieverName][pathName] = true;
                        pathsCollected++;
                        totalPathsCollected++;
                        c.Log($"Collected data for {pathName} ({journeys.GetCount()} journeys) ({Globals.GetPercentageAndCountString(pathsCollected, pathsToSearch)}), total ({Globals.GetPercentageAndCountString(totalPathsCollected, totalPathsToSearch)})");
                    }
                    c.Log($"Journey retrieval finished.");
                }
                catch (Exception ex)
                {
                    c.Logger.Log("An exception was thrown while collecting flights and the results have been returned early.");
                    c.Logger.Log($"Exception details: {ex}");
                    return new MultiJourneyCollectorResults(progress, allJourneys);
                }
            }
            return new MultiJourneyCollectorResults(progress, allJourneys);
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

        private static void UpdateJourneysAndDictWithDataFromExistingProgress(JourneyCollection journeys, Dictionary<string, Dictionary<string, bool>> newProgress, Dictionary<string, Dictionary<string, bool>> existingProgress)
        {
            RemoveJourneysMarkedAsNotRetrieved(journeys, existingProgress);
            foreach (KeyValuePair<string, Dictionary<string, bool>> workersAndPathData in newProgress)
            {
                string worker = workersAndPathData.Key;
                foreach (KeyValuePair<string, bool> pathAndCompletion in workersAndPathData.Value)
                {
                    string path = pathAndCompletion.Key;
                    if (existingProgress.ContainsKey(worker) && existingProgress[worker].ContainsKey(path))
                    {
                        newProgress[worker][path] = existingProgress[worker][path];
                    }
                }
            }
        }

        private static void RemoveJourneysMarkedAsNotRetrieved(JourneyCollection journeys, Dictionary<string, Dictionary<string, bool>> existingProgress)
        {
            foreach (KeyValuePair<string, Dictionary<string, bool>> data in existingProgress)
            {
                string retrieverName = data.Key;
                foreach (KeyValuePair<string, bool> pathAndCompletion in data.Value)
                {
                    if (!pathAndCompletion.Value) journeys.RemoveJourneysThatMatchPathAndRetriever(retrieverName, pathAndCompletion.Key);
                }
            }
        }

        private static void RemoveAlreadyCompletedPathsForRetriever(JourneyRetrieverData retrieverData, string retrieverName, Dictionary<string, Dictionary<string, bool>> progress)
        {
            foreach (KeyValuePair<string, bool> alreadyCompletedPaths in progress[retrieverName])
            {
                if (alreadyCompletedPaths.Value) retrieverData.RemovePath(alreadyCompletedPaths.Key);
            }
        }

        private static Dictionary<string, Dictionary<string, bool>> GetProgressDict(Dictionary<string, JourneyRetrieverData> retrieversAndData)
        {
            Dictionary<string, Dictionary<string, bool>> progress = new();
            foreach (KeyValuePair<string, JourneyRetrieverData> data in retrieversAndData)
            {
                string worker = data.Key;
                progress.Add(worker, new Dictionary<string, bool>());
                foreach (DirectPath directPath in data.Value.DirectPaths)
                {
                    progress[data.Key].Add(directPath.ToString(), false);
                }
            }
            return progress;
        }
    }
}