using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public class PathsToDirectPathGroupsConverter
    {
        public Dictionary<string, JourneyRetrieverData> GetGroups(List<Path> paths, Dictionary<string, JourneyRetrieverData> existingData, Dictionary<string, HashSet<string>> airportsAndDestinations)
        {
            List<DirectPath> directPaths = new();
            foreach (Path path in paths) directPaths.AddRange(path.GetDirectPaths());
            List<DirectPath> cleanDirectPaths = RemoveDuplicatesAndOrderDirectPaths(directPaths);

            Dictionary<DirectPath, bool> directPathsThatHaveADefinedWorker = InitialiseDictWithDirectPathsWithPredefinedWorkers(cleanDirectPaths);
            Dictionary<string, JourneyRetrieverData> data = new();
            foreach (KeyValuePair<string, JourneyRetrieverData> existingDataPair in existingData)
            {
                AddDataFromPairToToNewData(existingDataPair, data, cleanDirectPaths, directPathsThatHaveADefinedWorker);
            }
            return AddGoogleFlightJourneys(data, directPathsThatHaveADefinedWorker, airportsAndDestinations);
        }

        private static Dictionary<DirectPath, bool> InitialiseDictWithDirectPathsWithPredefinedWorkers(List<DirectPath> cleanDirectPaths)
        {
            Dictionary<DirectPath, bool> directPathsThatHaveADefinedWorker = new();
            foreach (DirectPath directPath in cleanDirectPaths)
            {
                directPathsThatHaveADefinedWorker.Add(directPath, false);
            }
            return directPathsThatHaveADefinedWorker;
        }

        private static List<DirectPath> RemoveDuplicatesAndOrderDirectPaths(List<DirectPath> directPaths)
        {
            return directPaths
                .GroupBy(x => x.ToString())
                .Select(g => g.First())
                .OrderBy(x => x.ToString())
                .ToList();
        }

        private static void AddDataFromPairToToNewData(
            KeyValuePair<string, JourneyRetrieverData> existingDataPair,
            Dictionary<string, JourneyRetrieverData> newData,
            List<DirectPath> cleanDirectPaths,
            Dictionary<DirectPath, bool> directPathsThatHaveADefinedWorker)
        {
            newData.Add(existingDataPair.Key, null);
            JourneyRetrieverData data = existingDataPair.Value;

            Dictionary<string, string> translationsForPair = data.Translations;
            List<DirectPath> pathsForPair = new();
            for (int i = 0; i < cleanDirectPaths.Count; i++)
            {
                DirectPath directPath = cleanDirectPaths[i];
                if (data.DirectPaths.Any(d => d.ToString().Equals(directPath.ToString())))
                {
                    pathsForPair.Add(directPath);
                    directPathsThatHaveADefinedWorker[directPath] = true;
                }
            }
            string worker = existingDataPair.Key;
            newData[worker] = new(pathsForPair, translationsForPair);
        }

        private static Dictionary<string, JourneyRetrieverData> AddGoogleFlightJourneys(Dictionary<string, JourneyRetrieverData> data, Dictionary<DirectPath, bool> directPathsThatHaveADefinedWorker, Dictionary<string, HashSet<string>> airportsAndDestinations)
        {
            List<DirectPath> remainingDirectPaths = new();
            foreach (KeyValuePair<DirectPath, bool> pair in directPathsThatHaveADefinedWorker)
            {
                DirectPath path = pair.Key;
                string start = path.GetStart();
                string end = path.GetEnd();
                if (airportsAndDestinations.ContainsKey(start) && airportsAndDestinations[start].Contains(end)) remainingDirectPaths.Add(pair.Key);
            }

            List<DirectPath> cleanDirectPaths = RemoveDuplicatesAndOrderDirectPaths(remainingDirectPaths);
            if (cleanDirectPaths.Count == 0) return data;
            data.Add(nameof(GoogleFlightsWorker), new(cleanDirectPaths, new()));
            return data;
        }
    }
}