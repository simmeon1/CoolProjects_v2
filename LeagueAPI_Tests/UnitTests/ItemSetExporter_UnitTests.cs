using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class ItemSetExporter_UnitTests
    {
        [TestMethod]
        public void ItemSetCreatedAsExpected()
        {
            Item item1 = new() { Name = "Guardian Hammer" };
            Item item2 = new() { Tags = new List<string>() { "Boots" } };
            Item item3 = new() { Description = "rarityMythic" };
            Item item4 = new() { Description = "rarityMythic" };
            Item item5 = new() { BuildsInto = new(), Gold = 2500 };
            Item item6 = new() { BuildsInto = new(), Gold = 2500 };
            Item item7 = new() { };
            Item item8 = null;
            Item item9 = new() { Name = "Doran Blade" };
            Item item10 = new() { BuildsInto = new List<string>() { "100" }, Gold = 100 };
            Item item11 = new() { BuildsInto = new List<string>() { "100" }, Gold = 3000 };
            Item item12 = new() { Name = "Tear of the Goddess", BuildsInto = new List<string>() { "100" } };

            Item item100 = new() { Id = 100, Name = "Manamune", Description = "360 mana.*<raritylegendary>Muramana</raritylegendary>", Gold = 2500 };
            Item muramana = new() { Id = 1000, Name = "Muramana", Gold = 3000 };

            Dictionary<int, WinLossData> itemWinLossData = new();
            itemWinLossData.Add(1, new WinLossData());
            itemWinLossData.Add(2, new WinLossData());
            itemWinLossData.Add(3, new WinLossData(1, 0));
            itemWinLossData.Add(4, new WinLossData(0, 1));
            itemWinLossData.Add(5, new WinLossData(1, 1));
            itemWinLossData.Add(6, new WinLossData(0, 1));
            itemWinLossData.Add(7, new WinLossData());
            itemWinLossData.Add(8, new WinLossData());
            itemWinLossData.Add(9, new WinLossData());
            itemWinLossData.Add(10, new WinLossData());
            itemWinLossData.Add(11, new WinLossData());
            itemWinLossData.Add(12, new WinLossData());
            itemWinLossData.Add(100, new WinLossData(0, 1));
            itemWinLossData.Add(1000, new WinLossData(1, 0));

            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetItem(1)).Returns(item1);
            repo.Setup(x => x.GetItem(2)).Returns(item2);
            repo.Setup(x => x.GetItem(3)).Returns(item3);
            repo.Setup(x => x.GetItem(4)).Returns(item4);
            repo.Setup(x => x.GetItem(5)).Returns(item5);
            repo.Setup(x => x.GetItem(6)).Returns(item6);
            repo.Setup(x => x.GetItem(7)).Returns(item7);
            repo.Setup(x => x.GetItem(8)).Returns(item8);
            repo.Setup(x => x.GetItem(9)).Returns(item9);
            repo.Setup(x => x.GetItem(10)).Returns(item10);
            repo.Setup(x => x.GetItem(11)).Returns(item11);
            repo.Setup(x => x.GetItem(12)).Returns(item12);
            repo.Setup(x => x.GetItem(100)).Returns(item100);
            repo.Setup(x => x.GetItem(1000)).Returns(muramana);
            repo.Setup(x => x.GetItem("Muramana")).Returns(muramana);

            ItemSetExporter exporter = new(repo.Object);
            string result = exporter.GetItemSet(itemWinLossData, "test");
            string target = @"
        {
          ""title"": ""test"",
          ""associatedMaps"": [
            11,
            12
          ],
          ""associatedChampions"": [],
          ""blocks"": [
            {
              ""items"": [{""id"":""1"",""count"":1}],
              ""type"": ""Guardian""
            },
            {
              ""items"": [{""id"":""9"",""count"":1}],
              ""type"": ""Doran""
            },
            {
              ""items"": [{""id"":""2"",""count"":1}],
              ""type"": ""Boots""
            },
            {
              ""items"": [{""id"":""3"",""count"":1}],
              ""type"": ""Mythics 50+ WR""
            },
            {
              ""items"": [{""id"":""4"",""count"":1}],
              ""type"": ""Mythics 50- WR""
            },
            {
              ""items"": [{""id"":""1000"",""count"":1},{""id"":""100"",""count"":1},{""id"":""5"",""count"":1}],
              ""type"": ""Legendaries 50+ WR""
            },
            {
              ""items"": [{""id"":""6"",""count"":1}],
              ""type"": ""Legendaries 50- WR""
            }
          ]
        }";
            Assert.IsTrue(result.Equals(target));
        }
    }
}
