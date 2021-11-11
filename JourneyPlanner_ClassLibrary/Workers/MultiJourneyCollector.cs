using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public class MultiJourneyCollector
    {
        private IJourneyRetrieverInstanceCreator InstanceCreator { get; set; }
        private JourneyRetrieverComponents Components { get; set; }

        public MultiJourneyCollector(JourneyRetrieverComponents components, IJourneyRetrieverInstanceCreator instanceCreator)
        {
            Components = components;
            InstanceCreator = instanceCreator;
        }

        public async Task<JourneyCollection> GetJourneys(Dictionary<string, JourneyRetrieverData> retrieversAndData, DateTime dateFrom, DateTime dateTo)
        {
            JourneyCollection journeys = new();
            foreach (KeyValuePair<string, JourneyRetrieverData> data in retrieversAndData)
            {
                string retrieverName = data.Key;
                JourneyRetrieverData retrieverData = data.Value;
                string fullClassName = $"{Assembly.GetExecutingAssembly().GetName().Name}.{retrieverName}";
                IJourneyRetriever retriever = InstanceCreator.CreateInstance(fullClassName, Components);
                journeys.AddRange(await retriever.CollectJourneys(retrieverData, dateFrom, dateTo));
            }
            return journeys;
        }
    }
}