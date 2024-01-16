using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LeagueAPI_ClassLibrary
{
    public class Item : ITableEntry
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Plaintext { get; set; } = "";
        public string Description { get; set; } = "";
        public int Gold { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<string> BuildsInto { get; set; } = new();

        public string GetTagsString()
        {
            return Tags.ConcatenateListOfStringsToCommaAndSpaceString();
        }

        public string GetCleanDescription()
        {
            return Regex.Replace(Description, "<.*?>", "");
        }
        
        public bool IsOrnnItem()
        {
            return Description.Contains("ornnBonus");
        }
        
        public bool IsMoreThan2000G()
        {
            return Gold > 2000;
        }

        public bool IsGuardian()
        {
            return Name.Contains("Guardian") && !Name.Contains("Angel");
        }     

        public bool IsBoots()
        {
            return Tags.Contains("Boots");
        }
        
        public bool IsDoran()
        {
            return Name.Contains("Doran");
        }
        
        public bool IsTearOfTheGoddess()
        {
            return Name.Contains("Tear of the Goddess");
        }

        public string GetSecondFormNameForTearItem()
        {
            Match match = Regex.Match(Description, "transforms into <rarityLegendary>(.*?)</rarityLegendary>", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.ToString() : "";
        }

        public string GetIdentifier()
        {
            return Name;
        }

        public string GetCategory()
        {
            return "Items";
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            return new List<KeyValuePair<string, object>>
            {
                new("Name", Name),
                new("Gold", Gold),
                new("More than 2000G", IsMoreThan2000G()),
                new("Is Ornn Item", IsOrnnItem()),
                new("Tags", GetTagsString()),
                new("Plaintext", Plaintext),
                new("Description", GetCleanDescription())
            };
        }
    }
}