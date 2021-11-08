using Common_ClassLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class PathAndFlightCollection
    {
        public Path Path { get; set; }
        public FlightCollection FlightCollection { get; set; }
        public PathAndFlightCollection(Path path, FlightCollection flightCollection)
        {
            Path = path;
            FlightCollection = flightCollection;
        }

        public override string ToString()
        {
            return $"{Path}, {FlightCollection.GetCountOfFlights()} flights, {FlightCollection.GetCountOfBuses()} buses";
        }
    }
}