using System.Collections.Generic;
using System.Linq;

namespace JourneyPlanner_ClassLibrary.Classes
{
    public class JourneyCollection
    {
        public List<Journey> Journeys { get; set; }
        public JourneyCollection()
        {
            Journeys = new List<Journey>();
        }

        public JourneyCollection(List<Journey> journeys)
        {
            Journeys = journeys;
        }

        public Journey this[int index]
        {
            get => Journeys[index];

            set => Journeys[index] = value;
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

        public int GetCountOfJourneys()
        {
            return Journeys.Count;
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
            return new JourneyCollection(Journeys.Where(j => j.Path.Equals(path)).ToList());
        }
        
        public bool AlreadyContainsJourney(Journey journey)
        {
            return Journeys.Any(s => s.ToString().Equals(journey.ToString()));
        }
    }
}