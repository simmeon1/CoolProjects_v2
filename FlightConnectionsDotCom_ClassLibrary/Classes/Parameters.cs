using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class Parameters
    {
        public List<string> Origins { get; set; }
        public List<string> Destinations { get; set; }
        public int MaxFlights { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string LocalAirportListFile { get; set; }
        public string LocalAirportDestinationsFile { get; set; }
        public string LocalGoogleFlightsWorkerResultsFile { get; set; }
        public string LocalCollectedPathJourneysFile { get; set; }
        public bool SkipUndoableJourneys { get; set; }
        public bool SkipNotSameDayFinishJourneys { get; set; }
        public bool OpenGoogleFlights { get; set; }
        public string FileSavePath { get; set; }
        public bool EuropeOnly { get; set; }
        public bool UKAndBulgariaOnly { get; set; }
        public int DefaultDelay { get; set; }
        public int NoLongerThan { get; set; }
        public bool OnlyIncludeShortestPaths { get; set; }
        public bool Headless { get; set; }
    }
}