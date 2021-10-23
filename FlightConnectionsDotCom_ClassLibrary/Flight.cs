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
        public Airport DepartingAirport { get; set; }
        public Airport ArrivingAirport { get; set; }
        public int Cost { get; set; }
        public Flight(DateTime departing, DateTime arriving, string airline, TimeSpan duration, Airport departingAirport, Airport arrivingAirport, int cost)
        {
            Departing = departing;
            Arriving = arriving;
            Airline = airline;
            Duration = duration;
            DepartingAirport = departingAirport;
            ArrivingAirport = arrivingAirport;
            Cost = cost;
        }

        public override string ToString()
        {
            return $"{GetDepartingAirport().Code}-{GetArrivingAirport().Code} - {Departing} - {Arriving} - {Airline} - {Duration} - {Cost}";
        }
        
        public Airport GetDepartingAirport()
        {
            return DepartingAirport;
        }
        
        public Airport GetArrivingAirport()
        {
            return ArrivingAirport;
        }
        
        public string GetPath()
        {
            return $"{DepartingAirport.Code}-{ArrivingAirport.Code}";
        }
    }
}