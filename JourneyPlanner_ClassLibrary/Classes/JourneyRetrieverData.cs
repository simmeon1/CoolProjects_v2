using System;
using System.Collections.Generic;
using System.Linq;

namespace JourneyPlanner_ClassLibrary
{
    public class JourneyRetrieverData
    {
        public List<DirectPath> DirectPaths { get; set; }
        public Dictionary<string, string> Translations { get; set; }
        public JourneyRetrieverData(List<DirectPath> directPaths, Dictionary<string, string> translations = null)
        {
            DirectPaths = directPaths;
            Translations = translations ?? new();
        }

        public string GetTranslation(string location)
        {
            return Translations.ContainsKey(location) ? Translations[location] : location;
        }

        public override string ToString()
        {
            return $"{DirectPaths.Count} direct paths, {Translations.Count} translations.";
        }
    }
}