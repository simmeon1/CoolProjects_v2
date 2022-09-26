using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LeagueAPI_ClassLibrary
{
    public class Rune : ITableEntry
    {
        private readonly string name;
        private readonly string tree;
        private readonly string longDescription;
        private readonly int slot;
        public Rune(string name, string tree, string longDescription, int slot)
        {
            this.name = name;
            this.tree = tree;
            this.longDescription = longDescription;
            this.slot = slot;
        }

        public string GetTree()
        {
            return tree;
        }
        
        public int GetSlot()
        {
            return slot;
        }

        public string GetIdentifier()
        {
            return name;
        }

        public string GetCategory()
        {
            return "Runes";
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            return new List<KeyValuePair<string, object>>
            {
                new("Name", name),
                new("Tree", tree),
                new("Slot", slot),
                new("Description", GetCleanDescription())
            };
        }
        
        private string GetCleanDescription()
        {
            return Regex.Replace(longDescription, "<.*?>", "");
        }
    }
}