using System.Collections.Generic;

namespace JourneyPlanner_ClassLibrary
{
    public interface IFlightConnectionsDotComWorker_AirportCollector
    {
        List<Airport> CollectAirports(int maxCountToCollect = 0);
    }
}