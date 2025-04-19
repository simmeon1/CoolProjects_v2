using System.Collections.Generic;
using System.Linq;
using JourneyPlanner_ClassLibrary.AirportFilterers;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class AirportListFilterer
    {
        private readonly List<Airport> airports;

        public AirportListFilterer(List<Airport> airports)
        {
            this.airports = airports;
        }

        public Dictionary<string, HashSet<string>> FilterAirports(Dictionary<string, HashSet<string>> fullAirportsAndDestinations, IAirportFilterer filterer)
        {
            Dictionary<string, HashSet<string>> filteredList = new();
            Dictionary<string, Airport> airportsDictionary = airports.ToDictionary(a => a.Code, a => a);
            foreach (KeyValuePair<string, HashSet<string>> airportAndDestinations in fullAirportsAndDestinations)
            {
                var airport = airportsDictionary.GetValueOrDefault(airportAndDestinations.Key);
                if (airport == null || !filterer.AirportMeetsCondition(airport)) continue;
                filteredList.Add(airport.Code, new HashSet<string>());
                foreach (string destination in airportAndDestinations.Value)
                {
                    Airport airport2 = airportsDictionary.TryGetValue(destination, out Airport value1) ? value1 : null;
                    if (airport2 != null && filterer.AirportMeetsCondition(airport2)) filteredList[airport.Code].Add(airport2.Code);
                }
            }
            return filteredList;
        }
    }
}