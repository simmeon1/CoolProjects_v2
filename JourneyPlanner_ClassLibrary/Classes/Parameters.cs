using System;
using System.Collections.Generic;

namespace JourneyPlanner_ClassLibrary.Classes
{
    public class Parameters
    {
        public List<string> Origins { get; set; }
        public List<string> Destinations { get; set; }
        public int MaxFlights { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string FileSavePath { get; set; }
        public string AirportListFile { get; set; }
        public string AirportDestinationsFile { get; set; }
        public int NoLongerThan { get; set; }
        public bool Headless { get; set; }
    }
}