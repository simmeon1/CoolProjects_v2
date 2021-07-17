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
        private Type TypeBool { get; set; }

        public DataTableCreator(IDDragonRepository dDragonRepository)
        {
            DDragonRepository = dDragonRepository;
            TypeString = Type.GetType("System.String");
            TypeInt32 = Type.GetType("System.Int32");
            TypeDouble = Type.GetType("System.Double");
            TypeBool = Type.GetType("System.Boolean");
        }

        public DataTable GetChampionTable(Dictionary<int, WinLossData> championData)
        {
            DataTable table = GetTableWithDefaultData("Champions");
            table.Columns.AddRange(new List<DataColumn> {
                new DataColumn("Tags", TypeString),
                new DataColumn("Difficulty", TypeInt32)
            }.ToArray());

            foreach (KeyValuePair<int, WinLossData> champEntry in championData)
            {
                Champion champ = DDragonRepository.GetChampion(champEntry.Key);
                DataRow row = table.NewRow();
                AddDefaultDataToRow(champ.Name, champEntry, row);
                row[5] = champ.GetTagsString();
                row[6] = champ.Difficulty;
                table.Rows.Add(row);
            }
            return table;
        }

        private void AddDefaultDataToRow(string name, KeyValuePair<int, WinLossData> entry, DataRow row)
        {
            row[0] = name;
            row[1] = entry.Value.GetWins();
            row[2] = entry.Value.GetLosses();
            row[3] = entry.Value.GetTotal();
            row[4] = entry.Value.GetWinRate();
        }

        public DataTable GetItemTable(Dictionary<int, WinLossData> itemData)
        {
            DataTable table = GetTableWithDefaultData("Items");
            table.Columns.AddRange(new List<DataColumn> {
                new DataColumn("Gold", TypeInt32),
                new DataColumn("More than 2000G", TypeBool),
                new DataColumn("Tags", TypeString),
                new DataColumn("Plaintext", TypeString),
                new DataColumn("Description", TypeString)
            }.ToArray());

            foreach (KeyValuePair<int, WinLossData> itemEntry in itemData)
            {
                Item item = DDragonRepository.GetItem(itemEntry.Key);
                DataRow row = table.NewRow();
                AddDefaultDataToRow(item.Name, itemEntry, row);
                row[5] = item.Gold;
                row[6] = item.IsMoreThan2000G();
                row[7] = item.GetTagsString();
                row[8] = item.Plaintext;
                row[9] = item.GetCleanDescription();
                table.Rows.Add(row);
            }
            return table;
        }

        private DataTable GetTableWithDefaultData(string tableName)
        {
            DataColumn nameColumn = new("Name", TypeString);
            DataColumn winsColumn = new("Wins", TypeInt32);
            DataColumn lossesColumn = new("Losses", TypeInt32);
            DataColumn totalColumn = new("Total", TypeInt32);
            DataColumn winRateColumn = new("Win rate", TypeDouble);
            DataTable table = new(tableName);
            table.Columns.AddRange(new List<DataColumn> { nameColumn, winsColumn, lossesColumn, totalColumn, winRateColumn }.ToArray());
            return table;
        }

        public DataTable GetRuneData(Dictionary<int, WinLossData> data)
        {
            DataTable table = GetTableWithDefaultData("Runes");
            table.Columns.AddRange(new List<DataColumn> {
                new DataColumn("Tree", TypeString),
                new DataColumn("Description", TypeString),
                new DataColumn("Is Keystone", TypeBool)
            }.ToArray());

            foreach (KeyValuePair<int, WinLossData> entry in data)
            {
                Rune item = DDragonRepository.GetRune(entry.Key);
                DataRow row = table.NewRow();
                AddDefaultDataToRow(item.Name, entry, row);
                row[5] = item.Tree;
                row[6] = item.GetCleanDescription();
                row[7] = item.IsKeystone;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}