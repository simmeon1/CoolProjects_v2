using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LeagueAPI_ClassLibrary
{
    public class Item
    {
        public string Name { get; set; } = "";
        public string Plaintext { get; set; } = "";
        public string Description { get; set; } = "";
        public int Gold { get; set; }
        public List<string> Tags { get; set; } = new();
        public bool IsFinished { get; set; }

        public string GetTagsString()
        {
            return Tags.ConcatenateListOfStringsToCommaString();
        }

        public string GetCleanDescription()
        {
            return Regex.Replace(Description, "<.*?>", "");
        }
        
        public bool IsMythic()
        {
            return Description.Contains("rarityMythic");
        }
        
        public bool IsMoreThan2000G()
        {
            return Gold > 2000;
        }
        
        public bool IsOrnnItem()
        {
            return Description.Contains("ornnBonus");
        }

        public bool IsGuardian()
        {
            return Name.Contains("Guardian");
        }     
        
        public bool IsLegendary()
        {
            return Description.Contains("rarityLegendary");
        }

        public bool IsBoots()
        {
            return Tags.Contains("Boots");
        }
    }
}