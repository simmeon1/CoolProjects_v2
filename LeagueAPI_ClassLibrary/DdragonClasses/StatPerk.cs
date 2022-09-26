using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class StatPerk: ITableEntry
    {
        private string description;

        public StatPerk(string description)
        {
            this.description = description;
        }

        public string GetIdentifier()
        {
            return description;
        }

        public string GetCategory()
        {
            return "Stat Perks";
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            return new List<KeyValuePair<string, object>>
            {
                new("Name", description)
            };
        }
    }
}