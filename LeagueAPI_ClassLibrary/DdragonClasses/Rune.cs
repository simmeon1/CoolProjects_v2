using System.Text.RegularExpressions;

namespace LeagueAPI_ClassLibrary
{
    public class Rune
    {
        public string Name { get; set; } = "";
        public string Tree { get; set; } = "";
        public string LongDescription { get; set; } = "";
        public int Slot { get; set; }
        public string GetCleanDescription()
        {
            return Regex.Replace(LongDescription, "<.*?>", "");
        }
    }
}