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
            repo.Setup(x => x.GetChampion(1)).Returns(new Champion() { Name = "Aatrox", Difficulty = 4, Tags = new List<string>() { "Fighter", "Tank" } });

            DataTableCreator creator = new(repo.Object);
            Dictionary<int, WinLossData> championData = new();
            championData.Add(1, new WinLossData(6, 4));
            DataTable championTable = creator.GetChampionTable(championData);
            Assert.IsTrue(championTable.TableName.Equals("Champions"));
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
        
        [TestMethod]
        public void CreateItemTable_ExpectedResults()
        {
            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetItem(1)).Returns(new Item() { Name = "name", Description = "<li>desc</li>", Gold = 250, Plaintext = "plainText", Tags = null });

            DataTableCreator creator = new(repo.Object);
            Dictionary<int, WinLossData> itemData = new();
            itemData.Add(1, new WinLossData(6, 4));
            DataTable table = creator.GetItemTable(itemData);
            Assert.IsTrue(table.TableName.Equals("Items"));
            Assert.IsTrue(table.Rows.Count == 1);
            Assert.IsTrue(table.Rows[0].ItemArray.Length == 10);
            Assert.IsTrue(table.Rows[0].ItemArray[0].Equals("name"));
            Assert.IsTrue(table.Rows[0].ItemArray[1].Equals(6));
            Assert.IsTrue(table.Rows[0].ItemArray[2].Equals(4));
            Assert.IsTrue(table.Rows[0].ItemArray[3].Equals(10));
            Assert.IsTrue(table.Rows[0].ItemArray[4].Equals(60.0));
            Assert.IsTrue(table.Rows[0].ItemArray[5].Equals(250));
            Assert.IsTrue(table.Rows[0].ItemArray[6].Equals(false));
            Assert.IsTrue(table.Rows[0].ItemArray[7].Equals(""));
            Assert.IsTrue(table.Rows[0].ItemArray[8].Equals("plainText"));
            Assert.IsTrue(table.Rows[0].ItemArray[9].Equals("desc"));
        }
        
        [TestMethod]
        public void CreateRuneTable_ExpectedResults()
        {
            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetRune(1)).Returns(new Rune() { Name = "name", Tree = "tree", LongDescription = "<l>longdesc<l>", IsKeystone = true});

            DataTableCreator creator = new(repo.Object);
            Dictionary<int, WinLossData> data = new();
            data.Add(1, new WinLossData(6, 4));
            DataTable table = creator.GetRuneData(data);
            Assert.IsTrue(table.TableName.Equals("Runes"));
            Assert.IsTrue(table.Rows.Count == 1);
            Assert.IsTrue(table.Rows[0].ItemArray.Length == 8);
            Assert.IsTrue(table.Rows[0].ItemArray[0].Equals("name"));
            Assert.IsTrue(table.Rows[0].ItemArray[1].Equals(6));
            Assert.IsTrue(table.Rows[0].ItemArray[2].Equals(4));
            Assert.IsTrue(table.Rows[0].ItemArray[3].Equals(10));
            Assert.IsTrue(table.Rows[0].ItemArray[4].Equals(60.0));
            Assert.IsTrue(table.Rows[0].ItemArray[5].Equals("tree"));
            Assert.IsTrue(table.Rows[0].ItemArray[6].Equals("longdesc"));
            Assert.IsTrue(table.Rows[0].ItemArray[7].Equals(true));
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
