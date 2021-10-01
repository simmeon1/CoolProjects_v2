using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class FullPathCombinationOfFlightsCollector
    {
        private KeyValuePair<Path, List<KeyValuePair<Path, List<Flight>>>> DataWithFlightsForSinglePaths { get; set; }

        public FullPathCombinationOfFlightsCollector(KeyValuePair<Path, List<KeyValuePair<Path, List<Flight>>>> dataWithFlightsForSinglePaths)
        {
            DataWithFlightsForSinglePaths = dataWithFlightsForSinglePaths;
        }

        public List<List<Flight>> GetFullPathCombinationOfFLights()
        {
            string fullPathName = DataWithFlightsForSinglePaths.Key.ToString();
            List<KeyValuePair<Path, List<Flight>>> pathsAndFlights = DataWithFlightsForSinglePaths.Value;

            if (pathsAndFlights.Count == 0) return new();
            if (pathsAndFlights.Count == 1) return new() { pathsAndFlights[0].Value };

            List<List<Flight>> fullPathCombinationsOfFlights = new();
            LinkedList<Flight> flight = new();
            BuildUpCombinationOfFlights(0, flight, fullPathCombinationsOfFlights);
            return fullPathCombinationsOfFlights;
        }

        private void BuildUpCombinationOfFlights(int index, LinkedList<Flight> x, List<List<Flight>> combos)
        {
            for (int i = 0; i < DataWithFlightsForSinglePaths.Value.Count; i++)
            {
                KeyValuePair<Path, List<Flight>> pathAndFlights = DataWithFlightsForSinglePaths.Value[i];
                if (i < index) continue;
                List<Flight> pathFlights = pathAndFlights.Value;
                foreach (Flight pathFlight in pathFlights)
                {
                    x.AddLast(pathFlight);
                    if (index < DataWithFlightsForSinglePaths.Value.Count - 1) BuildUpCombinationOfFlights(i + 1, x, combos);
                    else if (x.Count == DataWithFlightsForSinglePaths.Value.Count) combos.Add(x.ToList());
                    x.RemoveLast();
                }
            }
        }
    }
}