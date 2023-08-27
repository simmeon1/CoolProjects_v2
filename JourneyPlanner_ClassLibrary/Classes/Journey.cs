using System;
using System.Text.RegularExpressions;
using JourneyPlanner_ClassLibrary.JourneyRetrievers;

namespace JourneyPlanner_ClassLibrary.Classes
{
    public class Journey
    {
        public DateTime Departing { get; }
        public DateTime Arriving { get; }
        public string Company { get; }
        public TimeSpan Duration { get; }
        public string Path { get; }
        public double Cost { get; }
        public Journey(DateTime departing, DateTime arriving, string company, TimeSpan duration, string path, double cost)
        {
            Departing = departing;
            Arriving = arriving;
            Company = company;
            Duration = duration;
            Path = path;
            Cost = cost;
        }

        public override string ToString()
        {
            return $"{GetDepartingLocation()}-{GetArrivingLocation()} - {Departing} - {Arriving} - {Company} - {Duration} - {Cost}";
        }
        
        public string GetDepartingLocation()
        {
            return Regex.Replace(Path, @"(\w+)\W+\w+", "$1");
        }
        
        public string GetArrivingLocation()
        {
            return Regex.Replace(Path, @"\w+\W+(\w+)", "$1");
        }
    }
}