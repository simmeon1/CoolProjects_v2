using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Common_ClassLibrary
{
    public class ExcelPrinter: IExcelPrinter
    {
        public void PrintTablesToWorksheet(List<DataTable> dataTables, string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage package = new(new FileInfo(Path.Combine(fileName)));
            foreach (DataTable table in dataTables)
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(table.TableName);
                ws.Cells["A1"].LoadFromDataTable(table, true);
                ws.Cells[ws.Dimension.Address].AutoFilter = true;
                ws.View.FreezePanes(2, 2);
                ws.Cells.AutoFitColumns();
            }
            package.Save();
        }
    }
}
