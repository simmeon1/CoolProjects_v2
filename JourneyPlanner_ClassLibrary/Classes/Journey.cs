using System;
using System.Text.RegularExpressions;

namespace JourneyPlanner_ClassLibrary
{
    public class Journey
    {
        public DateTime Departing { get; set; }
        public DateTime Arriving { get; set; }
        public string Airline { get; set; }
        public TimeSpan Duration { get; set; }
        public string Path { get; set; }
        public int Cost { get; set; }
        public JourneyType Type { get; set; }
        public Journey(DateTime departing, DateTime arriving, string airline, TimeSpan duration, string path, int cost, JourneyType type = JourneyType.Flight)
        {
            Departing = departing;
            Arriving = arriving;
            Airline = airline;
            Duration = duration;
            Path = path;
            Cost = cost;
            Type = type;
        }

        public override string ToString()
        {
            return $"{GetDepartingAirport()}-{GetArrivingAirport()} - {Departing} - {Arriving} - {Airline} - {Duration} - {Cost} - {Type}";
        }
        
        public string GetDepartingAirport()
        {
            return Regex.Replace(Path, @"(\w+)\W+\w+", "$1");
        }
        
        public string GetArrivingAirport()
        {
            return Regex.Replace(Path, @"\w+\W+(\w+)", "$1");
        }
    }
}