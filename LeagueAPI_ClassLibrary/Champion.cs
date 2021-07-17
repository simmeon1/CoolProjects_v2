using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class Champion
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public int Difficulty { get; set; }
        public string GetTagsString()
        {
            return Tags.ConcatenateListOfStringsToCommaString();
        }
    }
}
