using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LeagueAPI_ClassLibrary
{
    public class DataTableCreator
    {
        private IDDragonRepository DDragonRepository { get; set; }
        private int ColumnIndexCounter { get; set; }
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

        public DataTable GetChampionTable(Dictionary<int, Dictionary<int, WinLossData>> championData)
        {
            DataTable table = GetTableWithDefaultColumns("Champions", championData.Keys.OrderBy(m => m).ToList());
            table.Columns.AddRange(new List<DataColumn> {
                new DataColumn("Tags", TypeString),
                new DataColumn("Difficulty", TypeInt32)
            }.ToArray());

            foreach (KeyValuePair<int, WinLossData> champEntry in championData[0])
            {
                ColumnIndexCounter = 0;
                Champion champ = DDragonRepository.GetChampion(champEntry.Key);
                DataRow row = table.NewRow();
                AddDefaultDataToRow(champ.Name, champEntry, championData, row);
                row[ReturnColumnIndexCounterAndIncrementIt()] = champ.GetTagsString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = champ.Difficulty;
                table.Rows.Add(row);
            }
            return table;
        }

        private void AddDefaultDataToRow(string name, KeyValuePair<int, WinLossData> entry, Dictionary<int, Dictionary<int, WinLossData>> fullData, DataRow row)
        {
            row[ReturnColumnIndexCounterAndIncrementIt()] = name;
            row[ReturnColumnIndexCounterAndIncrementIt()] = entry.Value.GetWins();
            row[ReturnColumnIndexCounterAndIncrementIt()] = entry.Value.GetLosses();
            row[ReturnColumnIndexCounterAndIncrementIt()] = entry.Value.GetTotal();
            row[ReturnColumnIndexCounterAndIncrementIt()] = entry.Value.GetWinRate();

            List<int> minuteKeys = fullData.Keys.OrderBy(m => m).ToList();
            for (int i = 1; i < minuteKeys.Count; i++)
            {
                int minuteKey = minuteKeys[i];
                bool dataIsAvailable = fullData[minuteKey].ContainsKey(entry.Key);
                row[ReturnColumnIndexCounterAndIncrementIt()] = !dataIsAvailable ? 0 : fullData[minuteKey][entry.Key].GetWins();
                row[ReturnColumnIndexCounterAndIncrementIt()] = !dataIsAvailable ? 0 : fullData[minuteKey][entry.Key].GetLosses();
                row[ReturnColumnIndexCounterAndIncrementIt()] = !dataIsAvailable ? 0 : fullData[minuteKey][entry.Key].GetTotal();
                row[ReturnColumnIndexCounterAndIncrementIt()] = !dataIsAvailable ? 0 : fullData[minuteKey][entry.Key].GetWinRate();
                row[ReturnColumnIndexCounterAndIncrementIt()] = !dataIsAvailable ? 0 : fullData[minuteKey][entry.Key].GetWinRate() - entry.Value.GetWinRate();
            }
        }

        public DataTable GetItemTable(Dictionary<int, Dictionary<int, WinLossData>> itemData)
        {
            DataTable table = GetTableWithDefaultColumns("Items", itemData.Keys.OrderBy(m => m).ToList());
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

            foreach (KeyValuePair<int, WinLossData> itemEntry in itemData[0])
            {
                ColumnIndexCounter = 0;
                Item item = DDragonRepository.GetItem(itemEntry.Key);
                if (item == null) continue;
                DataRow row = table.NewRow();
                AddDefaultDataToRow(item.Name, itemEntry, itemData, row);
                row[ReturnColumnIndexCounterAndIncrementIt()] = item.Gold;
                row[ReturnColumnIndexCounterAndIncrementIt()] = item.IsMoreThan2000G();
                row[ReturnColumnIndexCounterAndIncrementIt()] = item.IsMythic();
                row[ReturnColumnIndexCounterAndIncrementIt()] = item.IsOrnnItem();
                row[ReturnColumnIndexCounterAndIncrementIt()] = item.IsFinished;
                row[ReturnColumnIndexCounterAndIncrementIt()] = item.GetTagsString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = item.Plaintext;
                row[ReturnColumnIndexCounterAndIncrementIt()] = item.GetCleanDescription();
                table.Rows.Add(row);
            }
            return table;
        }

        private DataTable GetTableWithDefaultColumns(string tableName, List<int> minuteKeys)
        {
            DataTable table = new(tableName);
            List<DataColumn> dataColumns = new()
            {
                new DataColumn("Name", TypeString),
                new DataColumn("Wins", TypeInt32),
                new DataColumn("Losses", TypeInt32),
                new DataColumn("Total", TypeInt32),
                new DataColumn("Win rate", TypeDouble)
            };

            for (int i = 1; i < minuteKeys.Count; i++)
            {
                int minute = minuteKeys[i];
                dataColumns.AddRange(new List<DataColumn>()
                {
                    new DataColumn($"Wins_{minute}", TypeInt32),
                    new DataColumn($"Losses_{minute}", TypeInt32),
                    new DataColumn($"Total_{minute}", TypeInt32),
                    new DataColumn($"Win rate_{minute}", TypeDouble),
                    new DataColumn($"Win rate diff_{minute}", TypeDouble)
                });
            }

            table.Columns.AddRange(dataColumns.ToArray());
            return table;
        }

        public DataTable GetRuneTable(Dictionary<int, Dictionary<int, WinLossData>> data)
        {
            DataTable table = GetTableWithDefaultColumns("Runes", data.Keys.OrderBy(m => m).ToList());
            table.Columns.AddRange(new List<DataColumn> {
                new DataColumn("Tree", TypeString),
                new DataColumn("Slot", TypeInt32),
                new DataColumn("Description", TypeString)
            }.ToArray());

            foreach (KeyValuePair<int, WinLossData> entry in data[0])
            {
                ColumnIndexCounter = 0;
                Rune item = DDragonRepository.GetRune(entry.Key);
                DataRow row = table.NewRow();
                AddDefaultDataToRow(item.Name, entry, data, row);
                row[ReturnColumnIndexCounterAndIncrementIt()] = item.Tree;
                row[ReturnColumnIndexCounterAndIncrementIt()] = item.Slot;
                row[ReturnColumnIndexCounterAndIncrementIt()] = item.GetCleanDescription();
                table.Rows.Add(row);
            }
            return table;
        }

        public DataTable GetSpellTable(Dictionary<int, Dictionary<int, WinLossData>> data)
        {
            DataTable table = GetTableWithDefaultColumns("Spells", data.Keys.OrderBy(m => m).ToList());
            table.Columns.AddRange(new List<DataColumn> {
                new DataColumn("Cooldown", TypeInt32),
                new DataColumn("Description", TypeString)
            }.ToArray());

            foreach (KeyValuePair<int, WinLossData> entry in data[0])
            {
                ColumnIndexCounter = 0;
                Spell spell = DDragonRepository.GetSpell(entry.Key);
                DataRow row = table.NewRow();
                AddDefaultDataToRow(spell.Name, entry, data, row);
                row[ReturnColumnIndexCounterAndIncrementIt()] = spell.Cooldown;
                row[ReturnColumnIndexCounterAndIncrementIt()] = spell.Description;
                table.Rows.Add(row);
            }
            return table;
        }

        public DataTable GetStatPerkTable(Dictionary<int, Dictionary<int, WinLossData>> data)
        {
            DataTable table = GetTableWithDefaultColumns("Stat Perks", data.Keys.OrderBy(m => m).ToList());
            foreach (KeyValuePair<int, WinLossData> entry in data[0])
            {
                ColumnIndexCounter = 0;
                DataRow row = table.NewRow();
                AddDefaultDataToRow(DDragonRepository.GetStatPerk(entry.Key), entry, data, row);
                table.Rows.Add(row);
            }
            return table;
        }

        private int ReturnColumnIndexCounterAndIncrementIt()
        {
            int result = ColumnIndexCounter;
            ColumnIndexCounter++;
            return result;
        }
    }
}