namespace JourneyPlanner_ClassLibrary
{
    public class UKBulgariaFilterer : IAirportFilterer
    {
        public bool AirportMeetsCondition(Airport airport)
        {
            return airport != null && (airport.Country.Contains("Bulgaria") || airport.Country.Contains("United Kingdom"));
        }
    }
}