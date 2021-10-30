using Common_ClassLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class ChromeWorkerResults
    {
        public Dictionary<string, FlightCollection> PathsAndFlights { get; set; }
        public List<FullPathAndListOfPathsAndFlightCollections> FullPathsAndFlightCollections { get; set; }
        public bool Success { get; set; }

        public ChromeWorkerResults(bool success, Dictionary<string, FlightCollection> pathsAndFlights, List<FullPathAndListOfPathsAndFlightCollections> fullPathsAndFlightCollections)
        {
            Success = success;
            FullPathsAndFlightCollections = fullPathsAndFlightCollections;
            PathsAndFlights = pathsAndFlights;
        }
    }
}