using System.Collections.Generic;
using System.Linq;

namespace JourneyPlanner_ClassLibrary.Classes
{
    public class JourneyRetrieverData
    {
        public List<DirectPath> DirectPaths { get; set; }
        public Dictionary<string, string> Translations { get; set; }
        public JourneyRetrieverData()
        {
        }
        
        public JourneyRetrieverData(List<DirectPath> directPaths, Dictionary<string, string> translations = null)
        {
            DirectPaths = directPaths;
            Translations = translations ?? new Dictionary<string, string>();
        }

        public string GetTranslation(string location)
        {
            return Translations.ContainsKey(location) ? Translations[location] : location;
        }

        public override string ToString()
        {
            return $"{DirectPaths.Count} direct paths, {Translations.Count} translations.";
        }

        public void RemovePath(string path)
        {
            DirectPaths = DirectPaths.Where(p => !p.ToString().Equals(path)).ToList();
        }
        
        public int GetCountOfDirectPaths()
        {
            return DirectPaths.Count;
        }

        public string GetKeyFromTranslation(string translation)
        {
            foreach (KeyValuePair<string, string> pair in Translations)
            {
                if (pair.Value.Equals(translation)) return pair.Key;
            }
            return translation;
        }
    }
}