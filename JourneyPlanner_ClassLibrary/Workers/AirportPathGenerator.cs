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
        private List<Path> Paths { get; set; }

        public AirportPathGenerator(Dictionary<string, HashSet<string>> airportsAndDestinations)
        {
            AirportsAndDestinations = airportsAndDestinations;
        }

        public List<Path> GeneratePaths(List<string> origins, List<string> targets, int maxFlights)
        {
            Paths = new List<Path>();
            MaxFlights = maxFlights;

            foreach (string origin in origins)
            {
                foreach (string target in targets)
                {
                    CurrentPath = new LinkedList<string>();
                    UpdateCurrentPathAndScanItIfNeeded(origin, target);
                }
            }

            return Paths.OrderBy(GetCountOfJourneys).ThenBy(p => p.Count()).ThenBy(p => p.ToString()).ToList();
        }

        private static int GetCountOfJourneys(Path p)
        {
            return p.Count() - 1;
        }
        
        private void UpdateCurrentPathAndScanItIfNeeded(string origin, string target)
        {
            CurrentPath.AddLast(origin);
            Path path = new(CurrentPath.ToList());
            int flightCount = GetCountOfJourneys(path);
            bool maxCountsNotPassed = flightCount <= MaxFlights;
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