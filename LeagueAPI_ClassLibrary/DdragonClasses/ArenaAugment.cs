using System;
using Common_ClassLibrary;
using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class ArenaAugment : ITableEntry
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Rarity { get; set; }
        public string Description { get; set; } = "";
        public string Tooltip { get; set; } = "";
        public Dictionary<string, string> DataValues { get; set; } = new();

        public string GetIdentifier()
        {
            return Name;
        }

        public string GetCategory()
        {
            return "Arena Augment";
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            return new List<KeyValuePair<string, object>>
            {
                new("Name", Name),
                new("Rarity", Rarity),
                new("Description", GetFixedText(Description)),
                new("Tooltip", GetFixedText(Tooltip)),
            };
        }

        private string GetFixedText(string text)
        {
            foreach ((string key, string value) in DataValues)
            {
                text = text.Replace($"@{key}", value);
            }
            return text;
        }
    }
}