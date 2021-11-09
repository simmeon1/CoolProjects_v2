using Common_ClassLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class PathAndJourneyCollection
    {
        public Path Path { get; set; }
        public JourneyCollection JourneyCollection { get; set; }
        public PathAndJourneyCollection(Path path, JourneyCollection flightCollection)
        {
            Path = path;
            JourneyCollection = flightCollection;
        }

        public override string ToString()
        {
            return $"{Path}, {JourneyCollection.GetCountOfFlights()} flights, {JourneyCollection.GetCountOfBuses()} buses";
        }
    }
}