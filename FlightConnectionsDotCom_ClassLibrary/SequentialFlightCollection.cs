using Common_ClassLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class SequentialFlightCollection : IEnumerable<Flight>
    {
        private FlightCollection FlightCollection { get; set; }

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

        public IEnumerator<Flight> GetEnumerator()
        {
            return FlightCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            foreach (Flight flight in FlightCollection) cost += flight.Cost;
            return cost;
        }

        public DateTime? GetStartTime()
        {
            return IsNotValid() ? null : FlightCollection.First().Departing;
        }

        public DateTime? GetEndTime()
        {
            return IsNotValid() ? null : FlightCollection.Last().Arriving;
        }

        public double GetTotalTime()
        {
            return IsNotValid() ? 0 : (GetEndTime().Value - GetStartTime().Value).TotalHours;
        }

        public double GetTotalTimeInFlights()
        {
            double time = 0;
            if (IsNotValid()) return time;
            foreach (Flight flight in FlightCollection) time += flight.Duration.TotalHours;
            return time;
        }

        public double GetTotalIdleTime()
        {
            return IsNotValid() ? 0 : GetTotalTime() - GetTotalTimeInFlights();
        }
        
        public bool StartsAndEndsOnSameDay()
        {
            return !IsNotValid() && GetStartTime().Value.Day == GetEndTime().Value.Day;
        }
    }
}