using Common_ClassLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class FlightCollection : IEnumerable<Flight>
    {
        private List<Flight> Flights { get; set; }
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

        public IEnumerator<Flight> GetEnumerator()
        {
            return Flights.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public int Count()
        {
            return Flights.Count;
        }
    }
}