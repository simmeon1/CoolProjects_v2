using FlightConnectionsDotCom_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class AirportPathGenerator
    {
        private IDictionary<string, HashSet<string>> AirportDestinations { get; set; }
        private LinkedList<string> CurrentPath { get; set; }
        private int MaxFlights { get; set; }
        private List<Path> Paths { get; set; }
        private string Target { get; set; }

        public AirportPathGenerator(IDictionary<string, HashSet<string>> airportDestinations)
        {
            AirportDestinations = airportDestinations;
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
                    Target = target;
                    UpdateCurrentPathAndScanItIfNeeded(origin, target);
                }
            }
            return Paths.OrderBy(p => p.Count()).ThenBy(p => p.ToString()).ToList();
        }

        private void UpdateCurrentPathAndScanItIfNeeded(string origin, string target)
        {
            if (CurrentPathHasReachedTargetOrExceededMaxFlightCount()) return;
            CurrentPath.AddLast(origin);
            if (!CurrentPathContainsRepeatedAirport()) ScanCurrentPath(origin, target);
            CurrentPath.RemoveLast();
        }

        private bool CurrentPathHasReachedTargetOrExceededMaxFlightCount()
        {
            return CurrentPath.Count >= MaxFlights || (CurrentPath.Last != null && CurrentPath.Last.Value.Equals(Target));
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
            HashSet<string> destinations = AirportDestinations[origin];
            foreach (string destination in destinations)
            {
                if (destination.Equals(target)) Paths.Add(new Path(new List<string>(CurrentPath) { destination }));
                UpdateCurrentPathAndScanItIfNeeded(destination, target);
            }
        }
    }
}