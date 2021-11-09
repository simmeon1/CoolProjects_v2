using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class SequentialJourneyCollectionBuilder
    {
        private FullPathAndListOfPathsAndJourneyCollections DataWithJourneysForSinglePaths { get; set; }

        public List<SequentialJourneyCollection> GetFullPathCombinationOfJourneys(FullPathAndListOfPathsAndJourneyCollections dataWithJourneysForSinglePaths)
        {
            DataWithJourneysForSinglePaths = dataWithJourneysForSinglePaths;
            string fullPathName = DataWithJourneysForSinglePaths.Path.ToString();
            List<PathAndJourneyCollection> pathsAndJourneys = DataWithJourneysForSinglePaths.PathsAndJourneyCollections;
            List<SequentialJourneyCollection> fullPathCombinationsOfJourneys = new();
            LinkedList<Journey> journey = new();
            BuildUpCombinationOfJourneys(0, journey, fullPathCombinationsOfJourneys);
            return fullPathCombinationsOfJourneys;
        }

        private void BuildUpCombinationOfJourneys(int index, LinkedList<Journey> listOfJourneys, List<SequentialJourneyCollection> combos)
        {
            int count = DataWithJourneysForSinglePaths.PathsAndJourneyCollections.Count;
            for (int i = index; i < count; i++)
            {
                PathAndJourneyCollection pathAndJourneys = DataWithJourneysForSinglePaths.PathsAndJourneyCollections[i];
                JourneyCollection pathJourneys = pathAndJourneys.JourneyCollection;
                for (int j = 0; j < pathJourneys.GetCount(); j++)
                {
                    listOfJourneys.AddLast(pathJourneys[j]);
                    if (index < count - 1) BuildUpCombinationOfJourneys(i + 1, listOfJourneys, combos);
                    else if (listOfJourneys.Count == count) combos.Add(new SequentialJourneyCollection(new(listOfJourneys.ToList())));
                    listOfJourneys.RemoveLast();
                }
            }
        }
    }
}