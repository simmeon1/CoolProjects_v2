using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueAPI_ClassLibrary
{
    public class ItemSetExporter
    {
        const string jsonTitle = "jsonTitle";
        const string guardianJson = "guardianJson";
        const string bootsJson = "bootsJson";
        const string doranJson = "doranJson";
        const string mythics50PlusJson = "mythics50PlusJson";
        const string mythics50MinusJson = "mythics50MinusJson";
        const string legendaries50PlusJson = "legendaries50PlusJson";
        const string legendaries50MinusJson = "legendaries50MinusJson";
        private IDDragonRepository Repository { get; set; }
        private string BaseJson { get; set; } = @"
        {
          'title': '" + jsonTitle + @"',
          'associatedMaps': [
            11,
            12
          ],
          'associatedChampions': [],
          'blocks': [
            {
              'items': " + guardianJson + @",
              'type': 'Guardian'
            },
            {
              'items': " + doranJson + @",
              'type': 'Doran'
            },
            {
              'items': " + bootsJson + @",
              'type': 'Boots'
            },
            {
              'items': " + mythics50PlusJson + @",
              'type': 'Mythics 50+ WR'
            },
            {
              'items': " + mythics50MinusJson + @",
              'type': 'Mythics 50- WR'
            },
            {
              'items': " + legendaries50PlusJson + @",
              'type': 'Legendaries 50+ WR'
            },
            {
              'items': " + legendaries50MinusJson + @",
              'type': 'Legendaries 50- WR'
            }
          ]
        }";

        public ItemSetExporter(IDDragonRepository repository)
        {
            Repository = repository;
        }

        public string GetItemSet(Dictionary<int, WinLossData> itemData, string itemSetName)
        {
            List<ItemEntry> guardianJsonArray = new();
            List<ItemEntry> bootsJsonArray = new();
            List<ItemEntry> doranJsonArray = new();
            List<ItemEntry> mythicsJsonArray = new();
            List<ItemEntry> legendariesJsonArray = new();

            Item tear = null;
            List<KeyValuePair<int, WinLossData>> sortedList = itemData.OrderByDescending(x => x.Value.GetWinRate()).ToList();
            foreach (KeyValuePair<int, WinLossData> entry in sortedList)
            {
                int id = entry.Key;
                double winRate = entry.Value.GetWinRate();
                Item item = Repository.GetItem(id);
                if (item == null) continue;

                ItemEntry itemEntry = new(id, winRate);

                if (item.IsTearOfTheGoddess()) tear = item;
                if (item.IsGuardian()) guardianJsonArray.Add(itemEntry);
                else if (item.IsBoots()) bootsJsonArray.Add(itemEntry);
                else if (item.IsDoran()) doranJsonArray.Add(itemEntry);
                else if (item.IsMythic()) mythicsJsonArray.Add(itemEntry);
                else if (item.IsFinished() && item.IsMoreThan2000G()) legendariesJsonArray.Add(itemEntry);
            }

            List<ItemEntry> legendariesAmendedJsonArray = new();
            Dictionary<int, Item> secondFormIdAndFirstForm = new();
            if (tear != null)
            {
                foreach (string firstFormId in tear.BuildsInto)
                {
                    int firstFormIdInt = int.Parse(firstFormId);
                    Item firstFormItem = Repository.GetItem(firstFormIdInt);
                    string secondFormItemName = firstFormItem.GetSecondFormNameForTearItem();
                    if (secondFormItemName.IsNullOrEmpty()) continue;

                    Item secondFormItem = Repository.GetItem(secondFormItemName);
                    int secondFormIdInt = secondFormItem.Id;

                    secondFormIdAndFirstForm.Add(secondFormIdInt, firstFormItem);
                }

                foreach (ItemEntry item in legendariesJsonArray)
                {
                    if (!tear.BuildsInto.Contains(item.Id.ToString()))
                    {
                        legendariesAmendedJsonArray.Add(item);
                        if (secondFormIdAndFirstForm.ContainsKey(item.Id))
                        {
                            Item firstForm = secondFormIdAndFirstForm[item.Id];
                            legendariesAmendedJsonArray.Add(new ItemEntry(firstForm.Id, itemData[item.Id].GetWinRate()));
                        }
                    }
                }
            }

            return BaseJson
                .Replace(jsonTitle, itemSetName)
                .Replace(guardianJson, GetSerializedList(guardianJsonArray, (x => true)))
                .Replace(bootsJson, GetSerializedList(bootsJsonArray, (x => true)))
                .Replace(doranJson, GetSerializedList(doranJsonArray, (x => true)))
                .Replace(mythics50PlusJson, GetSerializedList(mythicsJsonArray, (x => x.WinRateIsEqualOrAbove50())))
                .Replace(mythics50MinusJson, GetSerializedList(mythicsJsonArray, (x => !x.WinRateIsEqualOrAbove50())))
                .Replace(legendaries50PlusJson, GetSerializedList(legendariesAmendedJsonArray, (x => x.WinRateIsEqualOrAbove50())))
                .Replace(legendaries50MinusJson, GetSerializedList(legendariesAmendedJsonArray, (x => !x.WinRateIsEqualOrAbove50())))
                .Replace("'", "\"");
        }

        private static string GetSerializedList(List<ItemEntry> items, Func<ItemEntry, bool> winRateFunc)
        {
            return items.Where(x => winRateFunc.Invoke(x)).Select(x => x.GetSerialized()).ToList().SerializeObject();
        }

        private class ItemEntry
        {
            public int Id { get; set; }
            public double WinRate { get; set; }

            public ItemEntry(int id, double winRate)
            {
                Id = id;
                WinRate = winRate;
            }

            public bool WinRateIsEqualOrAbove50()
            {
                return WinRate >= 50;
            }

            public object GetSerialized()
            {
                return new { id = Id.ToString(), count = 1 };
            }
        }
    }
}
