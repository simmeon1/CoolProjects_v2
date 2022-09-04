using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.Workers;

namespace JourneyPlanner_ClassLibrary.Interfaces
{
    /// <summary>
    /// Don't forget constructor that uses <see cref="JourneyRetrieverComponents"/> as a parameter that the retriever will use.
    /// </summary>
    public interface IJourneyRetriever
    {
        void Initialise(JourneyRetrieverData data);
        Task<JourneyCollection> GetJourneysForDates(List<DirectPath> paths, List<DateTime> allDates);
    }
}