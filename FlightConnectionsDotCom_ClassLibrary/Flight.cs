using System;
using System.Text.RegularExpressions;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class Flight
    {
        public DateTime Departing { get; set; }
        public DateTime Arriving { get; set; }
        public string Airline { get; set; }
        public TimeSpan Duration { get; set; }
        public string Path { get; set; }
        public int Cost { get; set; }
        public Flight(DateTime departing, DateTime arriving, string airline, TimeSpan duration, string path, int cost)
        {
            Departing = departing;
            Arriving = arriving;
            Airline = airline;
            Duration = duration;
            Path = path;
            Cost = cost;
        }

        public override string ToString()
        {
            return $"{Path} - {Departing} - {Arriving} - {Cost}";
        }
        
        public string GetDepartingAirport()
        {
            return Regex.Replace(Path, @"(\w+)-\w+", "$1");
        }
        
        public string GetArrivingAirport()
        {
            return Regex.Replace(Path, @"\w+-(\w+)", "$1");
        }
    }
}