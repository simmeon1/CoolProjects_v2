using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class DataCollectorResults
    {
        private List<TableEntryAndWinLossData<ITableEntry>> entries;
        public DataCollectorResults(List<TableEntryAndWinLossData<ITableEntry>> entries)
        {
            this.entries = entries;
        }

        public List<TableEntryAndWinLossData<ITableEntry>> GetEntries()
        {
            return entries;
        }
        
        public List<TableEntryAndWinLossData<Item>> GetItemData()
        {
            List<TableEntryAndWinLossData<Item>> items = new();
            foreach (TableEntryAndWinLossData<ITableEntry> x in entries)
            {
                if (x.GetEntry() is Item i) items.Add(new TableEntryAndWinLossData<Item>(i, x.GetWinLossData()));
            }
            return items;
        }
    }
}