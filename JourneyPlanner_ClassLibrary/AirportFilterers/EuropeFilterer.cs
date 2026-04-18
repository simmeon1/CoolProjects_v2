using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.AirportFilterers;

/// <summary>
/// List from https://www.flightconnections.com/airports-by-country. Asked Gemini which ones are in Europe. Does not
/// necessarily include every EU country if it doesn't have an airport. See previous version for all EU countries.
/// </summary>
public class EuropeFilterer : IAirportFilterer
{
    private static readonly string[] Countries =
    [
        "Albania",
        "Armenia",
        "Austria",
        "Azerbaijan",
        "Belarus",
        "Belgium",
        "Bosnia & Herzegovina",
        "Bulgaria",
        "Croatia",
        "Cyprus",
        "Czechia",
        "Denmark",
        "Estonia",
        "Faroe Islands",
        "Finland",
        "France",
        "Georgia",
        "Germany",
        "Gibraltar",
        "Greece",
        "Guernsey",
        "Hungary",
        "Iceland",
        "Ireland",
        "Isle of Man",
        "Italy",
        "Jersey",
        "Kazakhstan",
        "Kosovo",
        "Latvia",
        "Lithuania",
        "Luxembourg",
        "Malta",
        "Moldova",
        "Monaco",
        "Montenegro",
        "Netherlands",
        "Norway",
        "Poland",
        "Portugal",
        "Republic of North Macedonia",
        "Romania",
        "Russia",
        "Serbia",
        "Slovakia",
        "Slovenia",
        "Spain",
        "Sweden",
        "Switzerland",
        "Turkey",
        "United Kingdom"
    ];

    public bool AirportMeetsCondition(Airport airport)
    {
        var country = airport.Country;
        for (int i = 0; i < Countries.Length; i++)
        {
            if (country.Contains(Countries[i]))
            {
                return true;
            }
        }
        return false;
    }
}