using Common_ClassLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class FlightCollection
    {
        public List<Flight> Flights { get; set; }
        public FlightCollection()
        {
            Flights = new();
        }
        
        public FlightCollection(List<Flight> flights)
        {
            Flights = flights;
        }

        public Flight this[int index]
        {
            get
            {
                return Flights[index];
            }

            set
            {
                Flights[index] = value;
            }
        }

        public int GetCount()
        {
            return Flights.Count;
        }
        
        public int GetCountOfFlights()
        {
            return Flights.Where(x => x.Type == JourneyType.Flight).Count();
        }
        
        public int GetCountOfBuses()
        {
            return Flights.Where(x => x.Type == JourneyType.Bus).Count();
        }
        
        public Flight GetFirst()
        {
            return Flights[0];
        }
        
        public Flight GetLast()
        {
            return Flights[GetCount() - 1];
        }

        public override string ToString()
        {
            return $"{Flights.Count} flights";
        }
    }
}