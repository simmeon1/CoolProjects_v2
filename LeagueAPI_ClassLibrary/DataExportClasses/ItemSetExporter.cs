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

        public string GetItemSet(List<TableEntry<Item>> itemData, string itemSetName)
        {
            List<ItemEntry> guardianJsonArray = new();
            List<ItemEntry> bootsJsonArray = new();
            List<ItemEntry> doranJsonArray = new();
            List<ItemEntry> legendariesJsonArray = new();

            Item tear = null;
            List<TableEntry<Item>> sortedList =
                itemData.OrderByDescending(x => x.GetWinLossData().GetWinRate()).ToList();
            foreach (TableEntry<Item> entry in sortedList)
            {
                double winRate = entry.GetWinLossData().GetWinRate();
                Item item = entry.GetEntry();
                ItemEntry itemEntry = new(item.Id, winRate);

                if (item.IsTearOfTheGoddess()) tear = item;
                if (item.IsGuardian()) guardianJsonArray.Add(itemEntry);
                else if (item.IsBoots()) bootsJsonArray.Add(itemEntry);
                else if (item.IsDoran()) doranJsonArray.Add(itemEntry);
                else if (!item.IsOrnnItem() && item.IsMoreThan2000G()) legendariesJsonArray.Add(itemEntry);
            }

            List<ItemEntry> legendariesAmendedJsonArray =
                GetLegendariesWithAmendedTearItemPositioning(itemData, legendariesJsonArray, tear);

            return BaseJson
                .Replace(jsonTitle, itemSetName)
                .Replace(guardianJson, GetSerializedList(guardianJsonArray, (x => true)))
                .Replace(bootsJson, GetSerializedList(bootsJsonArray, (x => true)))
                .Replace(doranJson, GetSerializedList(doranJsonArray, (x => true)))
                .Replace(
                    legendaries50PlusJson,
                    GetSerializedList(legendariesAmendedJsonArray, (x => x.WinRateIsEqualOrAbove50()))
                )
                .Replace(
                    legendaries50MinusJson,
                    GetSerializedList(legendariesAmendedJsonArray, (x => !x.WinRateIsEqualOrAbove50()))
                )
                .Replace("'", "\"");
        }

        private List<ItemEntry> GetLegendariesWithAmendedTearItemPositioning(
            List<TableEntry<Item>> itemData,
            List<ItemEntry> legendariesJsonArray,
            Item tear
        )
        {
            if (tear == null) return legendariesJsonArray;

            List<ItemEntry> legendariesAmendedJsonArray = new();
            Dictionary<int, Item> secondFormIdAndFirstForm = GetSecondFormIdsAndFirstFormsOfTearItems(tear);

            foreach (ItemEntry item in legendariesJsonArray)
            {
                if (tear.BuildsInto.Contains(item.Id.ToString())) continue;

                legendariesAmendedJsonArray.Add(item);
                if (!secondFormIdAndFirstForm.ContainsKey(item.Id)) continue;

                Item firstForm = secondFormIdAndFirstForm[item.Id];
                legendariesAmendedJsonArray.Add(
                    new ItemEntry(
                        firstForm.Id,
                        itemData.First(i => i.GetEntry().Id == item.Id).GetWinLossData().GetWinRate()
                    )
                );
            }
            return legendariesAmendedJsonArray;
        }

        private Dictionary<int, Item> GetSecondFormIdsAndFirstFormsOfTearItems(Item tear)
        {
            Dictionary<int, Item> secondFormIdAndFirstForm = new();
            foreach (string firstFormId in tear.BuildsInto)
            {
                int firstFormIdInt = int.Parse(firstFormId);
                Item firstFormItem = Repository.GetItemById(firstFormIdInt);
                string secondFormItemName = firstFormItem.GetSecondFormNameForTearItem();
                if (secondFormItemName.IsNullOrEmpty()) continue;

                var secondFormItems = Repository.GetItemsByName(secondFormItemName);
                foreach (var secondFormItem in secondFormItems)
                {
                    secondFormIdAndFirstForm.Add(secondFormItem.Id, firstFormItem);
                }
            }
            return secondFormIdAndFirstForm;
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
                return new {id = Id.ToString(), count = 1};
            }
        }
    }
}