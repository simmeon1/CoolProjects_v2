using System;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Common_ClassLibrary
{
    public class ExcelPrinter : IExcelPrinter
    {
        public void PrintTablesToWorksheet(List<DataTable> dataTables, string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            var dbFile = Path.Combine(directory, Path.GetFileNameWithoutExtension(fileName) + ".sqlite3");
            CreateDb(dataTables, dbFile);
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage package = new(new FileInfo(Path.Combine(fileName)));
            foreach (DataTable table in dataTables)
            {
                var csvFile = Path.Combine(directory, Path.GetFileNameWithoutExtension(fileName) + $"_{table.TableName}.csv");
                WriteToCsvFile(table, csvFile);
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

        private static void CreateDb(List<DataTable> dataTables, string fileName)
        {
            using var connection = new SqliteConnection($"Data Source={fileName}");
            connection.Open();

            foreach (var table in dataTables)
            {
                var command = connection.CreateCommand();
                var createTableColumns = new List<string>();
                foreach (DataColumn column in table.Columns)
                {
                    var columnNameQuoted = "\"" + column.ColumnName + "\"";
                    var type = "text";
                    if (column.DataType == typeof(int) || column.DataType == typeof(bool))
                    {
                        type = "integer";
                        if (column.DataType == typeof(bool))
                        {
                            type += $" CHECK ({columnNameQuoted} IN (0, 1))";
                        }
                    }
                    else if (column.DataType == typeof(decimal) || column.DataType == typeof(double))
                    {
                        type = "real";
                    }

                    createTableColumns.Add(columnNameQuoted + " " + type);
                }
                
                var counter = 0;
                var values = new List<string>();
                var initial = counter;
                foreach (DataRow dr in table.Rows)
                {
                    for (int i = 0; i < dr.ItemArray.Length; i++)
                    {
                        if (i == 0)
                        {
                            initial = counter;
                        }

                        object column = dr.ItemArray[i];
                        command.Parameters.AddWithValue($"${counter}", column);
                        counter++;
                    }
                    
                    var valueRow = new List<string>();
                    for (int i = initial; i < counter; i++)
                    {
                        valueRow.Add($"${i}");
                    }
                    var value = "(" + valueRow.ConcatenateListOfStringsToCommaString() + ")";
                    values.Add(value);
                }

                var tableNameQuoted = "\"" + table.TableName + "\"";
                command.CommandText =
                    $@"
                        CREATE TABLE {tableNameQuoted} (
                            {createTableColumns.ConcatenateListOfStringsToCommaString()}
                        ) STRICT;

                        INSERT INTO {tableNameQuoted}
                        VALUES {values.ConcatenateListOfStringsToCommaString()};
                    ";
                
                // string query = command.CommandText;
                // foreach (SqliteParameter p in command.Parameters)
                // {
                //     query = query.Replace(p.ParameterName, p.Value.ToString());
                // }
                command.ExecuteNonQuery();
            }
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
            File.WriteAllText(filePath, fileContent.ToString());
        }
    }
}