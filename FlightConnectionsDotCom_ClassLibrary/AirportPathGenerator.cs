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
        private Dictionary<string, HashSet<string>> AirportsAndDestinationsFiltered { get; set; }
        private LinkedList<string> CurrentPath { get; set; }
        private int MaxFlights { get; set; }
        private List<Path> Paths { get; set; }
        private string Target { get; set; }

        public AirportPathGenerator(Dictionary<string, HashSet<string>> airportsAndDestinations)
        {
            AirportsAndDestinations = airportsAndDestinations;
        }

        public List<Path> GeneratePaths(List<string> origins, List<string> targets, int maxFlights)
        {
            AirportsAndDestinationsFiltered = AirportsAndDestinations;
            return GetPaths(origins, targets, maxFlights);
        }
        
        public List<Path> GeneratePaths(List<string> origins, List<string> targets, int maxFlights, List<Airport> airports, IAirportFilterer filterer = null)
        {
            if (filterer == null) filterer = new NoFilterer();
            AirportsAndDestinationsFiltered = new();

            Dictionary<string, Airport> airportsDictionary = airports.ToDictionary(a => a.Code, a => a);
            foreach (KeyValuePair<string, HashSet<string>> airportAndDestinations in AirportsAndDestinations)
            {
                Airport airport = airportsDictionary.ContainsKey(airportAndDestinations.Key) ? airportsDictionary[airportAndDestinations.Key] : null;
                if (airport == null || !filterer.AirportMeetsCondition(airport)) continue;
                AirportsAndDestinationsFiltered.Add(airport.Code, new HashSet<string>());
                foreach (string destination in airportAndDestinations.Value)
                {
                    Airport airport2 = airportsDictionary.ContainsKey(destination) ? airportsDictionary[destination] : null;
                    if (airport2 != null && filterer.AirportMeetsCondition(airport2)) AirportsAndDestinationsFiltered[airport.Code].Add(airport2.Code);
                }
            }

            return GetPaths(origins, targets, maxFlights);
        }

        private List<Path> GetPaths(List<string> origins, List<string> targets, int maxFlights)
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
            HashSet<string> destinations = AirportsAndDestinationsFiltered[origin];
            foreach (string destination in destinations)
            {
                if (destination.Equals(target)) Paths.Add(new Path(new List<string>(CurrentPath) { destination }));
                UpdateCurrentPathAndScanItIfNeeded(destination, target);
            }
        }
    }
}