using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Common_ClassLibrary
{
    public class DataTableCreator
    {
        public List<DataTable> GetTables(IEnumerable<ITableEntry> entries)
        {
            Dictionary<string, DataTable> tables = new();
            foreach (ITableEntry entry in entries)
            {
                string category = entry.GetCategory();
                List<KeyValuePair<string, object>> entryProps = entry.GetProperties();
                if (!tables.ContainsKey(category))
                {
                    DataTable newTable = new(category);
                    List<DataColumn> dataColumns = new();
                    foreach (KeyValuePair<string, object> entryProp in entryProps)
                    {
                        dataColumns.Add(new DataColumn(entryProp.Key, entryProp.Value.GetType()));
                    }
                    newTable.Columns.AddRange(dataColumns.ToArray());
                    tables.Add(category, newTable);
                }

                DataTable table = tables[category];
                DataRow row = table.NewRow();
                int columnIndex = 0;
                foreach (KeyValuePair<string, object> entryProp in entryProps)
                {
                    row[columnIndex++] = entryProp.Value;
                }
                table.Rows.Add(row);
            }
            return tables.Values.ToList();
        }
    }
}