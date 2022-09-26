using Common_ClassLibrary;
using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class Champion: ITableEntry
    {
        public string Name { get; set; } = "";
        public List<string> Tags { get; set; } = new();
        public int Difficulty { get; set; }

        public string GetFirstTag()
        {
            return Tags[0];
        }

        public string GetIdentifier()
        {
            return Name;
        }

        public string GetCategory()
        {
            return "Champions";
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            return new List<KeyValuePair<string, object>>
            {
                new("Name", Name),
                new("Tags", Tags.ConcatenateListOfStringsToCommaAndSpaceString()),
                new("Difficulty", Difficulty),
            };
        }
    }
}
