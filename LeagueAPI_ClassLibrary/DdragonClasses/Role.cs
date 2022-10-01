using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class Role: ITableEntry
    {
        private readonly string role;

        public Role(string role)
        {
            this.role = role;
        }

        public string GetIdentifier()
        {
            return role;
        }

        public string GetCategory()
        {
            return "Roles";
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            return new List<KeyValuePair<string, object>>
            {
                new("Name", role),
            };
        }
    }
}