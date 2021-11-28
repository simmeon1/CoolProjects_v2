using Common_ClassLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JourneyPlanner_ClassLibrary
{
    public class JourneyCollection
    {
        public List<Journey> Journeys { get; set; }
        public JourneyCollection()
        {
            Journeys = new();
        }

        public JourneyCollection(List<Journey> journeys)
        {
            Journeys = journeys;
        }

        public Journey this[int index]
        {
            get
            {
                return Journeys[index];
            }

            set
            {
                Journeys[index] = value;
            }
        }

        public int GetCount()
        {
            return Journeys.Count;
        }

        public void Add(Journey journey)
        {
            Journeys.Add(journey);
        }

        public void AddRange(JourneyCollection journeys)
        {
            int count = journeys.GetCount();
            for (int i = 0; i < count; i++) Add(journeys[i]);
        }

        public int GetCountOfFlights()
        {
            return Journeys.Where(x => x.IsFlight()).Count();
        }

        public int GetCountOfLocalLinks()
        {
            return Journeys.Where(x => !x.IsFlight()).Count();
        }

        public Journey GetFirst()
        {
            return Journeys[0];
        }

        public Journey GetLast()
        {
            return Journeys[GetCount() - 1];
        }

        public override string ToString()
        {
            return $"{Journeys.Count} journeys";
        }

        public JourneyCollection GetJourneysThatContainPath(string path)
        {
            return new(Journeys.Where(j => j.Path.Equals(path)).ToList());
        }

        internal void RemoveJourneysThatMatchPathAndRetriever(string retrieverName, string path)
        {
            List<Journey> newList = new();
            foreach (Journey journey in Journeys)
            {
                if (journey.RetrievedWithWorker.Equals(retrieverName) && journey.Path.ToString().Equals(path)) continue;
                newList.Add(journey);
            }
            Journeys = newList;
        }
    }
}