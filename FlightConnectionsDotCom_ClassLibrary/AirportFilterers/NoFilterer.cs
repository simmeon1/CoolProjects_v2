namespace FlightConnectionsDotCom_ClassLibrary
{
    public class NoFilterer : IAirportFilterer
    {
        public bool AirportMeetsCondition(Airport airport)
        {
            return airport != null;
        }
    }
}