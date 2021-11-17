using Common_ClassLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace JourneyPlanner_ClassLibrary
{
    public class SequentialJourneyCollection
    {
        public JourneyCollection JourneyCollection { get; set; }

        public SequentialJourneyCollection()
        {
            JourneyCollection = new();
        }

        public SequentialJourneyCollection(JourneyCollection journeyCollection)
        {
            if (journeyCollection != null && journeyCollection.GetCount() > 1) DoSequenceCheck(journeyCollection);
            JourneyCollection = journeyCollection;
        }

        private static void DoSequenceCheck(JourneyCollection journeyCollection)
        {
            Journey previousJourney = journeyCollection[0];
            for (int i = 1; i < journeyCollection.GetCount(); i++)
            {
                if (!previousJourney.GetArrivingAirport().Equals(journeyCollection[i].GetDepartingAirport()))
                {
                    throw new Exception("Journeys are not connected.");
                }
                previousJourney = journeyCollection[i];
            }
        }

        public Journey this[int index]
        {
            get
            {
                return JourneyCollection[index];
            }

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

        private bool IsNotValid()
        {
            return JourneyCollection == null || JourneyCollection.GetCount() == 0;
        }
        
        public int Count()
        {
            return IsNotValid() ? 0 : JourneyCollection.GetCount();
        }

        public override string ToString()
        {
            return IsNotValid() ? "No journeys in sequence." : $"{GetFullPath()}, Doable = {SequenceIsDoable()}, Start = {GetStartTime()}, End = {GetEndTime()}, Cost = {GetCost()}";
        }

        public string GetFullPath()
        {
            StringBuilder path = new("");
            path.Append(JourneyCollection[0].Path);
            for (int i = 1; i < JourneyCollection.GetCount(); i++)
            {
                Journey journey = JourneyCollection[i];
                path.Append($"-{journey.GetArrivingAirport()}");
            }
            return path.ToString();
        }

        public bool SequenceIsDoable()
        {
            if (IsNotValid()) return false;
            if (JourneyCollection.GetCount() == 1) return true;
            Journey previousJourney = JourneyCollection[0];
            for (int i = 1; i < JourneyCollection.GetCount(); i++)
            {
                if ((JourneyCollection[i].Departing - previousJourney.Arriving).TotalMinutes < (JourneyCollection[i].IsFlight() ? 120 : 30)) return false;
                previousJourney = JourneyCollection[i];
            }
            return true;
        }

        public double GetCost()
        {
            double cost = 0;
            if (IsNotValid()) return cost;
            for (int i = 0; i < JourneyCollection.GetCount(); i++) cost += JourneyCollection[i].Cost;
            return cost;
        }

        public DateTime? GetStartTime()
        {
            return IsNotValid() ? null : JourneyCollection.GetFirst().Departing;
        }

        public DateTime? GetEndTime()
        {
            return IsNotValid() ? null : JourneyCollection.GetLast().Arriving;
        }

        public int GetCountOfFlights()
        {
            return IsNotValid() ? 0 : JourneyCollection.GetCountOfFlights();
        }
        
        public int GetCountOfBuses()
        {
            return IsNotValid() ? 0 : JourneyCollection.GetCountOfLocalLinks();
        }

        public TimeSpan GetLength()
        {
            if (IsNotValid()) return new();
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
            return !IsNotValid() && GetStartTime().Value.Day == GetEndTime().Value.Day;
        }
        
        public bool HasJourneyWithZeroCost()
        {
            if (IsNotValid()) return false;
            for (int i = 0; i < JourneyCollection.GetCount(); i++) if (JourneyCollection[i].Cost == 0) return true;
            return false;
        }
    }
}