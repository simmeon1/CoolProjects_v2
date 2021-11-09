using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface IGoogleFlightsWorker
    {
        Task<GoogleFlightsWorkerResults> ProcessPaths(List<Path> paths, DateTime dateFrom, DateTime dateTo, int defaultDelay, Dictionary<string, JourneyCollection> collectedPathFlights);
    }
}