using System.Collections.Generic;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Interfaces
{
    public interface IFlightConnectionsDotComWorkerAirportCollector
    {
        List<Airport> CollectAirports(int maxCountToCollect = 0);
    }
}