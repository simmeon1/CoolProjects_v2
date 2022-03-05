using System.Collections.Generic;
using System.Linq;
using JourneyPlanner_ClassLibrary.AirportFilterers;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class AirportListFilterer
    {
        public List<Airport> Airports { get; set; }

        public AirportListFilterer(List<Airport> airports)
        {
            Airports = airports;
        }

        public Dictionary<string, HashSet<string>> FilterAirports(Dictionary<string, HashSet<string>> fullAirportsAndDestinations, IAirportFilterer filterer)
        {
            if (filterer == null) filterer = new NoFilterer();
            Dictionary<string, HashSet<string>> filteredList = new();

            Dictionary<string, Airport> airportsDictionary = Airports.ToDictionary(a => a.Code, a => a);
            foreach (KeyValuePair<string, HashSet<string>> airportAndDestinations in fullAirportsAndDestinations)
            {
                Airport airport = airportsDictionary.ContainsKey(airportAndDestinations.Key) ? airportsDictionary[airportAndDestinations.Key] : null;
                if (airport == null || !filterer.AirportMeetsCondition(airport)) continue;
                filteredList.Add(airport.Code, new HashSet<string>());
                foreach (string destination in airportAndDestinations.Value)
                {
                    Airport airport2 = airportsDictionary.ContainsKey(destination) ? airportsDictionary[destination] : null;
                    if (airport2 != null && filterer.AirportMeetsCondition(airport2)) filteredList[airport.Code].Add(airport2.Code);
                }
            }
            return filteredList;
        }
    }
}