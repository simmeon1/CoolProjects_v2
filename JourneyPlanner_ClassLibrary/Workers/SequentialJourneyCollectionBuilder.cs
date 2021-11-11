using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JourneyPlanner_ClassLibrary
{
    public class SequentialJourneyCollectionBuilder
    {
        private List<JourneyCollection> JourneyCollections { get; set; }

        public List<SequentialJourneyCollection> GetFullPathCombinationOfJourneys(List<JourneyCollection> journeyCollections)
        {
            JourneyCollections = journeyCollections;
            List<SequentialJourneyCollection> fullPathCombinationsOfJourneys = new();
            LinkedList<Journey> journey = new();
            BuildUpCombinationOfJourneys(0, journey, fullPathCombinationsOfJourneys);
            return fullPathCombinationsOfJourneys;
        }

        private void BuildUpCombinationOfJourneys(int index, LinkedList<Journey> listOfJourneys, List<SequentialJourneyCollection> combos)
        {
            int count = JourneyCollections.Count;
            for (int i = index; i < count; i++)
            {
                JourneyCollection journeys = JourneyCollections[i];
                for (int j = 0; j < journeys.GetCount(); j++)
                {
                    listOfJourneys.AddLast(journeys[j]);
                    if (index < count - 1) BuildUpCombinationOfJourneys(i + 1, listOfJourneys, combos);
                    else if (listOfJourneys.Count == count) combos.Add(new SequentialJourneyCollection(new(listOfJourneys.ToList())));
                    listOfJourneys.RemoveLast();
                }
            }
        }
    }
}