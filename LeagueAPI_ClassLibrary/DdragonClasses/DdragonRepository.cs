using Common_ClassLibrary;
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
        private IFileIO FileIO { get; set; }
        private string RepoPath { get; set; }
        private JObject ChampionJson { get; set; }
        private JObject ItemJson { get; set; }
        private JArray RuneJson { get; set; }
        private JObject StatPerkJson { get; set; }
        private JObject SpellJson { get; set; }
        public DdragonRepository(IFileIO fileIO, string repoPath)
        {
            FileIO = fileIO;
            RepoPath = repoPath;
        }

        public void RefreshData()
        {
            ChampionJson = JObject.Parse(FileIO.ReadAllText(Path.Combine(RepoPath, "champion.json")));
            ItemJson = JObject.Parse(FileIO.ReadAllText(Path.Combine(RepoPath, "item.json")));
            RuneJson = JArray.Parse(FileIO.ReadAllText(Path.Combine(RepoPath, "runesReforged.json")));
            StatPerkJson = JObject.Parse(FileIO.ReadAllText(Path.Combine(RepoPath, "statPerks.json")));
            SpellJson = JObject.Parse(FileIO.ReadAllText(Path.Combine(RepoPath, "summoner.json")));
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
                if (int.Parse(itemEntry.Name) == id) return GetItemFromEntry(itemEntry);
            }
            return null;
        }

        private static Item GetItemFromEntry(JProperty itemEntry)
        {
            Item item = new()
            {
                Id = int.Parse(itemEntry.Name),
                Name = itemEntry.Value["name"].ToString(),
                Plaintext = itemEntry.Value["plaintext"].ToString(),
                Description = itemEntry.Value["description"].ToString(),
                Gold = int.Parse(itemEntry.Value["gold"]["total"].ToString())
            };

            GetBuildsIntoData(itemEntry, item);
            GetTagsData(itemEntry, item);
            return item;
        }

        private static void GetTagsData(JProperty itemEntry, Item item)
        {
            List<string> tags = new();
            foreach (JToken tag in itemEntry.Value["tags"]) tags.Add(tag.ToString());
            item.Tags = tags;
        }

        private static void GetBuildsIntoData(JProperty itemEntry, Item item)
        {
            List<string> buildsInto = new();
            JArray buildsIntoJAray = (JArray)itemEntry.Value["into"];
            if (buildsIntoJAray != null) foreach (JToken entry in buildsIntoJAray) buildsInto.Add(entry.ToString());
            item.BuildsInto = buildsInto;
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

        public Item GetItem(string itemName)
        {
            foreach (JProperty itemEntry in ItemJson["data"])
            {
                string name = itemEntry.Value["name"].ToString();
                if (name.Equals(itemName)) return GetItemFromEntry(itemEntry);
            }
            return null;
        }
    }
}
