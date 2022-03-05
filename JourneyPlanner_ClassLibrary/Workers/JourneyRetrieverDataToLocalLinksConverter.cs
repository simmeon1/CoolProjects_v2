using System.Collections.Generic;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class JourneyRetrieverDataToLocalLinksConverter
    {
        public Dictionary<string, HashSet<string>> DoConversion(Dictionary<string, JourneyRetrieverData> data)
        {
            Dictionary<string, HashSet<string>> result = new();
            foreach (KeyValuePair<string, JourneyRetrieverData> pair in data)
            {
                foreach (DirectPath path in pair.Value.DirectPaths)
                {
                    string start = path.GetStart();
                    string end = path.GetEnd();
                    if (result.ContainsKey(start)) result[start].Add(end);
                    else result.Add(start, new HashSet<string>() { end });
                }
            }
            return result;
        }
    }
}