using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.AirportFilterers
{
    public class NoFilterer : IAirportFilterer
    {
        public bool AirportMeetsCondition(Airport airport)
        {
            return airport != null;
        }
    }
}