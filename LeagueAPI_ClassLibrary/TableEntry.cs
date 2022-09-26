using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class TableEntry<T>: ITableEntryWithWinLossData
    where T: ITableEntry
    {
        private readonly T entry;
        private readonly WinLossData winLossData;

        public TableEntry(T entry, WinLossData winLossData)
        {
            this.entry = entry;
            this.winLossData = winLossData;
        }

        public T GetEntry()
        {
            return entry;
        }
        
        public WinLossData GetWinLossData()
        {
            return winLossData;
        }

        public string GetIdentifier()
        {
            return $"{entry.GetIdentifier()} with Win Loss Data";
        }

        public string GetCategory()
        {
            return entry.GetCategory();
        }

        public List<KeyValuePair<string, object>> GetProperties()
        {
            List<KeyValuePair<string, object>> list = new();
            list.AddRange(entry.GetProperties());
            list.InsertRange(1, new List<KeyValuePair<string, object>>
            {
                new("Wins", winLossData.GetWins()),
                new("Losses", winLossData.GetLosses()),
                new("Total", winLossData.GetTotal()),
                new("Win rate", winLossData.GetWinRate())
            });
            return list;
        }
    }
}
