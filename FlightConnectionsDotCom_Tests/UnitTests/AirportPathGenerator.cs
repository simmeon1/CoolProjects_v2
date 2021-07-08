using FlightConnectionsDotCom_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    public class AirportPathGenerator
    {
        private IDictionary<string, HashSet<string>> AirportDestinations { get; set; }
        private LinkedList<string> CurrentPath { get; set; }
        private int MaxFlights { get; set; }
        private List<List<string>> Paths { get; set; }
        private string Origin { get; set; }
        private string Target { get; set; }

        public AirportPathGenerator(IDictionary<string, HashSet<string>> airportDestinations)
        {
            AirportDestinations = airportDestinations;
        }

        public List<List<string>> GeneratePaths(string origin, string target, int maxFlights)
        {
            Initialise(origin, target, maxFlights);
            UpdateCurrentPathAndScanItIfNeeded(origin, target);
            return Paths;
        }

        private void UpdateCurrentPathAndScanItIfNeeded(string origin, string target)
        {
            if (CurrentPath.Count >= MaxFlights || (CurrentPath.Last != null && CurrentPath.Last.Value.Equals(Target))) return;
            
            CurrentPath.AddLast(origin);
            Dictionary<string, int> airportOccurences = new();
            foreach (string airport in CurrentPath)
            {
                if (airportOccurences.ContainsKey(airport)) airportOccurences[airport]++;
                else airportOccurences.Add(airport, 1);
            }
            if (!airportOccurences.Values.Any(c => c > 1))
            {
                ScanCurrentPath(origin, target);
            }
            CurrentPath.RemoveLast();
        }

        private void Initialise(string origin, string target, int maxFlights)
        {
            Paths = new List<List<string>>();
            CurrentPath = new LinkedList<string>();
            MaxFlights = maxFlights;
            Origin = origin;
            Target = target;
        }

        private void ScanCurrentPath(string origin, string target)
        {
            HashSet<string> destinations = AirportDestinations[origin];
            foreach (string destination in destinations)
            {
                if (destination.Equals(target)) Paths.Add(new List<string>(CurrentPath) { destination });
                UpdateCurrentPathAndScanItIfNeeded(destination, target);
            }
        }
    }
}