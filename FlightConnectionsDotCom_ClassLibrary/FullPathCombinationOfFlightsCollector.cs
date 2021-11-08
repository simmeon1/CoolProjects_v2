using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class FullPathCombinationOfFlightsCollector
    {
        private FullPathAndListOfPathsAndFlightCollections DataWithFlightsForSinglePaths { get; set; }

        public List<SequentialFlightCollection> GetFullPathCombinationOfFLights(FullPathAndListOfPathsAndFlightCollections dataWithFlightsForSinglePaths)
        {
            DataWithFlightsForSinglePaths = dataWithFlightsForSinglePaths;
            string fullPathName = DataWithFlightsForSinglePaths.Path.ToString();
            List<PathAndFlightCollection> pathsAndFlights = DataWithFlightsForSinglePaths.PathsAndFlightCollections;
            List<SequentialFlightCollection> fullPathCombinationsOfFlights = new();
            LinkedList<Flight> flight = new();
            BuildUpCombinationOfFlights(0, flight, fullPathCombinationsOfFlights);
            return fullPathCombinationsOfFlights;
        }

        private void BuildUpCombinationOfFlights(int index, LinkedList<Flight> listOfFlights, List<SequentialFlightCollection> combos)
        {
            int count = DataWithFlightsForSinglePaths.PathsAndFlightCollections.Count;
            for (int i = index; i < count; i++)
            {
                PathAndFlightCollection pathAndFlights = DataWithFlightsForSinglePaths.PathsAndFlightCollections[i];
                FlightCollection pathFlights = pathAndFlights.FlightCollection;
                for (int j = 0; j < pathFlights.GetCount(); j++)
                {
                    listOfFlights.AddLast(pathFlights[j]);
                    if (index < count - 1) BuildUpCombinationOfFlights(i + 1, listOfFlights, combos);
                    else if (listOfFlights.Count == count) combos.Add(new SequentialFlightCollection(new(listOfFlights.ToList())));
                    listOfFlights.RemoveLast();
                }
            }
        }
    }
}