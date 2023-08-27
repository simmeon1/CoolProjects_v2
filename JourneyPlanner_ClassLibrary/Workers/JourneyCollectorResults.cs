using System.Collections.Generic;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class JourneyCollectorResults
    {
        public List<string> Origins { get; set; }
        public List<string> Destinations { get; set; }
        public int MaxFlights { get; set; }
        public JourneyCollection JourneyCollection { get; set; }
    }
}