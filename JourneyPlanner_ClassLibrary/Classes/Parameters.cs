﻿using System;
using System.Collections.Generic;

namespace JourneyPlanner_ClassLibrary.Classes
{
    public class Parameters
    {
        public string FileSavePath { get; set; }
        public string AirportListFile { get; set; }
        public string AirportDestinationsFile { get; set; }
        public string ExistingResultsPath { get; set; }
        public string TimePenaltiesFile { get; set; }
        public string CostPenaltiesFile { get; set; }
        public string BannedAirportsFile { get; set; }
        public List<string> Origins { get; set; }
        public List<string> Destinations { get; set; }
        public int MaxFlights { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int NoLongerThan { get; set; }
        public bool Headless { get; set; }
    }
}