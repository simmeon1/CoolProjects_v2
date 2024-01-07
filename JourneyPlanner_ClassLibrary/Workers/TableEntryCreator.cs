using System;
using System.Collections.Generic;
using System.Linq;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class TableEntryCreator
    {
        private double AvgLength { get; set; }
        private double AvgLengthPenalized { get; set; }
        private double AvgCost { get; set; }
        private Dictionary<string, Airport> AirportDict { get; set; }

        public List<ITableEntry> GetTableEntries(
            List<Airport> airportList,
            List<SequentialJourneyCollection> sequentialCollections,
            Dictionary<string, int> penalties
        )
        {
            penalties ??= new Dictionary<string, int>();
            InitialiseData(airportList, sequentialCollections, penalties);
            return GetPopulatedTables(
                sequentialCollections.OrderByDescending(x => GetBargainPercentage(x)).ToList(),
                penalties
            );
        }

        private List<ITableEntry> GetPopulatedTables(
            List<SequentialJourneyCollection> list,
            Dictionary<string, int> penalties
        )
        {
            List<ITableEntry> result = new();
            for (int i = 0; i < list.Count; i++)
            {
                var collection = list[i];
                var collectionId = i + 1;
                result.Add(new SequentialJourneyCollectionItemEntry(
                    collection,
                    collectionId,
                    penalties,
                    AirportDict,
                    AvgLength,
                    AvgCost,
                    AvgLengthPenalized
                ));

                for (int j = 0; j < collection.Count(); j++)
                {
                    var journey = collection[j];
                    result.Add(new JourneyItemEntry(
                        collection,
                        journey,
                        collectionId,
                        j + 1,
                        AirportDict
                    ));
                }
            }

            return result;
        }
        
        private static TimeSpan GetPenalizedLength(
            SequentialJourneyCollection seqCollection,
            Dictionary<string, int> penalties
        )
        {
            var penaltyTime = GetPenaltyTime(seqCollection, penalties);
            return seqCollection.GetLength() + penaltyTime;
        }

        private static TimeSpan GetPenaltyTime(SequentialJourneyCollection seqCollection, Dictionary<string, int> penalties)
        {
            return GetLocationPenalty(penalties, GetDepartingLocation(seqCollection)) + GetLocationPenalty(penalties, GetArrivingLocation(seqCollection));
        }
        
        private static string GetDepartingLocation(SequentialJourneyCollection seqCollection)
        {
            return seqCollection[0].GetDepartingLocation();
        }
        
        private static string GetArrivingLocation(SequentialJourneyCollection seqCollection)
        {
            return seqCollection[seqCollection.Count() - 1].GetArrivingLocation();
        }

        private static TimeSpan GetLocationPenalty(Dictionary<string, int> penalties, string location)
        {
            return new TimeSpan(0, penalties.TryGetValue(location, out int startPenalty) ? startPenalty : 0, 0);
        }
        
        private void InitialiseData(
            List<Airport> airportList,
            List<SequentialJourneyCollection> sequentialCollections,
            Dictionary<string, int> penalties
        )
        {
            AirportDict = GetAirportDict(airportList);
            AvgLength = GetAverage(sequentialCollections, x => x.GetLength().TotalMinutes);
            AvgLengthPenalized = GetAverage(sequentialCollections, x => GetPenalizedLength(x, penalties).TotalMinutes);
            AvgCost = GetAverage(sequentialCollections, x => x.GetCost());
        }

        private static double GetAverage(
            IReadOnlyCollection<SequentialJourneyCollection> sequentialCollections,
            Func<SequentialJourneyCollection, double> selector
        )
        {
            return sequentialCollections.Count == 0 ? 0 : sequentialCollections.Average(selector);
        }

        private static Dictionary<string, Airport> GetAirportDict(List<Airport> airportList)
        {
            Dictionary<string, Airport> airportDict = new();
            foreach (Airport airport in airportList)
            {
                airportDict.TryAdd(airport.Code, airport);
            }

            return airportDict;
        }
        
        private double GetBargainPercentage(SequentialJourneyCollection seqCollection)
        {
            return Math.Round(
                (100 - seqCollection.GetLength().TotalMinutes / AvgLength * 100) +
                (100 - seqCollection.GetCost() / AvgCost * 100),
                2
            );
        }
        
        private double GetBargainPercentagePenalized(SequentialJourneyCollection seqCollection, Dictionary<string, int> penalties)
        {
            return Math.Round(
                (100 - GetPenalizedLength(seqCollection, penalties).TotalMinutes / AvgLengthPenalized * 100) +
                (100 - seqCollection.GetCost() / AvgCost * 100),
                2
            );
        }
    }
}