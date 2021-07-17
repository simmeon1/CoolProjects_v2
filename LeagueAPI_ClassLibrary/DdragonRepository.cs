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
        public DdragonRepository(string ddragonJsonFilesDirectoryPath)
        {
            ChampionJson = JObject.Parse(File.ReadAllText(Path.Combine(ddragonJsonFilesDirectoryPath, "champion.json")));
        }

        public JObject GetChampion(int id)
        {
            foreach (JProperty champ in ChampionJson["data"]) if (int.Parse(champ.Value["key"].ToString()) == id) return (JObject)champ.Value;
            return null;
        }
    }
}
