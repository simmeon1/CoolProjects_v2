using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.AirportFilterers
{
    public class UKBulgariaFilterer : IAirportFilterer
    {
        public bool AirportMeetsCondition(Airport airport)
        {
            return airport != null && (airport.Country.Contains("Bulgaria") || airport.Country.Contains("United Kingdom"));
        }
    }
}