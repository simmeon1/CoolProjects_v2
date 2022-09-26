using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class DataCollectorResults
    {
        private readonly List<ITableEntry> entries;
        public DataCollectorResults(List<ITableEntry> entries)
        {
            this.entries = entries;
        }

        public List<ITableEntry> GetEntries()
        {
            return entries;
        }
        
        public List<TableEntry<Item>> GetItemData()
        {
            List<TableEntry<Item>> items = new();
            foreach (ITableEntry entry in entries)
            {
                if (entry is TableEntry<Item> i) items.Add(i);
            }
            return items;
        }
    }
}