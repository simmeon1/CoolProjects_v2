using System.Collections.Generic;

namespace JourneyPlanner_ClassLibrary
{
    public class MultiJourneyCollectorResults
    {
        public Dictionary<string, Dictionary<string, bool>> Progress { get; set; }
        public JourneyCollection JourneyCollection { get; set; }
        public MultiJourneyCollectorResults()
        {
            JourneyCollection = new();
            Progress = new();
        }
        
        public MultiJourneyCollectorResults(Dictionary<string, Dictionary<string, bool>> progress, JourneyCollection journeyCollection)
        {
            Progress = progress;
            JourneyCollection = journeyCollection;
        }
    }
}