using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class DdragonRepository : IDDragonRepository
    {
        private JObject ChampionJson { get; set; }
        private JObject ItemJson { get; set; }
        public DdragonRepository(string ddragonJsonFilesDirectoryPath)
        {
            ChampionJson = JObject.Parse(File.ReadAllText(Path.Combine(ddragonJsonFilesDirectoryPath, "champion.json")));
            ItemJson = JObject.Parse(File.ReadAllText(Path.Combine(ddragonJsonFilesDirectoryPath, "item.json")));
        }

        public Champion GetChampion(int id)
        {
            Champion result = null;
            foreach (JProperty champ in ChampionJson["data"])
            {
                if (int.Parse(champ.Value["key"].ToString()) == id)
                {
                    Champion champion = new()
                    {
                        Name = champ.Value["name"].ToString(),
                        Difficulty = (int)champ.Value["info"]["difficulty"],
                    };
                    List<string> tags = new();
                    foreach (JToken tag in champ.Value["tags"]) tags.Add(tag.ToString());
                    champion.Tags = tags;
                    result = champion;
                    break;
                }
            }
            return result;
        }

        public Item GetItem(int id)
        {
            Item result = null;
            foreach (JProperty itemEntry in ItemJson["data"])
            {
                if (int.Parse(itemEntry.Name) == id)
                {
                    Item item = new()
                    {
                        Name = itemEntry.Value["name"].ToString(),
                        Plaintext = itemEntry.Value["plaintext"].ToString(),
                        Description = itemEntry.Value["description"].ToString(),
                        Gold = int.Parse(itemEntry.Value["gold"]["total"].ToString())
                    };
                    List<string> tags = new();
                    foreach (JToken tag in itemEntry.Value["tags"]) tags.Add(tag.ToString());
                    item.Tags = tags;
                    result = item;
                    break;
                }
            }
            return result;
        }
    }
}
