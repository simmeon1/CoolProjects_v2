using System.Collections.Generic;
using System.Linq;
using Common_ClassLibrary;

namespace LeagueAPI_ClassLibrary
{
    public class TeamComposition: ITableEntry
    {
        private readonly string roles;
        private readonly int uniqueRoles;

        public TeamComposition(List<string> roles)
        {
            this.roles = roles.ConcatenateListOfStringsToCommaAndSpaceString();
            uniqueRoles = roles.Distinct().Count();
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
                new("Variety", uniqueRoles),
            };
        }
    }
}