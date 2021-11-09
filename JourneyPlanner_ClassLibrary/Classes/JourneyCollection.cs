using Common_ClassLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JourneyPlanner_ClassLibrary
{
    public class JourneyCollection
    {
        public List<Journey> Journeys { get; set; }
        public JourneyCollection()
        {
            Journeys = new();
        }
        
        public JourneyCollection(List<Journey> journeys)
        {
            Journeys = journeys;
        }

        public Journey this[int index]
        {
            get
            {
                return Journeys[index];
            }

            set
            {
                Journeys[index] = value;
            }
        }

        public int GetCount()
        {
            return Journeys.Count;
        }
        
        public int GetCountOfFlights()
        {
            return Journeys.Where(x => x.Type == JourneyType.Flight).Count();
        }
        
        public int GetCountOfBuses()
        {
            return Journeys.Where(x => x.Type == JourneyType.Local).Count();
        }
        
        public Journey GetFirst()
        {
            return Journeys[0];
        }
        
        public Journey GetLast()
        {
            return Journeys[GetCount() - 1];
        }

        public override string ToString()
        {
            return $"{Journeys.Count} flights";
        }
    }
}