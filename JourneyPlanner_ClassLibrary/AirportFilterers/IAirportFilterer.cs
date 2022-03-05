using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.AirportFilterers
{
    public interface IAirportFilterer
    {
        bool AirportMeetsCondition(Airport airport);
    }
}