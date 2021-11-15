using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JourneyPlanner_ClassLibrary
{
    public class AirportPathGenerator
    {
        private Dictionary<string, HashSet<string>> AirportsAndDestinations { get; set; }
        private LinkedList<string> CurrentPath { get; set; }
        private int MaxFlights { get; set; }
        private int MaxLocalLinks { get; set; }
        private List<Path> Paths { get; set; }
        private HashSet<string> LocalLinksStrings { get; set; }

        public AirportPathGenerator(Dictionary<string, HashSet<string>> airportsAndDestinations, Dictionary<string, HashSet<string>> airportLocalLinks = null)
        {
            AirportsAndDestinations = airportsAndDestinations;
            AddLocalLinksToAirportDestinations(airportLocalLinks);
        }

        public List<Path> GeneratePaths(List<string> origins, List<string> targets, int maxFlights, int maxLocalLinks, bool onlyIncludeShortestPaths)
        {
            Paths = new List<Path>();
            MaxFlights = maxFlights;
            MaxLocalLinks = maxLocalLinks;

            foreach (string origin in origins)
            {
                foreach (string target in targets)
                {
                    CurrentPath = new LinkedList<string>();
                    UpdateCurrentPathAndScanItIfNeeded(origin, target);
                }
            }
            List<Path> result = !onlyIncludeShortestPaths ? Paths : GetShortestPathsExcludingLocalLinks();
            return result.OrderBy(p => GetCountOfFlights(p)).ThenBy(p => p.Count()).ThenBy(p => p.ToString()).ToList();
        }

        private int GetCountOfFlights(Path p)
        {
            return GetCountOfJourneys(p) - GetCountOfLocalLinks(p);
        }

        private static int GetCountOfJourneys(Path p)
        {
            return p.Count() - 1;
        }

        private int GetCountOfLocalLinks(Path p)
        {
            int localLinksCount = 0;
            foreach (string link in LocalLinksStrings)
            {
                if (p.ToString().Contains(link)) localLinksCount++;
            }
            return localLinksCount;
        }

        private void AddLocalLinksToAirportDestinations(Dictionary<string, HashSet<string>> airportLocalLinks)
        {
            LocalLinksStrings = new();
            foreach (KeyValuePair<string, HashSet<string>> pair in airportLocalLinks)
            {
                foreach (string item in airportLocalLinks[pair.Key])
                {
                    AirportsAndDestinations[pair.Key].Add(item);
                    LocalLinksStrings.Add($"{pair.Key}-{item}");
                }
            }
        }

        private List<Path> GetShortestPathsExcludingLocalLinks()
        {
            Dictionary<string, List<Path>> shortPathAndFullPaths = new();
            foreach (Path path in Paths)
            {
                string shortPath = path.GetSummarisedPath();
                if (!shortPathAndFullPaths.ContainsKey(shortPath)) shortPathAndFullPaths.Add(shortPath, new List<Path>() { path });
                else shortPathAndFullPaths[shortPath].Add(path);
            }

            Dictionary<string, int> shortPathAndMinCounts = new();
            foreach (KeyValuePair<string, List<Path>> pair in shortPathAndFullPaths)
            {
                int minCount = pair.Value.Min(x => GetCountOfFlights(x));
                shortPathAndMinCounts.Add(pair.Key, minCount);
            }

            List<Path> newPaths = new();
            foreach (KeyValuePair<string, int> pair in shortPathAndMinCounts)
            {
                newPaths.AddRange(shortPathAndFullPaths[pair.Key].Where(x => GetCountOfFlights(x) == pair.Value));
            }
            return newPaths;
        }

        private void UpdateCurrentPathAndScanItIfNeeded(string origin, string target)
        {
            CurrentPath.AddLast(origin);
            Path path = new(CurrentPath.ToList());
            int flightCount = GetCountOfFlights(path);
            int localLinkCount = GetCountOfLocalLinks(path);
            bool maxCountsNotPassed = flightCount <= MaxFlights && localLinkCount <= MaxLocalLinks;
            if (maxCountsNotPassed)
            {
                if (!CurrentPathContainsRepeatedAirport() && origin.Equals(target)) Paths.Add(new Path(new List<string>(CurrentPath)));
                ScanCurrentPath(origin, target);
            }
            CurrentPath.RemoveLast();
        }

        private bool CurrentPathContainsRepeatedAirport()
        {
            Dictionary<string, int> airportOccurences = new();
            foreach (string airport in CurrentPath)
            {
                if (airportOccurences.ContainsKey(airport))
                {
                    airportOccurences[airport]++;
                    return true;
                }
                else airportOccurences.Add(airport, 1);
            }
            return false;
        }

        private void ScanCurrentPath(string origin, string target)
        {
            HashSet<string> destinations = AirportsAndDestinations[origin];
            foreach (string destination in destinations)
            {
                UpdateCurrentPathAndScanItIfNeeded(destination, target);
            }
        }
    }
}