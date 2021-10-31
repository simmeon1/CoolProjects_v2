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
            DataTable championTable = creator.GetChampionTable(GetDefaultTestData());
            Assert.IsTrue(championTable.TableName.Equals("Champions"));
            Assert.IsTrue(championTable.Rows.Count == 1);
            Assert.IsTrue(championTable.Rows[0].ItemArray.Length == 12);
            Assert.IsTrue(championTable.Rows[0].ItemArray[0].Equals("Aatrox"));
            TestDefaultColumns(championTable);
            Assert.IsTrue(championTable.Rows[0].ItemArray[10].Equals("Fighter, Tank"));
            Assert.IsTrue(championTable.Rows[0].ItemArray[11].Equals(4));
        }

        private static void TestDefaultColumns(DataTable table)
        {
            Assert.IsTrue(table.Rows[0].ItemArray[1].Equals(6));
            Assert.IsTrue(table.Rows[0].ItemArray[2].Equals(4));
            Assert.IsTrue(table.Rows[0].ItemArray[3].Equals(10));
            Assert.IsTrue(table.Rows[0].ItemArray[4].Equals(60.0));
            Assert.IsTrue(table.Rows[0].ItemArray[5].Equals(2));
            Assert.IsTrue(table.Rows[0].ItemArray[6].Equals(2));
            Assert.IsTrue(table.Rows[0].ItemArray[7].Equals(4));
            Assert.IsTrue(table.Rows[0].ItemArray[8].Equals(50.0));
            Assert.IsTrue(table.Rows[0].ItemArray[9].Equals(-10.0));
        }

        private static Dictionary<int, Dictionary<int, WinLossData>> GetDefaultTestData()
        {
            Dictionary<int, WinLossData> data_0 = new();
            data_0.Add(1, new WinLossData(6, 4));
            Dictionary<int, WinLossData> championData_20 = new();
            championData_20.Add(1, new WinLossData(2, 2));

            Dictionary<int, Dictionary<int, WinLossData>> data = new();
            data.Add(0, data_0);
            data.Add(20, championData_20);
            return data;
        }

        [TestMethod]
        public void CreateItemTable_ExpectedResults()
        {
            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetItem(1)).Returns(new Item() { Name = "name", Description = "<li>desc</li>", Gold = 250, Plaintext = "plainText", Tags = null });
            DataTableCreator creator = new(repo.Object);
            DataTable table = creator.GetItemTable(GetDefaultTestData());
            Assert.IsTrue(table.TableName.Equals("Items"));
            Assert.IsTrue(table.Rows.Count == 1);
            Assert.IsTrue(table.Rows[0].ItemArray.Length == 17);
            Assert.IsTrue(table.Rows[0].ItemArray[0].Equals("name"));
            TestDefaultColumns(table);
            Assert.IsTrue(table.Rows[0].ItemArray[10].Equals(250));
            Assert.IsTrue(table.Rows[0].ItemArray[11].Equals(false));
            Assert.IsTrue(table.Rows[0].ItemArray[12].Equals(false));
            Assert.IsTrue(table.Rows[0].ItemArray[13].Equals(false));
            Assert.IsTrue(table.Rows[0].ItemArray[14].Equals(""));
            Assert.IsTrue(table.Rows[0].ItemArray[15].Equals("plainText"));
            Assert.IsTrue(table.Rows[0].ItemArray[16].Equals("desc"));
        }

        [TestMethod]
        public void CreateRuneTable_ExpectedResults()
        {
            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetRune(1)).Returns(new Rune() { Name = "name", Tree = "tree", LongDescription = "<l>longdesc<l>", Slot = 1 });
            DataTableCreator creator = new(repo.Object);
            DataTable table = creator.GetRuneTable(GetDefaultTestData());
            Assert.IsTrue(table.TableName.Equals("Runes"));
            Assert.IsTrue(table.Rows.Count == 1);
            Assert.IsTrue(table.Rows[0].ItemArray.Length == 13);
            Assert.IsTrue(table.Rows[0].ItemArray[0].Equals("name"));
            TestDefaultColumns(table);
            Assert.IsTrue(table.Rows[0].ItemArray[10].Equals("tree"));
            Assert.IsTrue(table.Rows[0].ItemArray[11].Equals(1));
            Assert.IsTrue(table.Rows[0].ItemArray[12].Equals("longdesc"));
        }

        [TestMethod]
        public void CreateStatPerkTable_ExpectedResults_DataWithMinutes_AdditionalDataAvailable()
        {
            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetStatPerk(1)).Returns("hi");
            DataTableCreator creator = new(repo.Object);
            DataTable table = creator.GetStatPerkTable(GetDefaultTestData());
            Assert.IsTrue(table.TableName.Equals("Stat Perks"));
            Assert.IsTrue(table.Rows.Count == 1);
            Assert.IsTrue(table.Rows[0].ItemArray.Length == 10);
            Assert.IsTrue(table.Rows[0].ItemArray[0].Equals("hi"));
            TestDefaultColumns(table);
        }

        [TestMethod]
        public void CreateStatPerkTable_ExpectedResults_DataWithMinutes_AdditionalDataNotAvailable()
        {
            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetStatPerk(1)).Returns("hi");

            DataTableCreator creator = new(repo.Object);

            Dictionary<int, WinLossData> data_0 = new();
            data_0.Add(1, new WinLossData(6, 4));
            
            Dictionary<int, WinLossData> data_20 = new();
            data_20.Add(2, new WinLossData(6, 4));

            Dictionary<int, Dictionary<int, WinLossData>> allData = new();
            allData.Add(0, data_0);
            allData.Add(20, data_20);

            DataTable table = creator.GetStatPerkTable(allData);
            Assert.IsTrue(table.TableName.Equals("Stat Perks"));
            Assert.IsTrue(table.Rows.Count == 1);
            Assert.IsTrue(table.Rows[0].ItemArray.Length == 10);
            Assert.IsTrue(table.Rows[0].ItemArray[0].Equals("hi"));
            Assert.IsTrue(table.Rows[0].ItemArray[1].Equals(6));
            Assert.IsTrue(table.Rows[0].ItemArray[2].Equals(4));
            Assert.IsTrue(table.Rows[0].ItemArray[3].Equals(10));
            Assert.IsTrue(table.Rows[0].ItemArray[4].Equals(60.0));
            Assert.IsTrue(table.Rows[0].ItemArray[5].Equals(0));
            Assert.IsTrue(table.Rows[0].ItemArray[6].Equals(0));
            Assert.IsTrue(table.Rows[0].ItemArray[7].Equals(0));
            Assert.IsTrue(table.Rows[0].ItemArray[8].Equals(0.0));
            Assert.IsTrue(table.Rows[0].ItemArray[9].Equals(0.0));
        }

        [TestMethod]
        public void CreateStatPerkTable_ExpectedResults_OnlyAllData()
        {
            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetStatPerk(1)).Returns("hi");

            DataTableCreator creator = new(repo.Object);

            Dictionary<int, WinLossData> data = new();
            data.Add(1, new WinLossData(6, 4));

            Dictionary<int, Dictionary<int, WinLossData>> allData = new();
            allData.Add(0, data);

            DataTable table = creator.GetStatPerkTable(allData);
            Assert.IsTrue(table.TableName.Equals("Stat Perks"));
            Assert.IsTrue(table.Rows.Count == 1);
            Assert.IsTrue(table.Rows[0].ItemArray.Length == 5);
            Assert.IsTrue(table.Rows[0].ItemArray[0].Equals("hi"));
            Assert.IsTrue(table.Rows[0].ItemArray[1].Equals(6));
            Assert.IsTrue(table.Rows[0].ItemArray[2].Equals(4));
            Assert.IsTrue(table.Rows[0].ItemArray[3].Equals(10));
            Assert.IsTrue(table.Rows[0].ItemArray[4].Equals(60.0));
        }
        
        
        [TestMethod]
        public void CreateSpellTable_ExpectedResults()
        {
            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetSpell(1)).Returns(new Spell() { Name = "hi", Cooldown = 200, Description = "desc" });
            DataTableCreator creator = new(repo.Object);
            DataTable table = creator.GetSpellTable(GetDefaultTestData());
            Assert.IsTrue(table.TableName.Equals("Spells"));
            Assert.IsTrue(table.Rows.Count == 1);
            Assert.IsTrue(table.Rows[0].ItemArray.Length == 12);
            Assert.IsTrue(table.Rows[0].ItemArray[0].Equals("hi"));
            TestDefaultColumns(table);
            Assert.IsTrue(table.Rows[0].ItemArray[10].Equals(200));
            Assert.IsTrue(table.Rows[0].ItemArray[11].Equals("desc"));
        }
    }
}
