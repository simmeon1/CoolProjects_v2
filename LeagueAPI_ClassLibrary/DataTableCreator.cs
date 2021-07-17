using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LeagueAPI_ClassLibrary
{
    public class DataTableCreator
    {
        private const string nameColumnName = "Name";
        private const string winsColumnName = "Wins";
        private const string lossesColumnName = "Losses";
        private const string totalColumnName = "Total";
        private const string winRateColumName = "Win rate";
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
            DataTable table = GetTableWithDefaultData("Champions");
            const string tagsColumnName = "Tags";
            const string difficultyColumnName = "Difficulty";
            table.Columns.AddRange(new List<DataColumn> { new DataColumn(tagsColumnName, TypeString), new DataColumn(difficultyColumnName, TypeInt32) }.ToArray());

            foreach (KeyValuePair<int, WinLossData> champEntry in championData)
            {
                Champion champ = DDragonRepository.GetChampion(champEntry.Key);
                DataRow row = table.NewRow();
                AddDefaultDataToRow(champ.Name, champEntry, row);
                row[tagsColumnName] = champ.GetTagsString();
                row[difficultyColumnName] = champ.Difficulty;
                table.Rows.Add(row);
            }
            return table;
        }

        private void AddDefaultDataToRow(string name, KeyValuePair<int, WinLossData> entry, DataRow row)
        {
            row[nameColumnName] = name;
            row[winsColumnName] = entry.Value.GetWins();
            row[lossesColumnName] = entry.Value.GetLosses();
            row[totalColumnName] = entry.Value.GetTotal();
            row[winRateColumName] = entry.Value.GetWinRate();
        }

        private DataTable GetTableWithDefaultData(string tableName)
        {
            DataColumn nameColumn = new(nameColumnName, TypeString);
            DataColumn winsColumn = new(winsColumnName, TypeInt32);
            DataColumn lossesColumn = new(lossesColumnName, TypeInt32);
            DataColumn totalColumn = new(totalColumnName, TypeInt32);
            DataColumn winRateColumn = new(winRateColumName, TypeDouble);
            DataTable table = new(tableName);
            table.Columns.AddRange(new List<DataColumn> { nameColumn, winsColumn, lossesColumn, totalColumn, winRateColumn }.ToArray());
            return table;
        }
    }
}