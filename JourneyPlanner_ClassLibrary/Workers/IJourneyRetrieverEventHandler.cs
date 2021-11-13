using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public interface IMultiJourneyCollector
    {
        void InformOfPathDataFullyCollected(string path);
        Task<MultiJourneyCollectorResults> GetJourneys(JourneyRetrieverComponents components, Dictionary<string, JourneyRetrieverData> results, DateTime dateFrom, DateTime dateTo, MultiJourneyCollectorResults existingResults = null);
    }
}