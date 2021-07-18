using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class ItemSetExporter_UnitTests
    {
        [TestMethod]
        public void test()
        {
            Item item1 = new() { Name = "Guardian" };
            Item item2 = new() { Tags = new List<string>() { "Boots" } };
            Item item3 = new() { Description = "rarityMythic" };
            Item item4 = new() { Description = "rarityMythic" };
            Item item5 = new() { Description = "rarityLegendary" };
            Item item6 = new() { Description = "rarityLegendary" };
            Item item7 = new() { };

            Dictionary<int, WinLossData> itemWInLossData = new();
            itemWInLossData.Add(3184, new WinLossData());
            itemWInLossData.Add(3177, new WinLossData());
            itemWInLossData.Add(2051, new WinLossData(1, 0));
            itemWInLossData.Add(3112, new WinLossData(0, 1));
            itemWInLossData.Add(3158, new WinLossData(1, 1));
            itemWInLossData.Add(3009, new WinLossData(0, 1));
            itemWInLossData.Add(1, new WinLossData());

            Mock<IDDragonRepository> repo = new();
            repo.Setup(x => x.GetItem(3184)).Returns(item1);
            repo.Setup(x => x.GetItem(3177)).Returns(item2);
            repo.Setup(x => x.GetItem(2051)).Returns(item3);
            repo.Setup(x => x.GetItem(3112)).Returns(item4);
            repo.Setup(x => x.GetItem(3158)).Returns(item5);
            repo.Setup(x => x.GetItem(3009)).Returns(item6);
            repo.Setup(x => x.GetItem(1)).Returns(item7);

            ItemSetExporter exporter = new(repo.Object);
            string result = exporter.GetItemSet(itemWInLossData);
            string target = @"
        {
          ""title"": ""jsonTitle"",
          ""associatedMaps"": [
            11,
            12
          ],
          ""associatedChampions"": [],
          ""blocks"": [
            {
              ""items"": [{""id"":""3184"",""count"":1}],
              ""type"": ""Guardian""
            },
            {
              ""items"": [{""id"":""3177"",""count"":1}],
              ""type"": ""Boots""
            },
            {
              ""items"": [{""id"":""2051"",""count"":1}],
              ""type"": ""Mythics 50+ WR""
            },
            {
              ""items"": [{""id"":""3112"",""count"":1}],
              ""type"": ""Mythics 50- WR""
            },
            {
              ""items"": [{""id"":""3158"",""count"":1}],
              ""type"": ""Legendaries 50+ WR""
            },
            {
              ""items"": [{""id"":""3009"",""count"":1}],
              ""type"": ""Legendaries 50- WR""
            }
          ]
        }";
            Assert.IsTrue(result.Equals(target));
        }
    }
}