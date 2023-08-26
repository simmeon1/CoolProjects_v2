using System.Collections.Generic;
using System.Linq;

namespace JourneyPlanner_ClassLibrary.Classes
{
    public class JourneyRetrieverData
    {
        public List<DirectPath> DirectPaths { get; private set; }
        public JourneyRetrieverData()
        {
        }
        
        public JourneyRetrieverData(List<DirectPath> directPaths)
        {
            DirectPaths = directPaths;
        }
        
        public override string ToString()
        {
            return $"{DirectPaths.Count} direct paths.";
        }

        public void RemovePath(string path)
        {
            DirectPaths = DirectPaths.Where(p => !p.ToString().Equals(path)).ToList();
        }
        
        public int GetCountOfDirectPaths()
        {
            return DirectPaths.Count;
        }
    }
}