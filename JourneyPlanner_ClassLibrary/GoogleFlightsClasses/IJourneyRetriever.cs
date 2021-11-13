using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    /// <summary>
    /// Don't forget constructor that uses <see cref="JourneyRetrieverComponents"/> as a parameter that the retriever will use.
    /// </summary>
    public interface IJourneyRetriever
    {
        Task<JourneyCollection> CollectJourneys(JourneyRetrieverData paths, DateTime dateFrom, DateTime dateTo, JourneyCollection collectedJourneys);
    }
}