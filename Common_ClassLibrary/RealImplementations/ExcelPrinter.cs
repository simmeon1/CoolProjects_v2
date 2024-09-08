using System;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Common_ClassLibrary
{
    public class ExcelPrinter : IExcelPrinter
    {
        public void PrintTablesToWorksheet(List<DataTable> dataTables, string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage package = new(new FileInfo(Path.Combine(fileName)));
            foreach (DataTable table in dataTables)
            {
                WriteToCsvFile(table, fileName);
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(table.TableName);
                ws.Cells["A1"].LoadFromDataTable(table, true);
                foreach (DataColumn column in table.Columns)
                {
                    if (column.DataType == typeof(DateTime))
                    {
                        ws.Column(column.Ordinal + 1).Style.Numberformat.Format = "dd/mm/yyyy hh:mm";
                    }
                }

                ws.Cells[ws.Dimension.Address].AutoFilter = true;
                ws.View.FreezePanes(2, 2);
                ws.Cells.AutoFitColumns();
            }

            package.Save();
        }

        private static void WriteToCsvFile(DataTable dataTable, string filePath)
        {
            StringBuilder fileContent = new();

            foreach (var col in dataTable.Columns)
            {
                fileContent.Append(col + ",");
            }

            fileContent.Replace(",", Environment.NewLine, fileContent.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {
                // Types are to fit sqlite
                foreach (var column in dr.ItemArray)
                {
                    if (column is string c)
                    {
                        fileContent.Append("\"" + c + "\",");
                    }
                    else if (column is DateTime dt)
                    {
                        fileContent.Append(
                            "\"" + dt.ToString("o", System.Globalization.CultureInfo.InvariantCulture) + "\","
                        );
                    }
                    else if (column is bool b)
                    {
                        fileContent.Append(
                            (b ? 1 : 0) + ","
                        );
                    }
                    else
                    {
                        fileContent.Append(column + ",");
                    }
                }
                fileContent.Replace(",", Environment.NewLine, fileContent.Length - 1, 1);
            }
            File.WriteAllText(filePath.Replace(".xlsx", $"_{dataTable.TableName}.csv"), fileContent.ToString());
        }
    }
}