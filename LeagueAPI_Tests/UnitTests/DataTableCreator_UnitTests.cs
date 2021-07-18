using Common_ClassLibrary;
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
            Assert.IsTrue(table.Rows[0].ItemArray.Length == 13);
            Assert.IsTrue(table.Rows[0].ItemArray[0].Equals("name"));
            Assert.IsTrue(table.Rows[0].ItemArray[1].Equals(6));
            Assert.IsTrue(table.Rows[0].ItemArray[2].Equals(4));
            Assert.IsTrue(table.Rows[0].ItemArray[3].Equals(10));
            Assert.IsTrue(table.Rows[0].ItemArray[4].Equals(60.0));
            Assert.IsTrue(table.Rows[0].ItemArray[5].Equals(250));
            Assert.IsTrue(table.Rows[0].ItemArray[6].Equals(false));
            Assert.IsTrue(table.Rows[0].ItemArray[7].Equals(false));
            Assert.IsTrue(table.Rows[0].ItemArray[8].Equals(false));
            Assert.IsTrue(table.Rows[0].ItemArray[9].Equals(false));
            Assert.IsTrue(table.Rows[0].ItemArray[10].Equals(""));
            Assert.IsTrue(table.Rows[0].ItemArray[11].Equals("plainText"));
            Assert.IsTrue(table.Rows[0].ItemArray[12].Equals("desc"));
        }

        [TestMethod]
        public void CreateRuneTable_ExpectedResults()
        {
            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetRune(1)).Returns(new Rune() { Name = "name", Tree = "tree", LongDescription = "<l>longdesc<l>", Slot = 1 });

            DataTableCreator creator = new(repo.Object);
            Dictionary<int, WinLossData> data = new();
            data.Add(1, new WinLossData(6, 4));
            DataTable table = creator.GetRuneTable(data);
            Assert.IsTrue(table.TableName.Equals("Runes"));
            Assert.IsTrue(table.Rows.Count == 1);
            Assert.IsTrue(table.Rows[0].ItemArray.Length == 8);
            Assert.IsTrue(table.Rows[0].ItemArray[0].Equals("name"));
            Assert.IsTrue(table.Rows[0].ItemArray[1].Equals(6));
            Assert.IsTrue(table.Rows[0].ItemArray[2].Equals(4));
            Assert.IsTrue(table.Rows[0].ItemArray[3].Equals(10));
            Assert.IsTrue(table.Rows[0].ItemArray[4].Equals(60.0));
            Assert.IsTrue(table.Rows[0].ItemArray[5].Equals("tree"));
            Assert.IsTrue(table.Rows[0].ItemArray[6].Equals(1));
            Assert.IsTrue(table.Rows[0].ItemArray[7].Equals("longdesc"));
        }
        
        [TestMethod]
        public void CreateStatPerkTable_ExpectedResults()
        {
            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetStatPerk(1)).Returns("hi");

            DataTableCreator creator = new(repo.Object);
            Dictionary<int, WinLossData> data = new();
            data.Add(1, new WinLossData(6, 4));
            DataTable table = creator.GetStatPerkTable(data);
            Assert.IsTrue(table.TableName.Equals("Stat Perks"));
            Assert.IsTrue(table.Rows.Count == 1);
            Assert.IsTrue(table.Rows[0].ItemArray.Length == 5);
            Assert.IsTrue(table.Rows[0].ItemArray[0].Equals("hi"));
            Assert.IsTrue(table.Rows[0].ItemArray[1].Equals(6));
            Assert.IsTrue(table.Rows[0].ItemArray[2].Equals(4));
            Assert.IsTrue(table.Rows[0].ItemArray[3].Equals(10));
            Assert.IsTrue(table.Rows[0].ItemArray[4].Equals(60.0));
        }
    }
}
