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
        private JArray RuneJson { get; set; }
        public DdragonRepository(string ddragonJsonFilesDirectoryPath)
        {
            ChampionJson = JObject.Parse(File.ReadAllText(Path.Combine(ddragonJsonFilesDirectoryPath, "champion.json")));
            ItemJson = JObject.Parse(File.ReadAllText(Path.Combine(ddragonJsonFilesDirectoryPath, "item.json")));
            RuneJson = JArray.Parse(File.ReadAllText(Path.Combine(ddragonJsonFilesDirectoryPath, "runesReforged.json")));
        }

        public Champion GetChampion(int id)
        {
            foreach (JProperty champ in ChampionJson["data"])
            {
                if (int.Parse(champ.Value["key"].ToString()) != id) continue;
                Champion champion = new()
                {
                    Name = champ.Value["name"].ToString(),
                    Difficulty = (int)champ.Value["info"]["difficulty"],
                };
                List<string> tags = new();
                foreach (JToken tag in champ.Value["tags"]) tags.Add(tag.ToString());
                champion.Tags = tags;
                return champion;
            }
            return null;
        }

        public Item GetItem(int id)
        {
            foreach (JProperty itemEntry in ItemJson["data"])
            {
                if (int.Parse(itemEntry.Name) != id) continue;
                Item item = new()
                {
                    Name = itemEntry.Value["name"].ToString(),
                    Plaintext = itemEntry.Value["plaintext"].ToString(),
                    Description = itemEntry.Value["description"].ToString(),
                    Gold = int.Parse(itemEntry.Value["gold"]["total"].ToString())
                };
                JArray buildsInto = (JArray)itemEntry.Value["into"];
                item.IsFinished = buildsInto == null || (buildsInto.Count == 1 && GetItem(int.Parse(buildsInto[0].ToString())).IsOrnnItem());
                List<string> tags = new();
                foreach (JToken tag in itemEntry.Value["tags"]) tags.Add(tag.ToString());
                item.Tags = tags;
                return item;
            }
            return null;
        }

        public Rune GetRune(int id)
        {
            foreach (JToken treeEntry in RuneJson)
            {
                string tree = (string)treeEntry["name"];
                for (int i = 0; i < treeEntry["slots"].Count(); i++)
                {
                    JToken runeRow = treeEntry["slots"][i];
                    foreach (JToken rune in runeRow["runes"])
                    {
                        if (int.Parse(rune["id"].ToString()) != id) continue;
                        Rune result = new()
                        {
                            Name = rune["name"].ToString(),
                            Slot = i,
                            LongDescription = rune["longDesc"].ToString(),
                            Tree = tree
                        };
                        return result;
                    }
                }
            }
            return null;
        }
    }
}
