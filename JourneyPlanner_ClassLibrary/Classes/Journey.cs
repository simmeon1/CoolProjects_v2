using System;
using System.Text.RegularExpressions;

namespace JourneyPlanner_ClassLibrary
{
    public class Journey
    {
        private const string GoogleFlightsWorkerName = nameof(GoogleFlightsWorker);
        public DateTime Departing { get; set; }
        public DateTime Arriving { get; set; }
        public string Company { get; set; }
        public TimeSpan Duration { get; set; }
        public string Path { get; set; }
        public int Cost { get; set; }
        public string RetrievedWithWorker { get; set; }
        public Journey(DateTime departing, DateTime arriving, string company, TimeSpan duration, string path, int cost, string retrievedWithWorker = GoogleFlightsWorkerName)
        {
            Departing = departing;
            Arriving = arriving;
            Company = company;
            Duration = duration;
            Path = path;
            Cost = cost;
            RetrievedWithWorker = retrievedWithWorker;
        }

        public override string ToString()
        {
            return $"{GetDepartingAirport()}-{GetArrivingAirport()} - {Departing} - {Arriving} - {Company} - {Duration} - {Cost} - {RetrievedWithWorker}";
        }
        
        public string GetDepartingAirport()
        {
            return Regex.Replace(Path, @"(\w+)\W+\w+", "$1");
        }
        
        public string GetArrivingAirport()
        {
            return Regex.Replace(Path, @"\w+\W+(\w+)", "$1");
        }
        
        public bool IsFlight()
        {
            return RetrievedWithWorker.Equals(GoogleFlightsWorkerName);
        }
    }
}