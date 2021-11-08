using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class AirportPathGenerator
    {
        private Dictionary<string, HashSet<string>> AirportsAndDestinations { get; set; }
        private Dictionary<string, HashSet<string>> AirportLocalLinks { get; set; }
        private LinkedList<string> CurrentPath { get; set; }
        private int MaxFlights { get; set; }
        private List<Path> Paths { get; set; }
        private HashSet<string> LocalLinksStrings { get; set; }

        public AirportPathGenerator(Dictionary<string, HashSet<string>> airportsAndDestinations, Dictionary<string, HashSet<string>> airportLocalLinks = null)
        {
            AirportsAndDestinations = airportsAndDestinations;
            AirportLocalLinks = airportLocalLinks ?? new();
        }

        public List<Path> GeneratePaths(List<string> origins, List<string> targets, int maxFlights, bool onlyIncludeShortestPaths, bool includeLocalLinks = false)
        {
            Paths = new List<Path>();
            MaxFlights = maxFlights;
            LocalLinksStrings = new();

            if (includeLocalLinks) AddLocalLinksToAirportDestinations();

            foreach (string origin in origins)
            {
                foreach (string target in targets)
                {
                    CurrentPath = new LinkedList<string>();
                    UpdateCurrentPathAndScanItIfNeeded(origin, target);
                }
            }
            List<Path> result = !onlyIncludeShortestPaths ? Paths : GetShortestPathsExcludingLocalLinks();
            return result.OrderBy(p => GetCountOfTripsExcludingLocalLinks(p)).ThenBy(p => p.Count()).ThenBy(p => p.ToString()).ToList();
        }

        private int GetCountOfTripsExcludingLocalLinks(Path p)
        {
            int allCount = p.Count() - 1;
            int localLinksCount = 0;
            foreach (string link in LocalLinksStrings)
            {
                if (p.ToString().Contains(link)) localLinksCount++;
            }
            return allCount - localLinksCount;
        }

        private void AddLocalLinksToAirportDestinations()
        {
            foreach (KeyValuePair<string, HashSet<string>> pair in AirportLocalLinks)
            {
                foreach (string item in AirportLocalLinks[pair.Key])
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
                int minCount = pair.Value.Min(x => GetCountOfTripsExcludingLocalLinks(x));
                shortPathAndMinCounts.Add(pair.Key, minCount);
            }

            List<Path> newPaths = new();
            foreach (KeyValuePair<string, int> pair in shortPathAndMinCounts)
            {
                newPaths.AddRange(shortPathAndFullPaths[pair.Key].Where(x => GetCountOfTripsExcludingLocalLinks(x) == pair.Value));
            }
            return newPaths;
        }

        private void UpdateCurrentPathAndScanItIfNeeded(string origin, string target)
        {
            CurrentPath.AddLast(origin);
            if (!CurrentPathContainsRepeatedAirport() && origin.Equals(target)) Paths.Add(new Path(new List<string>(CurrentPath)));
            if (GetCountOfTripsExcludingLocalLinks(new Path(CurrentPath.ToList())) < MaxFlights) ScanCurrentPath(origin, target);
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