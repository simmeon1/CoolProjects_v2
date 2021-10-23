using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface IChromeWorker
    {
        Task<ChromeWorkerResults> ProcessPaths(List<Airport> airportList, List<Path> paths, DateTime dateFrom, DateTime dateTo, int defaultDelay, Dictionary<string, FlightCollection> collectedPathFlights);
    }
}