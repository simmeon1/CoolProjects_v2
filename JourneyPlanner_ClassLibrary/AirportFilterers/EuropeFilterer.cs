using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.AirportFilterers
{
    public class EuropeFilterer : IAirportFilterer
    {
        public bool AirportMeetsCondition(Airport airport)
        {
            if (airport == null) return false;
            string country = airport.Country;
            return country.Contains("Albania") ||
                    country.Contains("Andorra") ||
                    country.Contains("Austria") ||
                    country.Contains("Belarus") ||
                    country.Contains("Belgium") ||
                    country.Contains("Bosnia") ||
                    country.Contains("Bulgaria") ||
                    country.Contains("Croatia") ||
                    country.Contains("Czech") ||
                    country.Contains("Denmark") ||
                    country.Contains("Estonia") ||
                    country.Contains("Finland") ||
                    country.Contains("France") ||
                    country.Contains("Germany") ||
                    country.Contains("Greece") ||
                    country.Contains("Holy See") ||
                    country.Contains("Hungary") ||
                    country.Contains("Iceland") ||
                    country.Contains("Ireland") ||
                    country.Contains("Italy") ||
                    country.Contains("Latvia") ||
                    country.Contains("Liechtenstein") ||
                    country.Contains("Lithuania") ||
                    country.Contains("Luxembourg") ||
                    country.Contains("Malta") ||
                    country.Contains("Moldova") ||
                    country.Contains("Monaco") ||
                    country.Contains("Montenegro") ||
                    country.Contains("Netherlands") ||
                    country.Contains("Macedonia") ||
                    country.Contains("Norway") ||
                    country.Contains("Poland") ||
                    country.Contains("Portugal") ||
                    country.Contains("Romania") ||
                    country.Contains("Russia") ||
                    country.Contains("San Marino") ||
                    country.Contains("Serbia") ||
                    country.Contains("Slovakia") ||
                    country.Contains("Slovenia") ||
                    country.Contains("Spain") ||
                    country.Contains("Sweden") ||
                    country.Contains("Switzerland") ||
                    country.Contains("Ukraine") ||
                    country.Contains("United Kingdom"); 
        }
    }
}