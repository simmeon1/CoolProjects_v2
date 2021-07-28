using System.Collections.Generic;
using System.Data;

namespace LeagueAPI_ClassLibrary
{
    public interface IExcelPrinter
    {
        void PrintTablesToWorksheet(List<DataTable> dataTables, string fileName);
    }
}