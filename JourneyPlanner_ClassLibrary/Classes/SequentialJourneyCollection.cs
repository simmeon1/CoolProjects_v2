﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common_ClassLibrary;

namespace JourneyPlanner_ClassLibrary.Classes
{
    public class SequentialJourneyCollection
    {
        private JourneyCollection JourneyCollection { get; }
        public SequentialJourneyCollection(JourneyCollection journeyCollection)
        {
            if (journeyCollection == null || journeyCollection.GetCount() == 0) throw new Exception("No journeys in collection.");
            DoSequenceCheck(journeyCollection);
            JourneyCollection = journeyCollection;
        }

        private static void DoSequenceCheck(JourneyCollection journeyCollection)
        {
            Journey previousJourney = journeyCollection[0];
            for (int i = 1; i < journeyCollection.GetCount(); i++)
            {
                if (!previousJourney.GetArrivingLocation().Equals(journeyCollection[i].GetDepartingLocation()))
                {
                    throw new Exception("Journeys are not connected.");
                }
                previousJourney = journeyCollection[i];
            }
        }

        public Journey this[int index]
        {
            get => JourneyCollection[index];

            set
            {
                Journey originalJourney = JourneyCollection[index];
                JourneyCollection[index] = value;
                try
                {
                    DoSequenceCheck(JourneyCollection);
                }
                catch (Exception)
                {
                    JourneyCollection[index] = originalJourney;
                    throw;
                }
            }
        }

        public int Count()
        {
            return JourneyCollection.GetCount();
        }

        public override string ToString()
        {
            return $"{GetFullPath()}, Doable = {SequenceIsDoable()}, Start = {GetStartTime()}, End = {GetEndTime()}, Cost = {GetCost()}";
        }

        public string GetFullPath()
        {
            StringBuilder path = new("");
            path.Append(JourneyCollection[0].Path);
            for (int i = 1; i < JourneyCollection.GetCount(); i++)
            {
                Journey journey = JourneyCollection[i];
                path.Append($"-{journey.GetArrivingLocation()}");
            }
            return path.ToString();
        }

        public bool SequenceIsDoable()
        {
            if (JourneyCollection.GetCount() == 1) return true;
            Journey previousJourney = JourneyCollection[0];
            for (int i = 1; i < JourneyCollection.GetCount(); i++)
            {
                if ((JourneyCollection[i].Departing - previousJourney.Arriving).TotalMinutes < 30) return false;
                previousJourney = JourneyCollection[i];
            }
            return true;
        }
        
        public TimeSpan GetShortestPause()
        {
            if (JourneyCollection.GetCount() == 1) return new TimeSpan();

            var pauses = new List<TimeSpan>();
            Journey previousJourney = JourneyCollection[0];
            for (int i = 1; i < JourneyCollection.GetCount(); i++)
            {
                pauses.Add(JourneyCollection[i].Departing - previousJourney.Arriving);
                previousJourney = JourneyCollection[i];
            }
            return pauses.Min();
        }


        public double GetCost()
        {
            double cost = 0;
            for (int i = 0; i < JourneyCollection.GetCount(); i++) cost += JourneyCollection[i].Cost;
            return cost;
        }

        public DateTime? GetStartTime()
        {
            return JourneyCollection.GetFirst().Departing;
        }

        public DateTime? GetEndTime()
        {
            return JourneyCollection.GetLast().Arriving;
        }

        public int GetCountOfFlights()
        {
            return JourneyCollection.GetCountOfJourneys();
        }
        
        public TimeSpan GetLength()
        {
            TimeSpan time = new();
            for (int i = 0; i < JourneyCollection.GetCount(); i++)
            {
                Journey journey = JourneyCollection[i];
                TimeSpan waitFromPrev = i == 0 ? new TimeSpan() : (journey.Departing - JourneyCollection[i - 1].Arriving);
                time += waitFromPrev + journey.Duration;
            }
            return time;
        }
        
        public bool StartsAndEndsOnSameDay()
        {
            return GetStartTime().Value.Day == GetEndTime().Value.Day;
        }
        
        public bool HasJourneyWithZeroCost()
        {
            for (int i = 0; i < JourneyCollection.GetCount(); i++) if (JourneyCollection[i].Cost == 0) return true;
            return false;
        }

        public string GetDepartingLocation()
        {
            return JourneyCollection[0].GetDepartingLocation();
        }

        public int GetCountOfCompanies()
        {
            return GetCompanies().Count;
        }
        
        public int GetCountOfAirlines()
        {
            return GetCompanies().Count;
        }

        private HashSet<string> GetCompanies()
        {
            HashSet<string> companies = new();
            for (int i = 0; i < JourneyCollection.GetCount(); i++)
            {
                Journey journey = JourneyCollection[i];
                companies.Add(journey.Company);
            }
            return companies;
        }

        public string GetCompaniesString()
        {
            return GetCompanies().ToList().ConcatenateListOfStringsToCommaAndSpaceString();
        }
    }
}