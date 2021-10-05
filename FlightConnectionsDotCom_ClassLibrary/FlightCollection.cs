using Common_ClassLibrary;
using System.Collections;
using System.Collections.Generic;
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

        public int Count()
        {
            return Flights.Count;
        }
        
        public Flight GetFirst()
        {
            return Flights[0];
        }
        
        public Flight GetLast()
        {
            return Flights[Count() - 1];
        }
    }
}