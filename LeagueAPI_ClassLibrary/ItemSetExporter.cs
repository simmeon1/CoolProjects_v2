using Common_ClassLibrary;
using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class ItemSetExporter
    {
        const string jsonTitle = "jsonTitle";
        const string guardianJson = "guardianJson";
        const string bootsJson = "bootsJson";
        const string mythics50PlusJson = "mythics50PlusJson";
        const string mythics50MinusJson = "mythics50MinusJson";
        const string legendaries50PlusJson = "legendaries50PlusJson";
        const string legendaries50MinusJson = "legendaries50MinusJson";
        public IDDragonRepository Repository { get; set; }
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

        public string GetItemSet(Dictionary<int, WinLossData> itemData)
        {
            List<object> guardianJsonArray = new();
            List<object> bootsJsonArray = new();
            List<object> mythics50PlusJsonArray = new();
            List<object> mythics50MinusJsonArray = new();
            List<object> legendaries50PlusJsonArray = new();
            List<object> legendaries50MinusJsonArray = new();

            foreach (KeyValuePair<int, WinLossData> entry in itemData)
            {
                int id = entry.Key;
                double winRate = entry.Value.GetWinRate();
                Item item = Repository.GetItem(id);
                if (item.IsGuardian()) AddItemToList(id, guardianJsonArray);
                else if (item.IsBoots()) AddItemToList(id, bootsJsonArray);
                else if (item.IsMythic() && winRate >= 50) AddItemToList(id, mythics50PlusJsonArray);
                else if (item.IsMythic() && winRate < 50) AddItemToList(id, mythics50MinusJsonArray);
                else if (item.IsLegendary() && winRate >= 50) AddItemToList(id, legendaries50PlusJsonArray);
                else if (item.IsLegendary() && winRate < 50) AddItemToList(id, legendaries50MinusJsonArray);
            }
            return BaseJson
                .Replace(guardianJson, guardianJsonArray.SerializeObject())
                .Replace(bootsJson, bootsJsonArray.SerializeObject())
                .Replace(mythics50PlusJson, mythics50PlusJsonArray.SerializeObject())
                .Replace(mythics50MinusJson, mythics50MinusJsonArray.SerializeObject())
                .Replace(legendaries50PlusJson, legendaries50PlusJsonArray.SerializeObject())
                .Replace(legendaries50MinusJson, legendaries50MinusJsonArray.SerializeObject())
                .Replace("'", "\"");
        }

        private void AddItemToList(int id, List<object> list)
        {
            list.Add(new { id = id.ToString(), count = 1 });
        }
    }
}
