using Common_ClassLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class SequentialFlightCollection
    {
        public FlightCollection FlightCollection { get; set; }

        public SequentialFlightCollection()
        {
            FlightCollection = new();
        }

        public SequentialFlightCollection(FlightCollection flightCollection)
        {
            if (flightCollection != null && flightCollection.Count() > 1) DoSequenceCheck(flightCollection);
            FlightCollection = flightCollection;
        }

        private static void DoSequenceCheck(FlightCollection flightCollection)
        {
            Flight previousFlight = flightCollection[0];
            for (int i = 1; i < flightCollection.Count(); i++)
            {
                if (!previousFlight.GetArrivingAirport().Equals(flightCollection[i].GetDepartingAirport()))
                {
                    throw new Exception("Flights are not connected.");
                }
                previousFlight = flightCollection[i];
            }
        }

        public Flight this[int index]
        {
            get
            {
                return FlightCollection[index];
            }

            set
            {
                Flight originalFlight = FlightCollection[index];
                FlightCollection[index] = value;
                try
                {
                    DoSequenceCheck(FlightCollection);
                }
                catch (Exception)
                {
                    FlightCollection[index] = originalFlight;
                    throw;
                }
            }
        }

        private bool IsNotValid()
        {
            return FlightCollection == null || FlightCollection.Count() == 0;
        }
        
        public int Count()
        {
            return IsNotValid() ? 0 : FlightCollection.Count();
        }

        public override string ToString()
        {
            return IsNotValid() ? "No flights in sequence." : $"{GetFullPath()}, Doable = {SequenceIsDoable()}, Start = {GetStartTime()}, End = {GetEndTime()}, Cost = {GetCost()}";
        }

        public string GetFullPath()
        {
            StringBuilder path = new("");
            path.Append(FlightCollection[0].Path);
            for (int i = 1; i < FlightCollection.Count(); i++)
            {
                Flight flight = FlightCollection[i];
                path.Append($"-{flight.GetArrivingAirport()}");
            }
            return path.ToString();
        }

        public bool SequenceIsDoable()
        {
            if (IsNotValid()) return false;
            if (FlightCollection.Count() == 1) return true;
            Flight previousFlight = FlightCollection[0];
            for (int i = 1; i < FlightCollection.Count(); i++)
            {
                if ((FlightCollection[i].Departing - previousFlight.Arriving).TotalMinutes < 120) return false;
                previousFlight = FlightCollection[i];
            }
            return true;
        }

        public int GetCost()
        {
            int cost = 0;
            if (IsNotValid()) return cost;
            for (int i = 0; i < FlightCollection.Count(); i++) cost += FlightCollection[i].Cost;
            return cost;
        }

        public DateTime? GetStartTime()
        {
            return IsNotValid() ? null : FlightCollection.GetFirst().Departing;
        }

        public DateTime? GetEndTime()
        {
            return IsNotValid() ? null : FlightCollection.GetLast().Arriving;
        }

        public int GetCountOfFlights()
        {
            return IsNotValid() ? 0 : FlightCollection.Count();
        }

        public TimeSpan GetLength()
        {
            if (IsNotValid()) return new();
            TimeSpan time = new();
            for (int i = 0; i < FlightCollection.Count(); i++)
            {
                Flight flight = FlightCollection[i];
                TimeSpan waitFromPrev = i == 0 ? new TimeSpan() : (flight.Departing - FlightCollection[i - 1].Arriving);
                time += waitFromPrev + flight.Duration;
            }
            return time;
        }
        
        public bool StartsAndEndsOnSameDay()
        {
            return !IsNotValid() && GetStartTime().Value.Day == GetEndTime().Value.Day;
        }
        
        public bool HasFlightWithZeroCost()
        {
            if (IsNotValid()) return false;
            for (int i = 0; i < FlightCollection.Count(); i++) if (FlightCollection[i].Cost == 0) return true;
            return false;
        }
    }
}