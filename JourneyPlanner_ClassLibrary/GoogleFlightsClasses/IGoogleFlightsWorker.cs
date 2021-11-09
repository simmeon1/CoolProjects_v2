using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public interface IGoogleFlightsWorker
    {
        Task<GoogleFlightsWorkerResults> ProcessPaths(List<Path> paths, DateTime dateFrom, DateTime dateTo, int defaultDelay, Dictionary<string, JourneyCollection> collectedPathFlights);
    }
}