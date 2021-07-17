using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class DataCollector_UnitTests
    {
        private LeagueMatch Match { get; set; }
        private Participant P1 { get; set; }
        private Participant P2 { get; set; }
        private DataCollectorResults Results { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            P1 = new();
            P2 = new();
            P1.championId = 1000;
            P1.item0 = 1;
            P1.item1 = 2;
            P1.item2 = 3;
            P1.item3 = 4;
            P1.item4 = 5;
            P1.item5 = 6;
            P1.item6 = 7;
            P1.perk1_1 = 9;
            P1.perk1_2 = 10;
            P1.perk1_3 = 11;
            P1.perk1_4 = 12;
            P1.perk2_1 = 13;
            P1.perk2_2 = 14;
            P1.perkTree_1 = 111;
            P1.perkTree_2 = 222;
            P1.statPerkDefense = 15;
            P1.statPerkFlex = 16;
            P1.statPerkOffense = 17;
            P1.summoner1Id = 18;
            P1.summoner2Id = 19;
            P1.win = true;

            P2 = P1.CloneObject();
            P2.championId = 2000;
            P2.perk1_1 = 90;
            P2.perk1_2 = 100;
            P2.perk1_3 = 110;
            P2.perk1_4 = 120;
            P2.perk2_1 = 130;
            P2.perk2_2 = 140;
            P2.perkTree_1 = 333;
            P2.perkTree_2 = 444;
            P2.win = false;
            List<Participant> participants = new() { P1, P2 };
            Match = new();
            Match.participants = participants;

            DataCollector dataCollector = new();
            Results = dataCollector.GetData(new List<LeagueMatch>() { Match });
        }

        [TestMethod]
        public void CollectData_ExpectedData_ChampionData()
        {
            Dictionary<int, WinLossData> championData = Results.GetChampionData();
            Assert.IsTrue(championData.Count == 2);
            Assert.IsTrue(championData[P1.championId.Value].GetWinRate() == 100);
            Assert.IsTrue(championData[P2.championId.Value].GetWinRate() == 0);

            Dictionary<int, WinLossData> itemData = Results.GetItemData();
            Assert.IsTrue(itemData.Count == 7);
            Assert.IsTrue(itemData[P1.item0.Value].GetWinRate() == 50);
            Assert.IsTrue(itemData[P1.item1.Value].GetWinRate() == 50);
            Assert.IsTrue(itemData[P1.item2.Value].GetWinRate() == 50);
            Assert.IsTrue(itemData[P1.item3.Value].GetWinRate() == 50);
            Assert.IsTrue(itemData[P1.item4.Value].GetWinRate() == 50);
            Assert.IsTrue(itemData[P1.item5.Value].GetWinRate() == 50);
            Assert.IsTrue(itemData[P1.item6.Value].GetWinRate() == 50);
        }

        [TestMethod]
        public void CollectData_ExpectedData_RuneData()
        {
            Dictionary<int, WinLossData> runeData = Results.GetRuneData();
            Assert.IsTrue(runeData.Count == 12);
            Assert.IsTrue(runeData[P1.perk1_1.Value].GetWinRate() == 100);
            Assert.IsTrue(runeData[P1.perk1_2.Value].GetWinRate() == 100);
            Assert.IsTrue(runeData[P1.perk1_3.Value].GetWinRate() == 100);
            Assert.IsTrue(runeData[P1.perk1_4.Value].GetWinRate() == 100);
            Assert.IsTrue(runeData[P1.perk2_1.Value].GetWinRate() == 100);
            Assert.IsTrue(runeData[P1.perk2_2.Value].GetWinRate() == 100);
            Assert.IsTrue(runeData[P2.perk1_1.Value].GetWinRate() == 0);
            Assert.IsTrue(runeData[P2.perk1_2.Value].GetWinRate() == 0);
            Assert.IsTrue(runeData[P2.perk1_3.Value].GetWinRate() == 0);
            Assert.IsTrue(runeData[P2.perk1_4.Value].GetWinRate() == 0);
            Assert.IsTrue(runeData[P2.perk2_1.Value].GetWinRate() == 0);
            Assert.IsTrue(runeData[P2.perk2_2.Value].GetWinRate() == 0);
        }

        [TestMethod]
        public void CollectData_ExpectedData_RuneTreeData()
        {
            Dictionary<int, WinLossData> runeTreeData = Results.GetRuneTreeData();
            Assert.IsTrue(runeTreeData.Count == 4);
            Assert.IsTrue(runeTreeData[P1.perkTree_1.Value].GetWinRate() == 100);
            Assert.IsTrue(runeTreeData[P1.perkTree_2.Value].GetWinRate() == 100);
            Assert.IsTrue(runeTreeData[P2.perkTree_1.Value].GetWinRate() == 0);
            Assert.IsTrue(runeTreeData[P2.perkTree_2.Value].GetWinRate() == 0);
        }

        [TestMethod]
        public void CollectData_ExpectedData_RuneSpellData()
        {
            Dictionary<int, WinLossData> spellData = Results.GetSpellData();
            Assert.IsTrue(spellData.Count == 2);
            Assert.IsTrue(spellData[P1.summoner1Id.Value].GetWinRate() == 50);
            Assert.IsTrue(spellData[P1.summoner2Id.Value].GetWinRate() == 50);
        }

        [TestMethod]
        public void CollectData_ExpectedData_StatPerkData()
        {
            Dictionary<int, WinLossData> statPerkData = Results.GetStatPerkData();
            Assert.IsTrue(statPerkData.Count == 3);
            Assert.IsTrue(statPerkData[P1.statPerkDefense.Value].GetWinRate() == 50);
            Assert.IsTrue(statPerkData[P1.statPerkFlex.Value].GetWinRate() == 50);
            Assert.IsTrue(statPerkData[P1.statPerkOffense.Value].GetWinRate() == 50);
        }
    }
}
