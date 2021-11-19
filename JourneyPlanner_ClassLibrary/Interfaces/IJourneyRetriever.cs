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
        void Initialise(JourneyRetrieverData data);
        Task<JourneyCollection> GetJourneysForDates(string origin, string destination, List<DateTime> allDates);
    }
}