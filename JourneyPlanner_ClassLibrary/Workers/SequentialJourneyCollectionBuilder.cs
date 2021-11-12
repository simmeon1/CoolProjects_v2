using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JourneyPlanner_ClassLibrary
{
    public class SequentialJourneyCollectionBuilder
    {
        private List<JourneyCollection> JourneyCollections { get; set; }

        public List<SequentialJourneyCollection> GetFullPathCombinationOfJourneys(List<Path> fullPaths, JourneyCollection journeyCollection)
        {
            Dictionary<string, JourneyCollection> collectionsByDirectPathDict = GetDictOfCollectionsGroupedByDirectPath(fullPaths, journeyCollection);
            List<List<JourneyCollection>> collectionsByDirectPathLists = GetListsOfCollectionsGroupedByDirectPath(fullPaths, collectionsByDirectPathDict);
            return GetCombinationsOfSequentialJourneys(collectionsByDirectPathLists);
        }

        private static Dictionary<string, JourneyCollection> GetDictOfCollectionsGroupedByDirectPath(List<Path> fullPaths, JourneyCollection journeyCollection)
        {
            Dictionary<string, JourneyCollection> collectionsByDirectPath = new();
            foreach (Path path in fullPaths)
            {
                List<DirectPath> directPaths = path.GetDirectPaths();
                foreach (DirectPath directPath in directPaths)
                {
                    string directPathStr = directPath.ToString();
                    if (!collectionsByDirectPath.ContainsKey(directPathStr))
                    {
                        collectionsByDirectPath.Add(directPathStr, journeyCollection.GetJourneysThatContainPath(directPathStr));
                    }
                }
            }
            return collectionsByDirectPath;
        }

        private static List<List<JourneyCollection>> GetListsOfCollectionsGroupedByDirectPath(List<Path> fullPaths, Dictionary<string, JourneyCollection> collectionsByDirectPath)
        {
            List<List<JourneyCollection>> journeyCollectionsGrouped = new();
            foreach (Path path in fullPaths)
            {
                List<JourneyCollection> fullyConnectedPathJourneys = new();
                List<DirectPath> directPaths = path.GetDirectPaths();
                foreach (DirectPath directPath in directPaths)
                {
                    JourneyCollection col = collectionsByDirectPath[directPath.ToString()];
                    if (col.GetCount() == 0)
                    {
                        fullyConnectedPathJourneys.Clear();
                        break;
                    }
                    else
                    {
                        fullyConnectedPathJourneys.Add(col);
                    }
                }
                if (fullyConnectedPathJourneys.Count > 0) journeyCollectionsGrouped.Add(fullyConnectedPathJourneys);
            }

            return journeyCollectionsGrouped;
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

        private List<SequentialJourneyCollection> GetCombinationsOfSequentialJourneys(List<List<JourneyCollection>> journeyCollectionsGrouped)
        {
            List<SequentialJourneyCollection> fullPathCombinationsOfJourneys = new();
            foreach (List<JourneyCollection> journeyCollections in journeyCollectionsGrouped)
            {
                JourneyCollections = journeyCollections;
                LinkedList<Journey> journey = new();
                BuildUpCombinationOfJourneys(0, journey, fullPathCombinationsOfJourneys);
            }
            return fullPathCombinationsOfJourneys;
        }
    }
}