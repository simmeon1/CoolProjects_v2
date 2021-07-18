using System.Text.RegularExpressions;

namespace LeagueAPI_ClassLibrary
{
    public class Spell
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Cooldown { get; set; }
    }
}