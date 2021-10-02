using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class FullPathCombinationOfFlightsCollector
    {
        private KeyValuePair<Path, List<KeyValuePair<Path, FlightCollection>>> DataWithFlightsForSinglePaths { get; set; }

        public List<SequentialFlightCollection> GetFullPathCombinationOfFLights(KeyValuePair<Path, List<KeyValuePair<Path, FlightCollection>>> dataWithFlightsForSinglePaths)
        {
            DataWithFlightsForSinglePaths = dataWithFlightsForSinglePaths;
            string fullPathName = DataWithFlightsForSinglePaths.Key.ToString();
            List<KeyValuePair<Path, FlightCollection>> pathsAndFlights = DataWithFlightsForSinglePaths.Value;

            if (pathsAndFlights.Count == 0) return new();
            if (pathsAndFlights.Count == 1) return new() { new(pathsAndFlights[0].Value )};

            List<SequentialFlightCollection> fullPathCombinationsOfFlights = new();
            LinkedList<Flight> flight = new();
            BuildUpCombinationOfFlights(0, flight, fullPathCombinationsOfFlights);
            return fullPathCombinationsOfFlights;
        }

        private void BuildUpCombinationOfFlights(int index, LinkedList<Flight> listOfFlights, List<SequentialFlightCollection> combos)
        {
            for (int i = index; i < DataWithFlightsForSinglePaths.Value.Count; i++)
            {
                KeyValuePair<Path, FlightCollection> pathAndFlights = DataWithFlightsForSinglePaths.Value[i];
                FlightCollection pathFlights = pathAndFlights.Value;
                foreach (Flight pathFlight in pathFlights)
                {
                    listOfFlights.AddLast(pathFlight);
                    if (index < DataWithFlightsForSinglePaths.Value.Count - 1) BuildUpCombinationOfFlights(i + 1, listOfFlights, combos);
                    else if (listOfFlights.Count == DataWithFlightsForSinglePaths.Value.Count) combos.Add(new SequentialFlightCollection(new(listOfFlights.ToList())));
                    listOfFlights.RemoveLast();
                }
            }
        }
    }
}