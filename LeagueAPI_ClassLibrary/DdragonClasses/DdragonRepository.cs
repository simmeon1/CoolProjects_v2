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
        private readonly ILeagueAPIClient client;
        private readonly Dictionary<int, Champion> champions = new();
        private readonly Dictionary<int, Item> items = new();
        private readonly Dictionary<int, Rune> runes = new();
        private readonly Dictionary<int, StatPerk> statPerks = new();
        private readonly Dictionary<int, Spell> spells = new();
        public DdragonRepository(ILeagueAPIClient client)
        {
            this.client = client;
        }
        
        public async Task RefreshData(string version)
        {
            Task<string> ddragonChampions = client.GetDdragonChampions(version);
            Task<string> ddragonItems = client.GetDdragonItems(version);
            Task<string> ddragonRunes = client.GetDdragonRunes(version);
            Task<string> ddragonStatPerks = client.GetDdragonStatPerks(version);
            Task<string> ddragonSpells = client.GetDdragonSpells(version);
            List<Task<string>> tasks = new()
                {ddragonChampions, ddragonItems, ddragonRunes, ddragonSpells, ddragonStatPerks};
            await Task.WhenAll(tasks);

            PopulateChampions(ddragonChampions.Result);
            PopulateItems(ddragonItems.Result);
            PopulateRunes(ddragonRunes.Result);
            PopulateStatPerks(ddragonStatPerks.Result);
            PopulateSpells(ddragonSpells.Result);
        }

        private void PopulateSpells(string spellsJson)
        {
            JObject spellJson = JObject.Parse(spellsJson);
            foreach (JProperty entry in spellJson["data"])
            {
                int id = int.Parse(entry.Value["key"].ToString());
                Spell spell = new()
                {
                    Name = entry.Value["name"].ToString(),
                    Cooldown = entry.Value["cooldown"][0].ToString(),
                    Description = entry.Value["description"].ToString()
                };
                spells.Add(id, spell);
            }
        }

        private void PopulateStatPerks(string perks)
        {
            JObject statPerkJson = JObject.Parse(perks);
            foreach (KeyValuePair<string, JToken> entry in statPerkJson)
            {
                int id = int.Parse(entry.Key);
                statPerks.Add(id, new StatPerk(entry.Value.ToString()));
            }
        }

        private void PopulateRunes(string runesJson)
        {
            JArray runeJson = JArray.Parse(runesJson);
            foreach (JToken treeEntry in runeJson)
            {
                string tree = (string) treeEntry["name"];
                for (int i = 0; i < treeEntry["slots"].Count(); i++)
                {
                    JToken runeRow = treeEntry["slots"][i];
                    foreach (JToken rune in runeRow["runes"])
                    {
                        Rune result = new(rune["name"].ToString(), tree, rune["longDesc"].ToString(), i);
                        runes.Add(int.Parse(rune["id"].ToString()), result);
                    }
                }
            }
        }

        private void PopulateItems(string itemsJson)
        {
            JObject itemJson = JObject.Parse(itemsJson);
            foreach (JProperty itemEntry in itemJson["data"])
            {
                int itemId = 0;
                bool itemHasValidNumericId = int.TryParse(itemEntry.Name, out itemId);
                if (!itemHasValidNumericId) continue;
                
                Item item = new()
                {
                    Id = itemId,
                    Name = itemEntry.Value["name"].ToString(),
                    Plaintext = itemEntry.Value["plaintext"].ToString(),
                    Description = itemEntry.Value["description"].ToString(),
                    Gold = int.Parse(itemEntry.Value["gold"]["total"].ToString())
                };
                
                List<string> buildsInto = new();
                JArray buildsIntoJAray = (JArray)itemEntry.Value["into"];
                if (buildsIntoJAray != null) foreach (JToken entry in buildsIntoJAray) buildsInto.Add(entry.ToString());
                item.BuildsInto = buildsInto;

                List<string> tags = new();
                foreach (JToken tag in itemEntry.Value["tags"]) tags.Add(tag.ToString());
                item.Tags = tags;
                
                items.Add(int.Parse(itemEntry.Name), item);
            }
        }

        private void PopulateChampions(string championsJson)
        {
            JObject jObject = JObject.Parse(championsJson);
            foreach (JProperty champ in jObject["data"])
            {
                int id = int.Parse(champ.Value["key"].ToString());
                Champion champion = new()
                {
                    Name = champ.Value["name"].ToString(),
                    Difficulty = (int) champ.Value["info"]["difficulty"],
                };
                List<string> tags = new();
                foreach (JToken tag in champ.Value["tags"]) tags.Add(tag.ToString());
                champion.Tags = tags;
                champions.Add(id, champion);
            }
        }

        public Champion GetChampion(int id)
        {
            return GetEntryOrNullFromDict(id, champions);
        }

        public Item GetItem(int id)
        {
            return GetEntryOrNullFromDict(id, items);
        }

        public Rune GetRune(int id)
        {
            return GetEntryOrNullFromDict(id, runes);
        }

        public StatPerk GetStatPerk(int id)
        {
            return GetEntryOrNullFromDict(id, statPerks);
        }

        public Spell GetSpell(int id)
        {
            return GetEntryOrNullFromDict(id, spells);
        }
        
        private static T GetEntryOrNullFromDict<T>(int id, IReadOnlyDictionary<int, T> dict)
        {
            return dict.ContainsKey(id) ? dict[id] : default;
        }

        public Item GetItem(string itemName)
        {
            foreach (Item item in items.Values.ToList())
            {
                string name = item.Name;
                if (name.Equals(itemName)) return item;
            }
            return null;
        }
    }
}
