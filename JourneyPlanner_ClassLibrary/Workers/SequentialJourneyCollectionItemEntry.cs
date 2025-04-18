﻿using System;
using System.Collections.Generic;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class SequentialJourneyCollectionItemEntry : ITableEntry
    {
        private readonly SequentialJourneyCollection seqCollection;
        private readonly int id;
        private readonly Dictionary<string, int> timePenalties;
        private readonly Dictionary<string, int> costPenalties;
        private readonly Dictionary<string, Airport> airportDict;
        private readonly double avgLength;
        private readonly double avgCost;
        private readonly double avgLengthPenalized;
        private readonly double avgCostPenalized;

        public SequentialJourneyCollectionItemEntry(
            SequentialJourneyCollection seqCollection,
            int id,
            Dictionary<string, int> timePenalties,
            Dictionary<string, int> costPenalties,
            Dictionary<string, Airport> airportDict,
            double avgLength,
            double avgCost,
            double avgLengthPenalized,
            double avgCostPenalized
        )
        {
            this.seqCollection = seqCollection;
            this.id = id;
            this.timePenalties = timePenalties;
            this.costPenalties = costPenalties;
            this.airportDict = airportDict;
            this.avgLength = avgLength;
            this.avgCost = avgCost;
            this.avgLengthPenalized = avgLengthPenalized;
            this.avgCostPenalized = avgCostPenalized;
        }

        public string GetIdentifier()
        {
            return seqCollection.GetFullPath();
        }

        public string GetCategory()
        {
            return "Summary";
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            return new List<KeyValuePair<string, object>>
            {
                //Core info
                new("Path", seqCollection.GetFullPath()),
                new("Id", id),
                new("Flights", seqCollection.GetCountOfFlights()),
                new("Shortest Pause", GetShortTimeSpan(seqCollection.GetShortestPause())),
                new("Start Time", seqCollection.GetStartTime()),
                new("End Time", seqCollection.GetEndTime()),
                new("Length", GetShortTimeSpan(seqCollection.GetLength())),
                new("Cost £", seqCollection.GetCost()),
                new("Bargain %", GetBargainPercentage(seqCollection)),

                //Extra info
                new("Start Location", GetDepartingLocation(seqCollection)),
                new("End Location", GetArrivingLocation(seqCollection)),
                new("Has 0 Cost Journey", seqCollection.HasJourneyWithZeroCost()),
                new("Same Day Finish", seqCollection.StartsAndEndsOnSameDay()),
                new("Airline Count", seqCollection.GetCountOfAirlines()),
                new("Country Changes", GetCountryChanges(seqCollection)),
                new("Companies", seqCollection.GetCompaniesString()),

                //Penalties
                new("Start Penalized", GetPenalizedStartTime(seqCollection, timePenalties)),
                new("End Penalized", GetPenalizedEndTimeTime(seqCollection, timePenalties)),
                new("Length Penalized", GetShortTimeSpan(GetPenalizedLength(seqCollection, timePenalties))),
                new("Cost Penalized", GetPenalizedCost(seqCollection, costPenalties)),
                new("Penalty Time", GetPenaltyTime(seqCollection, timePenalties).TotalMinutes),
                new("Penalty Cost", GetPenaltyCost(seqCollection, costPenalties)),
                new("Bargain % Penalized", GetBargainPercentagePenalized(seqCollection, timePenalties, costPenalties)),

                //Ints
                new("Shortest Pause Int", seqCollection.GetShortestPause().TotalMinutes),
                new("Length Int", seqCollection.GetLength().TotalMinutes),
                new("Length Int Penalized", GetPenalizedLength(seqCollection, timePenalties).TotalMinutes),
            };
        }

        // private static string GetShortDateTime(DateTime? dt)
        // {
        //     return dt.Value.ToString("dd/MM/yyyy HH:mm:ss");
        // }

        private static string GetShortTimeSpan(TimeSpan? ts)
        {
            return ts.Value.ToString(@"dd\:hh\:mm");
        }

        private static TimeSpan GetPenalizedLength(
            SequentialJourneyCollection seqCollection,
            Dictionary<string, int> timePenalties
        )
        {
            var penaltyTime = GetPenaltyTime(seqCollection, timePenalties);
            return seqCollection.GetLength() + penaltyTime;
        }
        
        private static double GetPenalizedCost(
            SequentialJourneyCollection seqCollection,
            Dictionary<string, int> costPenalties
        )
        {
            var penaltyCost = GetPenaltyCost(seqCollection, costPenalties);
            return seqCollection.GetCost() + penaltyCost;
        }

        private static TimeSpan GetPenaltyTime(SequentialJourneyCollection seqCollection, Dictionary<string, int> penalties)
        {
            return GetLocationTimePenalty(penalties, GetDepartingLocation(seqCollection)) + GetLocationTimePenalty(penalties, GetArrivingLocation(seqCollection));
        }
        
        private static int GetPenaltyCost(SequentialJourneyCollection seqCollection, Dictionary<string, int> penalties)
        {
            return GetLocationCostPenalty(penalties, GetDepartingLocation(seqCollection)) + GetLocationCostPenalty(penalties, GetArrivingLocation(seqCollection));
        }
        
        private static TimeSpan GetLocationTimePenalty(Dictionary<string, int> penalties, string location)
        {
            return new TimeSpan(0, penalties.TryGetValue(location, out int startPenalty) ? startPenalty : 0, 0);
        }
        
        private static int GetLocationCostPenalty(Dictionary<string, int> penalties, string location)
        {
            return penalties.TryGetValue(location, out int startPenalty) ? startPenalty : 0;
        }

        private static DateTime? GetPenalizedStartTime(
            SequentialJourneyCollection seqCollection,
            Dictionary<string, int> penalties
        )
        {
            var location = GetDepartingLocation(seqCollection);
            TimeSpan penalty = GetLocationTimePenalty(penalties, location);
            return seqCollection.GetStartTime() - penalty;
        }

        private static string GetDepartingLocation(SequentialJourneyCollection seqCollection)
        {
            return seqCollection[0].GetDepartingLocation();
        }

        private DateTime? GetPenalizedEndTimeTime(
            SequentialJourneyCollection seqCollection,
            Dictionary<string, int> penalties
        )
        {
            var location = GetArrivingLocation(seqCollection);
            TimeSpan penalty = GetLocationTimePenalty(penalties, location);
            return seqCollection.GetEndTime() + penalty;
        }

        private static string GetArrivingLocation(SequentialJourneyCollection seqCollection)
        {
            return seqCollection[seqCollection.Count() - 1].GetArrivingLocation();
        }
        
        private double GetCountryChanges(SequentialJourneyCollection c)
        {
            int changes = 0;
            Journey previousJourney = c[0];
            if (!airportDict[previousJourney.GetDepartingLocation()].Country
                    .Equals(airportDict[previousJourney.GetArrivingLocation()].Country))
            {
                changes++;
            }

            for (int i = 1; i < c.Count(); i++)
            {
                Journey currentJourney = c[i];
                if (!airportDict[currentJourney.GetArrivingLocation()].Country
                        .Equals(airportDict[previousJourney.GetArrivingLocation()].Country))
                {
                    changes++;
                }

                previousJourney = currentJourney;
            }

            return changes;
        }

        private double GetBargainPercentage(SequentialJourneyCollection seqCollection)
        {
            return Math.Round(
                (100 - seqCollection.GetLength().TotalMinutes / avgLength * 100) +
                (100 - seqCollection.GetCost() / avgCost * 100),
                2
            );
        }

        private double GetBargainPercentagePenalized(
            SequentialJourneyCollection seqCollection,
            Dictionary<string, int> timePenalties,
            Dictionary<string, int> costPenalties
        )
        {
            return Math.Round(
                (100 - GetPenalizedLength(seqCollection, timePenalties).TotalMinutes / avgLengthPenalized * 100) +
                (100 - GetPenalizedCost(seqCollection, costPenalties) / avgCostPenalized * 100),
                2
            );
        }
    }
}