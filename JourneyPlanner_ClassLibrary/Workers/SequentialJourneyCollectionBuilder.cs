using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JourneyPlanner_ClassLibrary
{
    public class SequentialJourneyCollectionBuilder
    {
        public bool SkipUndoableJourneys { get; set; }
        public bool SkipNotSameDayFinishJourneys { get; set; }
        public int NoLongerThan { get; set; }
        public List<SequentialJourneyCollection> GetFullPathCombinationOfJourneys(List<Path> fullPaths, JourneyCollection journeyCollection, bool skipUndoableJourneys, bool skipNotSameDayFinishJourneys, int noLongerThan)
        {
            SkipUndoableJourneys = skipUndoableJourneys;
            SkipNotSameDayFinishJourneys = skipNotSameDayFinishJourneys;
            NoLongerThan = noLongerThan;

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

        private List<SequentialJourneyCollection> GetCombinationsOfSequentialJourneys(List<List<JourneyCollection>> journeyCollectionsGrouped)
        {
            Stack<Journey> currentStack = new();
            Stack<SequentialJourneyCollection> results = new();

            for (int i = 0; i < journeyCollectionsGrouped.Count; i++)
            {
                List<JourneyCollection> journeyCollections = journeyCollectionsGrouped[i];
                BuildUpCombinationOfJourneys(0, journeyCollections, currentStack, results);
            }
            return results.ToList();
        }

        private void BuildUpCombinationOfJourneys(int index, List<JourneyCollection> journeyCollections, Stack<Journey> currentStack, Stack<SequentialJourneyCollection> results)
        {
            JourneyCollection col = journeyCollections[index];
            for (int i = 0; i < col.GetCount(); i++)
            {
                currentStack.Push(col[i]);
                SequentialJourneyCollection seq = new(new(currentStack.Reverse().ToList()));
                if (
                    (!SkipUndoableJourneys || seq.SequenceIsDoable()) &&
                    (!SkipNotSameDayFinishJourneys || seq.StartsAndEndsOnSameDay()) &&
                    (seq.GetLength().TotalHours <= NoLongerThan)
                )
                {
                    if (seq.Count() == journeyCollections.Count) results.Push(seq);
                    else BuildUpCombinationOfJourneys(index + 1, journeyCollections, currentStack, results);
                }
                currentStack.Pop();
            }
        }
    }
}