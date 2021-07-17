using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class DataTableCreator_UnitTests
    {
        [TestMethod]
        public void CreateChampionTable_ExpectedResults()
        {
            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetChampion(1)).Returns(JObject.Parse(@"{'name':'Aatrox','info':{'difficulty':4},'tags':['Fighter','Tank']}"));

            DataTableCreator creator = new(repo.Object);
            Dictionary<int, WinLossData> championData = new();
            championData.Add(1, new WinLossData(6, 4));
            DataTable championTable = creator.GetChampionTable(championData);
            Assert.IsTrue(championTable.Rows.Count == 1);
            Assert.IsTrue(championTable.Rows[0].ItemArray.Length == 7);
            Assert.IsTrue(championTable.Rows[0].ItemArray[0].Equals("Aatrox"));
            Assert.IsTrue(championTable.Rows[0].ItemArray[1].Equals(6));
            Assert.IsTrue(championTable.Rows[0].ItemArray[2].Equals(4));
            Assert.IsTrue(championTable.Rows[0].ItemArray[3].Equals(10));
            Assert.IsTrue(championTable.Rows[0].ItemArray[4].Equals(60.0));
            Assert.IsTrue(championTable.Rows[0].ItemArray[5].Equals("Fighter, Tank"));
            Assert.IsTrue(championTable.Rows[0].ItemArray[6].Equals(4));
        }

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //using ExcelPackage package = new(new FileInfo("MyWorkbook10.xlsx"));
            //ExcelWorksheet ws = package.Workbook.Worksheets.Add(championTable.TableName);
            //ws.Cells["A1"].LoadFromDataTable(championTable, true);
            //ws.Cells[ws.Dimension.Address].AutoFilter = true;
            //ws.View.FreezePanes(2, 2);
            //ws.Cells.AutoFitColumns();
            //package.Save();
    }
}
