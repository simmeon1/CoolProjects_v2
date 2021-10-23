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
        public ChromeWorkerResults(Dictionary<string, FlightCollection> pathsAndFlights, List<FullPathAndListOfPathsAndFlightCollections> fullPathsAndFlightCollections)
        {
            FullPathsAndFlightCollections = fullPathsAndFlightCollections;
            PathsAndFlights = pathsAndFlights;
        }
    }
}