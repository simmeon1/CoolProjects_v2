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
        private JObject StatPerkJson { get; set; }
        private JObject SpellJson { get; set; }
        public DdragonRepository(IFileIO fileIO, string ddragonJsonFilesDirectoryPath)
        {
            ChampionJson = JObject.Parse(fileIO.ReadAllText(Path.Combine(ddragonJsonFilesDirectoryPath, "champion.json")));
            ItemJson = JObject.Parse(fileIO.ReadAllText(Path.Combine(ddragonJsonFilesDirectoryPath, "item.json")));
            RuneJson = JArray.Parse(fileIO.ReadAllText(Path.Combine(ddragonJsonFilesDirectoryPath, "runesReforged.json")));
            StatPerkJson = JObject.Parse(fileIO.ReadAllText(Path.Combine(ddragonJsonFilesDirectoryPath, "statPerks.json")));
            SpellJson = JObject.Parse(fileIO.ReadAllText(Path.Combine(ddragonJsonFilesDirectoryPath, "summoner.json")));
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
                if (buildsInto == null)
                {
                    item.IsFinished = true;
                }
                else if (buildsInto.Count == 1)
                {
                    Item intoItem = GetItem(int.Parse(buildsInto[0].ToString()));
                    if (intoItem != null) item.IsFinished = intoItem.IsOrnnItem();
                }
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

        public string GetStatPerk(int id)
        {
            return StatPerkJson.ContainsKey(id.ToString()) ? StatPerkJson[id.ToString()].ToString() : null;
        }

        public Spell GetSpell(int id)
        {
            foreach (JProperty entry in SpellJson["data"])
            {
                if (int.Parse(entry.Value["key"].ToString()) != id) continue;
                Spell spell = new()
                {
                    Name = entry.Value["name"].ToString(),
                    Cooldown = int.Parse(entry.Value["cooldown"][0].ToString()),
                    Description = entry.Value["description"].ToString()
                };
                return spell;
            }
            return null;
        }
    }
}
