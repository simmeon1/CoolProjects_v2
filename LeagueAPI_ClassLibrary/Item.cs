using System.Text.RegularExpressions;

namespace LeagueAPI_ClassLibrary
{
    public class Item
    {
        public string Name { get; set; }
        public string Plaintext { get; set; }
        public string Description { get; set; }
        public int Gold { get; set; }
        public string GetCleanDescription()
        {
            return Regex.Replace(Description, "<.*?>", "");
        }
        
        public bool IsMoreThan2000G()
        {
            return Gold > 2000;
        }
    }
}