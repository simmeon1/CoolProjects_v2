using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FlightConnectionsDotCom_Console
{
    public class Parameters
    {
        public List<string> Origins { get; set; }
        public List<string> Destinations { get; set; }
        public int MaxFlights { get; set; }
        public DateTime Date { get; set; }
        public string LocalAirportListFile { get; set; }
        public string LocalAirportDestinationsFile { get; set; }
        public bool OpenGoogleFlights { get; set; }
        public string FileSavePath { get; set; }
        public bool EuropeOnly{ get; set; }
        public bool UKAndBulgariaOnly{ get; set; }
    }
}