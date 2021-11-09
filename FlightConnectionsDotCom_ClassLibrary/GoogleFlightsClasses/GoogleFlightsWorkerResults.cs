using Common_ClassLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class GoogleFlightsWorkerResults
    {
        public Dictionary<string, JourneyCollection> PathsAndJourneys { get; set; }
        public List<FullPathAndListOfPathsAndJourneyCollections> FullPathsAndJourneyCollections { get; set; }
        public bool Success { get; set; }

        public GoogleFlightsWorkerResults(bool success, Dictionary<string, JourneyCollection> pathsAndJourneys, List<FullPathAndListOfPathsAndJourneyCollections> fullPathsAndJourneyCollections)
        {
            Success = success;
            FullPathsAndJourneyCollections = fullPathsAndJourneyCollections;
            PathsAndJourneys = pathsAndJourneys;
        }
    }
}