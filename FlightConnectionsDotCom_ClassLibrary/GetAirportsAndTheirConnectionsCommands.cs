namespace FlightConnectionsDotCom_ClassLibrary
{
    public class GetAirportsAndTheirConnectionsCommands
    {
        public string GetPopularDestinationsDiv { get; } = "return document.querySelector('#popular-destinations')";
        public string GetShowMoreButton { get; } = "return document.querySelector('.show-all-destinations-btn')";
        public string GetPopularDestinationsEntries { get; } = "return arguments[0].querySelectorAll('.popular-destination')";
        public string GetDestinationFromEntry { get; } = "return arguments[0].getAttribute('data-a')";
    }
}