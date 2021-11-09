using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JourneyPlanner_ClassLibrary
{
    public class PathsAndWorkerGrouper
    {
        public const string DEFAULT_WORKER = "GoogleFlightsWorker";
        public Dictionary<string, List<Path>> GetGroups(List<Path> paths, List<PathsAndWorkerGroup> groups)
        {
            Dictionary<string, List<Path>> workerAndPathsDict = new();
            PopulateDictWithGroups(groups, workerAndPathsDict);
            foreach (Path path in paths) AddPathToCorrectEntryInDict(path, groups, workerAndPathsDict);
            return workerAndPathsDict;
        }

        private static void PopulateDictWithGroups(List<PathsAndWorkerGroup> groups, Dictionary<string, List<Path>> dict)
        {
            dict.Add(DEFAULT_WORKER, new());
            foreach (PathsAndWorkerGroup group in groups)
            {
                dict.Add(group.Worker, new());
            }
        }

        private static void AddPathToCorrectEntryInDict(Path path, List<PathsAndWorkerGroup> groups, Dictionary<string, List<Path>> dict)
        {
            string previousLocation = path[0];
            for (int i = 1; i < path.Count(); i++)
            {
                string pathStr = $"{previousLocation}-{path[i]}";
                string entryName = DEFAULT_WORKER;
                foreach (PathsAndWorkerGroup group in groups)
                {
                    if (group.Paths.Any(p => p.ToString().Equals(pathStr)))
                    {
                        entryName = group.Worker;
                        break;
                    }
                }
                if (!dict[entryName].Any(p => p.ToString().Equals(pathStr)))
                {
                    dict[entryName].Add(new Path(new List<string>() { previousLocation, path[i] }));
                }
                previousLocation = path[i];
            }
        }
    }
}