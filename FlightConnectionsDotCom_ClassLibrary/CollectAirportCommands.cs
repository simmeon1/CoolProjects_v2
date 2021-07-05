namespace FlightConnectionsDotCom_ClassLibrary
{
    public class CollectAirportCommands
    {
        public string GetAirportLists { get; } = "return document.querySelectorAll('.airport-list')";
        public string GetAirportListEntries { get; } = "return arguments[0].getElementsByTagName('li')";
        public string GetAirportCodeFromEntry { get; } = "return arguments[0].querySelector('.airport-code').innerText";
        public string GetAirportCityAndCountryFromEntry { get; } = "return arguments[0].querySelector('.airport-city-country').innerText";
        public string GetAirportNameFromEntry { get; } = "return arguments[0].querySelector('.airport-name').innerText";
    }
}