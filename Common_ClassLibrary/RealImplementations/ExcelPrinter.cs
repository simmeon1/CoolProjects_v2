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

            foreach (DataTable table in dataTables)
            {
                var rowArray = new DataRow[table.Rows.Count];
                table.Rows.CopyTo(rowArray, 0);
                var rowList = rowArray.ToList();

                var maxTake = 999 / table.Columns.Count;
                var rowSets = new List<DataRow[]>();
                while (rowList.Any())
                {
                    var dataRows = rowList.Take(maxTake).ToArray();
                    rowSets.Add(dataRows);
                    rowList.RemoveRange(0, dataRows.Length);
                }

                foreach (DataRow[] rowSet in rowSets)
                {
                    var values = new List<string>();
                    var command = connection.CreateCommand();
                    foreach (DataRow dr in rowSet)
                    {
                        var valueRow = new List<string>();
                        foreach (object column in dr.ItemArray)
                        {
                            var paramCount = command.Parameters.Count;
                            command.Parameters.AddWithValue($"${paramCount}", column);
                            valueRow.Add($"${paramCount}");
                        }

                        var value = "(" + valueRow.ConcatenateListOfStringsToCommaString() + ")";
                        values.Add(value);
                    }

                    var tableNameQuoted = GetQuotedString(table.TableName);
                    if (rowSet == rowSets[0])
                    {
                        var createTableColumns = new List<string>();
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            DataColumn column = table.Columns[i];
                            var columnNameQuoted = GetQuotedString(column.ColumnName);
                            // This will break if a value is null along with DataTableCreator
                            var type = command.Parameters[i].SqliteType.ToString();
                            if (column.DataType == typeof(bool))
                            {
                                type += $" CHECK ({columnNameQuoted} IN (0, 1))";
                            }
                            createTableColumns.Add($"{columnNameQuoted} {type}");
                        }

                        var createTableCommand = connection.CreateCommand();
                        createTableCommand.CommandText =
                            $@"
                                CREATE TABLE {tableNameQuoted} (
                                 {createTableColumns.ConcatenateListOfStringsToCommaString()}
                                ) STRICT;
                             ";
                            createTableCommand.ExecuteNonQuery();
                    }

                    command.CommandText = $@"
                        INSERT INTO {tableNameQuoted}
                        VALUES {values.ConcatenateListOfStringsToCommaString()};
                    ";
                    command.ExecuteNonQuery();
                }
            }
        }

        private static string GetQuotedString(string str) => "\"" + str + "\"";

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
                        fileContent.Append($"{GetQuotedString(c)},");
                    }
                    else if (column is DateTime dt)
                    {
                        fileContent.Append(
                            $"{GetQuotedString(dt.ToString("o", System.Globalization.CultureInfo.InvariantCulture))},"
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