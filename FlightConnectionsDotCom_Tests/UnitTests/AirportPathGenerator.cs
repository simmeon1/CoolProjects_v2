using FlightConnectionsDotCom_ClassLibrary;
using System;
using System.Collections.Generic;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    public class AirportPathGenerator
    {
        private IEnumerable<Airport> Airports { get; set; }
        private IDictionary<string, HashSet<string>> AirportDestinations { get; set; }
        private LinkedList<string> CurrentPath { get; set; }
        private int MaxFlights { get; set; }
        private List<List<string>> Paths { get; set; }

        public AirportPathGenerator(IEnumerable<Airport> airports, IDictionary<string, HashSet<string>> airportDestinations)
        {
            Airports = airports;
            AirportDestinations = airportDestinations;
        }

        public List<List<string>> GeneratePaths(string origin, string target, int maxFlights)
        {
            Initialise(maxFlights);
            ScanPathAndUpdateCurrentPath(origin, target);
            return Paths;
        }

        private void ScanPathAndUpdateCurrentPath(string origin, string target)
        {
            CurrentPath.AddLast(origin);
            ScanPath(origin, target);
            CurrentPath.RemoveLast();
        }

        private void Initialise(int maxFlights)
        {
            Paths = new List<List<string>>();
            CurrentPath = new LinkedList<string>();
            MaxFlights = maxFlights;
        }

        private void ScanPath(string origin, string target)
        {
            if (CurrentPath.Count - 1 >= MaxFlights) return;
            HashSet<string> destinations = AirportDestinations[origin];
            foreach (string destination in destinations)
            {
                if (destination.Equals(target)) Paths.Add(new List<string>(CurrentPath) { destination });
                ScanPathAndUpdateCurrentPath(destination, target);
            }
        }
    }
}