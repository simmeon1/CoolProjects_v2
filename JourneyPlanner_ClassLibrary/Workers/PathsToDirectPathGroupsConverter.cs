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
        public Dictionary<string, JourneyRetrieverData> GetGroups(List<Path> paths, Dictionary<string, JourneyRetrieverData> existingData = null)
        {
            List<DirectPath> directPaths = new();
            foreach (Path path in paths) directPaths.AddRange(path.GetDirectPaths());
            List<DirectPath> cleanDirectPaths = RemoveDuplicatesAndOrderDirectPaths(directPaths);
            existingData ??= new();

            Dictionary<string, JourneyRetrieverData> newData = new();
            foreach (KeyValuePair<string, JourneyRetrieverData> existingDataPair in existingData)
            {
                AddDataFromPairToToNewData(existingDataPair, newData, cleanDirectPaths);
            }
            return AddGoogleFlightsToNewDataIfPathsForItRemain(cleanDirectPaths, newData);
        }

        private static List<DirectPath> RemoveDuplicatesAndOrderDirectPaths(List<DirectPath> directPaths)
        {
            return directPaths
                .GroupBy(x => x.ToString())
                .Select(g => g.First())
                .OrderBy(x => x.ToString())
                .ToList();
        }

        private static void AddDataFromPairToToNewData(KeyValuePair<string, JourneyRetrieverData> existingDataPair, Dictionary<string, JourneyRetrieverData> newData, List<DirectPath> directPaths)
        {
            newData.Add(existingDataPair.Key, null);
            string worker = existingDataPair.Key;
            JourneyRetrieverData data = existingDataPair.Value;

            Dictionary<string, string> translationsForPair = data.Translations;
            List<DirectPath> pathsForPair = new();
            for (int i = 0; i < directPaths.Count; i++)
            {
                DirectPath directPath = directPaths[i];
                if (data.DirectPaths.Any(d => d.ToString().Equals(directPath.ToString())))
                {
                    pathsForPair.Add(directPath);
                    directPaths.RemoveAt(i);
                    i--;
                }
            }
            newData[worker] = new(pathsForPair, translationsForPair);
        }

        private static Dictionary<string, JourneyRetrieverData> AddGoogleFlightsToNewDataIfPathsForItRemain(List<DirectPath> cleanDirectPaths, Dictionary<string, JourneyRetrieverData> newData)
        {
            if (cleanDirectPaths.Count == 0) return newData;
            newData.Add(nameof(GoogleFlightsWorker), new(cleanDirectPaths, new()));
            return newData;
        }
    }
}