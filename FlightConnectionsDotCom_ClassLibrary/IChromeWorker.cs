using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface IChromeWorker
    {
        Task<List<FullPathAndListOfPathsAndFlightCollections>> ProcessPaths(List<Path> paths, DateTime dateFrom, DateTime dateTo, int defaultDelay = 500);
    }
}