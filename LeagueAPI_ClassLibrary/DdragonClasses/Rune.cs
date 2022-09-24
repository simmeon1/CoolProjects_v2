using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LeagueAPI_ClassLibrary
{
    public class Rune : ITableEntry
    {
        public string Name { get; set; } = "";
        public string Tree { get; set; } = "";
        public string LongDescription { get; set; } = "";
        public int Slot { get; set; }
        public string GetCleanDescription()
        {
            return Regex.Replace(LongDescription, "<.*?>", "");
        }

        public string GetCategory()
        {
            return "Runes";
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            return new List<KeyValuePair<string, object>>
            {
                new("Name", Name),
                new("Tree", Tree),
                new("Slot", Slot),
                new("Description", GetCleanDescription())
            };
        }
    }
}