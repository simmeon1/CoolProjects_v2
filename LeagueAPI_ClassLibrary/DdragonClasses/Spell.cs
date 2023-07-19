using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LeagueAPI_ClassLibrary
{
    public class Spell: ITableEntry
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Cooldown { get; set; } = "";
        public string GetIdentifier()
        {
            return Name;
        }

        public string GetCategory()
        {
            return "Spells";
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            return new List<KeyValuePair<string, object>>
            {
                new("Name", Name),
                new("Cooldown", Cooldown),
                new("Description", Description),
            };
        }
    }
}