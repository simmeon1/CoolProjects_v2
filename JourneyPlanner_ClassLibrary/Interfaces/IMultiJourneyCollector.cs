using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Workers;

namespace JourneyPlanner_ClassLibrary.Interfaces
{
    public interface IMultiJourneyCollector
    {
        Task<MultiJourneyCollectorResults> GetJourneys(JourneyRetrieverComponents components, Dictionary<string, JourneyRetrieverData> results, DateTime dateFrom, DateTime dateTo, MultiJourneyCollectorResults existingResults = null);
    }
}