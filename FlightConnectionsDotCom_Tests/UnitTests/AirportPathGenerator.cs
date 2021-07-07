using FlightConnectionsDotCom_ClassLibrary;
using System;
using System.Collections.Generic;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    public class AirportPathGenerator
    {
        private IEnumerable<Airport> Airports { get; set; }
        private IDictionary<string, HashSet<string>> AirportDestinations { get; set; }

        public AirportPathGenerator(IEnumerable<Airport> airports, IDictionary<string, HashSet<string>> airportDestinations)
        {
            Airports = airports;
            AirportDestinations = airportDestinations;
        }

        public List<List<string>> GeneratePaths(string origin, string target, int maxFlights)
        {
            List<List<string>> paths = new();
            LinkedList<string> currentPath = new();
            currentPath.AddLast(origin);
            int depthScanned = 0;
            GeneratePath(origin, target, paths, ref depthScanned, maxFlights, currentPath);
            return paths;
        }

        private void GeneratePath(string origin, string target, List<List<string>> paths, ref int depthScanned, int maxFlights, LinkedList<string> currentPath)
        {
            while (depthScanned < maxFlights)
            {
                HashSet<string> destinations = AirportDestinations[origin];
                foreach (string destination in destinations)
                {
                    if (destination.Equals(target)) paths.Add(new List<string>(currentPath) { destination });
                    depthScanned++;
                    currentPath.AddLast(destination);
                    GeneratePath(destination, target, paths, ref depthScanned, maxFlights, currentPath);
                    currentPath.RemoveLast();
                }
            }
        }
    }
}