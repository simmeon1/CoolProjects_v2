using System.Collections.Generic;

namespace JourneyPlanner_ClassLibrary
{
    public interface IFlightConnectionsDotComWorker_AirportPopulator
    {
        Dictionary<string, HashSet<string>> PopulateAirports(List<Airport> airportsList, IAirportFilterer filterer = null);
    }
}