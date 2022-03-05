using System.Collections.Generic;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class MultiJourneyCollectorResults
    {
        public Dictionary<string, Dictionary<string, bool>> Progress { get; set; }
        public JourneyCollection JourneyCollection { get; set; }

        public MultiJourneyCollectorResults()
        {
            JourneyCollection = new JourneyCollection();
            Progress = new Dictionary<string, Dictionary<string, bool>>();
        }

        public MultiJourneyCollectorResults(Dictionary<string, Dictionary<string, bool>> progress,
            JourneyCollection journeyCollection)
        {
            Progress = progress;
            JourneyCollection = journeyCollection;
        }
    }
}