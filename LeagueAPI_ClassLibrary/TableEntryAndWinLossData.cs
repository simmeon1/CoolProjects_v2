using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class TableEntryAndWinLossData<T> where T: ITableEntry
    {
        private readonly T entry;
        private readonly WinLossData winLossData;

        public TableEntryAndWinLossData(T entry, WinLossData winLossData)
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
