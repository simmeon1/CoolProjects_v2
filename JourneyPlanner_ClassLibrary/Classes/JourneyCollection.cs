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
            for (int i = 0; i < journeys.GetCount(); i++) Add(journeys[i]);
        }

        public int GetCountOfFlights()
        {
            return Journeys.Where(x => x.Type == JourneyType.Flight).Count();
        }

        public int GetCountOfBuses()
        {
            return Journeys.Where(x => x.Type == JourneyType.Local).Count();
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
            return $"{Journeys.Count} flights";
        }

        internal List<List<JourneyCollection>> GetListOfCollectionsGroupedByDirectPaths(List<Path> paths)
        {
            List<List<JourneyCollection>> list = new();
            foreach (Path path in paths)
            {
                List<JourneyCollection> fullyConnectedPathJourneys = new();
                List<DirectPath> directPaths = path.GetDirectPaths();
                foreach (DirectPath directPath in directPaths)
                {
                    JourneyCollection col = new(Journeys.Where(j => j.Path.Equals(directPath.ToString())).ToList());
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
                if (fullyConnectedPathJourneys.Count > 0) list.Add(fullyConnectedPathJourneys);
            }
            return list;
        }
    }
}