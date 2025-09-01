using System;
using System.Collections.Generic;

namespace JourneyPlanner_ClassLibrary.Classes
{
    public record Parameters(
        string FileSavePath,
        string AirportListFile,
        string AirportDestinationsFile,
        string ExistingResultsPath,
        string TimePenaltiesFile,
        string CostPenaltiesFile,
        string BannedAirportsFile,
        List<string> Origins,
        List<string> Destinations,
        int MaxFlights,
        DateTime DateFrom,
        DateTime DateTo,
        int NoLongerThan,
        bool Headless,
        bool RetryLast
    );
}