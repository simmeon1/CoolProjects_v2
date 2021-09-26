using System;
using System.Diagnostics;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class Parameters
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int MaxFlights { get; set; }
        public DateTime Date { get; set; }
        public string LocalAirportListFile { get; set; }
        public string LocalAirportDestinationsFile { get; set; }
        public bool OpenGoogleFlights { get; set; }
        public string FileSavePath { get; set; }
    }
}