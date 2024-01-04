using System;
using System.Collections.Generic;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class JourneyItemEntry : ITableEntry
    {
        private readonly SequentialJourneyCollection seqCollection;
        private readonly Journey journey;
        private readonly int collectionId;
        private readonly int journeyId;
        private readonly Dictionary<string, Airport> airportDict;

        public JourneyItemEntry(SequentialJourneyCollection seqCollection, Journey journey, int collectionId, int journeyId, Dictionary<string, Airport> airportDict)
        {
            this.seqCollection = seqCollection;
            this.journey = journey;
            this.collectionId = collectionId;
            this.journeyId = journeyId;
            this.airportDict = airportDict;
        }

        public string GetIdentifier()
        {
            return journey.Path;
        }

        public string GetCategory()
        {
            return "Details";
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            return new List<KeyValuePair<string, object>>
            {
                new("Path", journey.Path),
                new("Id", collectionId),
                new("Journey #", journeyId),
                new("Departing Time", journey.Departing),
                new("Arriving Time", journey.Arriving),
                new("Wait Time From Prev", GetShortTimeSpan(
                    journeyId == 1 ? new TimeSpan() : journey.Departing - seqCollection[journeyId - 2].Arriving
                )),
                new("Length", GetShortTimeSpan(journey.Duration)),
                new("Cost £", journey.Cost),
                new("Departing Airport", airportDict[journey.GetDepartingLocation()].Name),
                new("Arriving Airport", airportDict[journey.GetArrivingLocation()].Name),
                new("Departing Country", airportDict[journey.GetDepartingLocation()].Country),
                new("Arriving Country", airportDict[journey.GetArrivingLocation()].Country),
                new("Company", journey.Company),
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
    }
}