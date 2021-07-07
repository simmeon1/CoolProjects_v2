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

        public List<List<string>> GeneratePaths(string origin, string destination)
        {
            List<List<string>> paths = new();
            ICollection<string> destinations1 = AirportDestinations[origin];
            foreach (string destination1 in destinations1)
            {
                if (destination1.Equals(destination))
                {
                    paths.Add(new List<string>() { origin, destination1 });
                }
                ICollection<string> destinations2 = AirportDestinations[destination1];
                foreach (string destination2 in destinations2)
                {
                    if (destination2.Equals(destination))
                    {
                        paths.Add(new List<string>() { origin, destination1, destination2 });
                    }
                    ICollection<string> destinations3 = AirportDestinations[destination2];
                    foreach (string destination3 in destinations3)
                    {
                        if (destination3.Equals(destination))
                        {
                            paths.Add(new List<string>() { origin, destination1, destination2, destination3 });
                        }
                    }
                }
            }
            return paths;
        }
    }
}