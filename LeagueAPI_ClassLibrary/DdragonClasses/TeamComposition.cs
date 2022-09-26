using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class TeamComposition: ITableEntry
    {
        private string roles;

        public TeamComposition(string roles)
        {
            this.roles = roles;
        }

        public string GetIdentifier()
        {
            return roles;
        }

        public string GetCategory()
        {
            return "Team Compositions";
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            return new List<KeyValuePair<string, object>>
            {
                new("Name", roles),
            };
        }
    }
}