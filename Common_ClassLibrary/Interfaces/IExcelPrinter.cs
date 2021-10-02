using System.Collections.Generic;
using System.Data;

namespace Common_ClassLibrary
{
    public interface IExcelPrinter
    {
        void PrintTablesToWorksheet(List<DataTable> dataTables, string fileName);
    }
}