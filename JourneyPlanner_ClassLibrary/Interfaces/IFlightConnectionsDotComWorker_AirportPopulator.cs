using System.Collections.Generic;
using JourneyPlanner_ClassLibrary.AirportFilterers;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Interfaces
{
    public interface IFlightConnectionsDotComWorkerAirportPopulator
    {
        Dictionary<string, HashSet<string>> PopulateAirports(List<Airport> airportsList, IAirportFilterer filterer = null);
    }
}