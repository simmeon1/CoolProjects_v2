using System.Collections.Generic;
using System.Linq;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Workers
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
            AddLocalLinksToAirportDestinations(airportLocalLinks ?? new Dictionary<string, HashSet<string>>());
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
            return result.OrderBy(GetCountOfJourneys).ThenBy(p => p.Count()).ThenBy(p => p.ToString()).ToList();
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
            return LocalLinksStrings.Count(link => p.ToString().Contains(link));
        }

        private void AddLocalLinksToAirportDestinations(Dictionary<string, HashSet<string>> airportLocalLinks)
        {
            LocalLinksStrings = new HashSet<string>();
            foreach (string origin in airportLocalLinks.Keys)
            {
                foreach (string destination in airportLocalLinks[origin])
                {
                    AirportsAndDestinations[origin].Add(destination);
                    LocalLinksStrings.Add($"{origin}-{destination}");
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
            foreach ((string key, List<Path> value) in shortPathAndFullPaths)
            {
                int minCount = value.Min(GetCountOfJourneys);
                shortPathAndMinCounts.Add(key, minCount);
            }

            List<Path> newPaths = new();
            foreach ((string key, int value) in shortPathAndMinCounts)
            {
                newPaths.AddRange(shortPathAndFullPaths[key].Where(x => GetCountOfJourneys(x) == value));
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
            if (maxCountsNotPassed && !CurrentPathContainsRepeatedAirport())
            {
                if (origin.Equals(target)) Paths.Add(new Path(new List<string>(CurrentPath)));
                ScanCurrentPath(origin, target);
            }
            CurrentPath.RemoveLast();
        }

        private bool CurrentPathContainsRepeatedAirport()
        {
            HashSet<string> airportOccurrences = new();
            foreach (string airport in CurrentPath)
            {
                if (airportOccurrences.Contains(airport))
                {
                    return true;
                }
                airportOccurrences.Add(airport);
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