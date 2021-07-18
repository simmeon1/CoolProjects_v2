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
        private int RowIndexCounter { get; set; }
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
            DataTable table = GetTableWithDefaultColumns("Champions");
            table.Columns.AddRange(new List<DataColumn> {
                new DataColumn("Tags", TypeString),
                new DataColumn("Difficulty", TypeInt32)
            }.ToArray());

            foreach (KeyValuePair<int, WinLossData> champEntry in championData)
            {
                RowIndexCounter = 0;
                Champion champ = DDragonRepository.GetChampion(champEntry.Key);
                DataRow row = table.NewRow();
                AddDefaultDataToRow(champ.Name, champEntry, row);
                row[ReturnRowIndexCounterAndIncrementIt()] = champ.GetTagsString();
                row[ReturnRowIndexCounterAndIncrementIt()] = champ.Difficulty;
                table.Rows.Add(row);
            }
            return table;
        }

        private void AddDefaultDataToRow(string name, KeyValuePair<int, WinLossData> entry, DataRow row)
        {
            row[ReturnRowIndexCounterAndIncrementIt()] = name;
            row[ReturnRowIndexCounterAndIncrementIt()] = entry.Value.GetWins();
            row[ReturnRowIndexCounterAndIncrementIt()] = entry.Value.GetLosses();
            row[ReturnRowIndexCounterAndIncrementIt()] = entry.Value.GetTotal();
            row[ReturnRowIndexCounterAndIncrementIt()] = entry.Value.GetWinRate();
        }

        public DataTable GetItemTable(Dictionary<int, WinLossData> itemData)
        {
            DataTable table = GetTableWithDefaultColumns("Items");
            table.Columns.AddRange(new List<DataColumn> {
                new DataColumn("Gold", TypeInt32),
                new DataColumn("More than 2000G", TypeBool),
                new DataColumn("Is Mythic", TypeBool),
                new DataColumn("Is Ornn Item", TypeBool),
                new DataColumn("Is Finished", TypeBool),
                new DataColumn("Tags", TypeString),
                new DataColumn("Plaintext", TypeString),
                new DataColumn("Description", TypeString)
            }.ToArray());

            foreach (KeyValuePair<int, WinLossData> itemEntry in itemData)
            {
                RowIndexCounter = 0;
                Item item = DDragonRepository.GetItem(itemEntry.Key);
                DataRow row = table.NewRow();
                AddDefaultDataToRow(item.Name, itemEntry, row);
                row[ReturnRowIndexCounterAndIncrementIt()] = item.Gold;
                row[ReturnRowIndexCounterAndIncrementIt()] = item.IsMoreThan2000G();
                row[ReturnRowIndexCounterAndIncrementIt()] = item.IsMythic();
                row[ReturnRowIndexCounterAndIncrementIt()] = item.IsOrnnItem();
                row[ReturnRowIndexCounterAndIncrementIt()] = item.IsFinished;
                row[ReturnRowIndexCounterAndIncrementIt()] = item.GetTagsString();
                row[ReturnRowIndexCounterAndIncrementIt()] = item.Plaintext;
                row[ReturnRowIndexCounterAndIncrementIt()] = item.GetCleanDescription();
                table.Rows.Add(row);
            }
            return table;
        }

        private DataTable GetTableWithDefaultColumns(string tableName)
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

        public DataTable GetRuneTable(Dictionary<int, WinLossData> data)
        {
            DataTable table = GetTableWithDefaultColumns("Runes");
            table.Columns.AddRange(new List<DataColumn> {
                new DataColumn("Tree", TypeString),
                new DataColumn("Slot", TypeInt32),
                new DataColumn("Description", TypeString)
            }.ToArray());

            foreach (KeyValuePair<int, WinLossData> entry in data)
            {
                RowIndexCounter = 0;
                Rune item = DDragonRepository.GetRune(entry.Key);
                DataRow row = table.NewRow();
                AddDefaultDataToRow(item.Name, entry, row);
                row[ReturnRowIndexCounterAndIncrementIt()] = item.Tree;
                row[ReturnRowIndexCounterAndIncrementIt()] = item.Slot;
                row[ReturnRowIndexCounterAndIncrementIt()] = item.GetCleanDescription();
                table.Rows.Add(row);
            }
            return table;
        }

        private int ReturnRowIndexCounterAndIncrementIt()
        {
            int result = RowIndexCounter;
            RowIndexCounter++;
            return result;
        }
    }
}