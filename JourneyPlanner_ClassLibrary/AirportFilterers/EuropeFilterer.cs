using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.AirportFilterers;

/// <summary>
/// List from https://www.flightconnections.com/airports-by-country. Asked Gemini which ones are in Europe. Does not
/// necessarily include every EU country if it doesn't have an airport. See previous version for all EU countries.
/// </summary>
public class EuropeFilterer : IAirportFilterer
{
    public bool AirportMeetsCondition(Airport airport)
    {
        var country = airport.Country;
        return country.Contains("Albania") ||
            country.Contains("Armenia") ||
            country.Contains("Austria") ||
            country.Contains("Azerbaijan") ||
            country.Contains("Belarus") ||
            country.Contains("Belgium") ||
            country.Contains("Bosnia & Herzegovina") ||
            country.Contains("Bulgaria") ||
            country.Contains("Croatia") ||
            country.Contains("Cyprus") ||
            country.Contains("Czechia") ||
            country.Contains("Denmark") ||
            country.Contains("Estonia") ||
            country.Contains("Faroe Islands") ||
            country.Contains("Finland") ||
            country.Contains("France") ||
            country.Contains("Georgia") ||
            country.Contains("Germany") ||
            country.Contains("Gibraltar") ||
            country.Contains("Greece") ||
            country.Contains("Guernsey") ||
            country.Contains("Hungary") ||
            country.Contains("Iceland") ||
            country.Contains("Ireland") ||
            country.Contains("Isle of Man") ||
            country.Contains("Italy") ||
            country.Contains("Jersey") ||
            country.Contains("Kazakhstan") ||
            country.Contains("Kosovo") ||
            country.Contains("Latvia") ||
            country.Contains("Lithuania") ||
            country.Contains("Luxembourg") ||
            country.Contains("Malta") ||
            country.Contains("Moldova") ||
            country.Contains("Monaco") ||
            country.Contains("Montenegro") ||
            country.Contains("Netherlands") ||
            country.Contains("Norway") ||
            country.Contains("Poland") ||
            country.Contains("Portugal") ||
            country.Contains("Republic of North Macedonia") ||
            country.Contains("Romania") ||
            country.Contains("Russia") ||
            country.Contains("Serbia") ||
            country.Contains("Slovakia") ||
            country.Contains("Slovenia") ||
            country.Contains("Spain") ||
            country.Contains("Sweden") ||
            country.Contains("Switzerland") ||
            country.Contains("Turkey") ||
            country.Contains("United Kingdom");
    }
}