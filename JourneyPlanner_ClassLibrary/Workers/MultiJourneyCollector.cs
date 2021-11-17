using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public class MultiJourneyCollector : IMultiJourneyCollector, IJourneyRetrieverEventHandler
    {
        private IJourneyRetrieverInstanceCreator InstanceCreator { get; set; }
        private string CurrentRetriever { get; set; }
        private Dictionary<string, Dictionary<string, bool>> Progress { get; set; }

        public MultiJourneyCollector(IJourneyRetrieverInstanceCreator instanceCreator)
        {
            InstanceCreator = instanceCreator;
        }

        public async Task<MultiJourneyCollectorResults> GetJourneys(
            JourneyRetrieverComponents components,
            Dictionary<string, JourneyRetrieverData> retrieversAndData,
            DateTime dateFrom,
            DateTime dateTo,
            MultiJourneyCollectorResults existingResults = null)
        {
            JourneyCollection journeys = existingResults == null ? new() : existingResults.JourneyCollection;
            Dictionary<string, Dictionary<string, bool>> dictionary = GetProgressDict(retrieversAndData);
            if (existingResults != null) UpdateJourneysAndDictWithDataFromExistingProgress(journeys, dictionary, existingResults.Progress);
            Progress = dictionary;

            CurrentRetriever = "";
            foreach (KeyValuePair<string, JourneyRetrieverData> data in retrieversAndData)
            {
                string retrieverName = data.Key;
                JourneyRetrieverData retrieverData = data.Value;

                CurrentRetriever = retrieverName;
                RemoveAlreadyCompletedPathsForRetriever(retrieverData);
                if (retrieverData.GetCountOfDirectPaths() == 0) continue;

                string fullClassName = $"{Assembly.GetExecutingAssembly().GetName().Name}.{retrieverName}";
                IJourneyRetriever retriever = InstanceCreator.CreateInstance(fullClassName, components);
                try
                {
                    journeys = await retriever.CollectJourneys(retrieverData, dateFrom, dateTo, journeys);
                }
                catch (Exception ex)
                {
                    components.Logger.Log("An exception was thrown while collecting flights and the results have been returned early.");
                    components.Logger.Log($"Exception details: {ex}");
                    return new MultiJourneyCollectorResults(Progress, journeys);
                }
            }
            return new MultiJourneyCollectorResults(Progress, journeys);
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

        private void RemoveAlreadyCompletedPathsForRetriever(JourneyRetrieverData retrieverData)
        {
            foreach (KeyValuePair<string, bool> alreadyCompletedPaths in Progress[CurrentRetriever])
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
                progress.Add(worker, new());
                foreach (DirectPath directPath in data.Value.DirectPaths)
                {
                    progress[data.Key].Add(directPath.ToString(), false);
                }
            }
            return progress;
        }

        public void InformOfPathDataFullyCollected(string path)
        {
            Progress[CurrentRetriever][path] = true;
        }
    }
}