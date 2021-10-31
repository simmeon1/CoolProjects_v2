using Common_ClassLibrary;
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
            List<object> guardianJsonArray = new();
            List<object> bootsJsonArray = new();
            List<object> doranJsonArray = new();
            List<object> mythics50PlusJsonArray = new();
            List<object> mythics50MinusJsonArray = new();
            List<object> legendaries50PlusJsonArray = new();
            List<object> legendaries50MinusJsonArray = new();

            List<KeyValuePair<int, WinLossData>> sortedList = itemData.OrderByDescending(x => x.Value.GetWinRate()).ToList();
            foreach (KeyValuePair<int, WinLossData> entry in sortedList)
            {
                int id = entry.Key;
                double winRate = entry.Value.GetWinRate();
                Item item = Repository.GetItem(id);
                if (item == null) continue;
                if (item.IsGuardian()) AddItemToList(id, guardianJsonArray);
                else if (item.IsBoots()) AddItemToList(id, bootsJsonArray);
                else if (item.IsDoran()) AddItemToList(id, doranJsonArray);
                else if (item.IsMythic() && winRate >= 50) AddItemToList(id, mythics50PlusJsonArray);
                else if (item.IsMythic()) AddItemToList(id, mythics50MinusJsonArray);
                else if (item.IsFinished && item.IsMoreThan2000G() && winRate >= 50) AddItemToList(id, legendaries50PlusJsonArray);
                else if (item.IsFinished && item.IsMoreThan2000G()) AddItemToList(id, legendaries50MinusJsonArray);
            }
            return BaseJson
                .Replace(jsonTitle, itemSetName)
                .Replace(guardianJson, guardianJsonArray.SerializeObject())
                .Replace(bootsJson, bootsJsonArray.SerializeObject())
                .Replace(doranJson, doranJsonArray.SerializeObject())
                .Replace(mythics50PlusJson, mythics50PlusJsonArray.SerializeObject())
                .Replace(mythics50MinusJson, mythics50MinusJsonArray.SerializeObject())
                .Replace(legendaries50PlusJson, legendaries50PlusJsonArray.SerializeObject())
                .Replace(legendaries50MinusJson, legendaries50MinusJsonArray.SerializeObject())
                .Replace("'", "\"");
        }

        private static void AddItemToList(int id, List<object> list)
        {
            list.Add(new { id = id.ToString(), count = 1 });
        }
    }
}
