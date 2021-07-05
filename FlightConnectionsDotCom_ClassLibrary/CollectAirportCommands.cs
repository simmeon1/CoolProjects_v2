namespace FlightConnectionsDotCom_ClassLibrary
{
    public class CollectAirportCommands
    {
        public string GetAirportListEntries { get; } = "return document.getElementsByTagName('li')";
        public string GetAirportCodeFromEntry { get; } = "return arguments[0].querySelector('.airport-code').innerText";
        public string GetAirportCityAndCountryFromEntry { get; } = "return arguments[0].querySelector('.airport-city-country').innerText";
        public string GetAirportNameFromEntry { get; } = "return arguments[0].querySelector('.airport-name').innerText";
        public string GetAirportLinkFromEntry { get; } = "return arguments[0].querySelector('a').href";
    }
}