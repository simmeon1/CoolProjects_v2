using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LeagueAPI_ClassLibrary
{
    public class DataTableCreator
    {
        private IDDragonRepository DDragonRepository { get; set; }
        private Type TypeString { get; set; }
        private Type TypeInt32 { get; set; }
        private Type TypeDouble { get; set; }

        public DataTableCreator(IDDragonRepository dDragonRepository)
        {
            DDragonRepository = dDragonRepository;
            TypeString = Type.GetType("System.String");
            TypeInt32 = Type.GetType("System.Int32");
            TypeDouble = Type.GetType("System.Double");
        }

        public DataTable GetChampionTable(Dictionary<int, WinLossData> championData)
        {
            const string nameColumnName = "Name";
            const string winsColumnName = "Wins";
            const string lossesColumnName = "Losses";
            const string totalColumnName = "Total";
            const string winRateColumName = "Win rate";
            const string tagsColumnName = "Tags";
            const string difficultyColumnName = "Difficulty";
            DataColumn nameColumn = new(nameColumnName, TypeString);
            DataColumn winsColumn = new(winsColumnName, TypeInt32);
            DataColumn lossesColumn = new(lossesColumnName, TypeInt32);
            DataColumn totalColumn = new(totalColumnName, TypeInt32);
            DataColumn winRateColumn = new(winRateColumName, TypeDouble);
            DataColumn tagsColumn = new(tagsColumnName, TypeString);
            DataColumn difficultyColumn = new(difficultyColumnName, TypeInt32);
            DataTable table = new("Champions");
            table.Columns.AddRange(new List<DataColumn> { nameColumn, winsColumn, lossesColumn, totalColumn, winRateColumn, tagsColumn, difficultyColumn }.ToArray());

            foreach (KeyValuePair<int, WinLossData> champEntry in championData)
            {
                JObject champ = DDragonRepository.GetChampion(champEntry.Key);
                string name = (string)champ["name"];
                JArray tags = (JArray)champ["tags"];
                StringBuilder tagsStr = new("");
                foreach (JToken tag in tags)
                {
                    if (tagsStr.Length > 0) tagsStr.Append(", ");
                    tagsStr.Append(tag.ToString());
                }
                int difficulty = (int)champ["info"]["difficulty"];
                DataRow row = table.NewRow();
                row[nameColumnName] = name;
                row[winsColumnName] = champEntry.Value.GetWins();
                row[lossesColumnName] = champEntry.Value.GetLosses();
                row[totalColumnName] = champEntry.Value.GetTotal();
                row[winRateColumName] = champEntry.Value.GetWinRate();
                row[tagsColumnName] = tagsStr.ToString();
                row[difficultyColumnName] = difficulty;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}