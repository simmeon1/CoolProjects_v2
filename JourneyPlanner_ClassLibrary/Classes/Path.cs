using System.Collections.Generic;
using Common_ClassLibrary;

namespace JourneyPlanner_ClassLibrary.Classes
{
    public class Path
    {
        private List<string> Entries { get; set; }        
        public Path()
        {
        }
        
        public Path(List<string> path)
        {
            Entries = path;
        }

        public string this[int index]
        {
            get => Entries[index];

            set => Entries[index] = value;
        }

        public override string ToString()
        {
            return Entries.ConcatenateListOfStringsToDashString();
        }
        
        public int Count()
        {
            return Entries.Count;
        }
        
        public string GetSummarisedPath()
        {
            return new List<string>() { Entries[0], Entries[Count() - 1] }.ConcatenateListOfStringsToDashString();
        }
        
        public List<DirectPath> GetDirectPaths()
        {
            if (Entries.Count < 2) return new List<DirectPath>();

            List<DirectPath> directPaths = new();
            string previousEntry = Entries[0];
            for (int i = 1; i < Entries.Count; i++)
            {
                directPaths.Add(new DirectPath(previousEntry, Entries[i]));
                previousEntry = Entries[i];
            }
            return directPaths;
        }
    }
}