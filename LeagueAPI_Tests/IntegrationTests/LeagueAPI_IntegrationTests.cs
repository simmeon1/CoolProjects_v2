using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.IntegrationTests
{
    [TestClass]
    public class LeagueAPI_IntegrationTests
    {
        public TestContext TestContext { get; set; }
        private IntegrationTestData IntegrationTestData { get; set; }
        private IHttpClient HttpClient { get; set; }
        private LeagueAPIClient LeagueAPIClient { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            IntegrationTestData = JsonConvert.DeserializeObject<IntegrationTestData>(File.ReadAllText((string)TestContext.Properties["integrationTestDataPath"]));
            HttpClient = new RealHttpClient();
            LeagueAPIClient = new(HttpClient, IntegrationTestData.Token, new Delayer(), new Logger_Debug());
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_KeyIsValid()
        {
            Account acc = await LeagueAPIClient.GetAccountBySummonerName(IntegrationTestData.AccountName);
            Assert.IsTrue(acc.Name.Equals(IntegrationTestData.AccountName));
            Assert.IsTrue(acc.AccountId.Length > 0);
            Assert.IsTrue(acc.Id.Length > 0);
            Assert.IsTrue(acc.Puuid.Length > 0);
        }

        [TestMethod]
        public async Task GetMatchIds_MatchIdsReturned()
        {
            List<string> matchIds = await LeagueAPIClient.GetMatchIds(450, IntegrationTestData.AccountPuuid);
            Assert.IsTrue(matchIds.Count > 1);
        }
        
        [TestMethod]
        public async Task GetMatch_DataCollected()
        {
            List<string> matchIds = await LeagueAPIClient.GetMatchIds(450, IntegrationTestData.AccountPuuid);
            Assert.IsTrue(matchIds.Count > 1);
            LeagueMatch match = await LeagueAPIClient.GetMatch(matchIds[0]);
            Assert.IsTrue(match.matchId.Length > 0);
            Assert.IsTrue(match.gameVersion.Length > 0);
            Assert.IsTrue(match.mapId.HasValue);
            Assert.IsTrue(match.queueId.HasValue);
            Assert.IsTrue(match.participants.Count > 0);

            Participant p1 = match.participants[0];
            Assert.IsTrue(p1.championId.HasValue);
            Assert.IsTrue(p1.puuid.Length > 0);
            Assert.IsTrue(p1.item0.HasValue);
            Assert.IsTrue(p1.item1.HasValue);
            Assert.IsTrue(p1.item2.HasValue);
            Assert.IsTrue(p1.item3.HasValue);
            Assert.IsTrue(p1.item4.HasValue);
            Assert.IsTrue(p1.item5.HasValue);
            Assert.IsTrue(p1.item6.HasValue);
            Assert.IsTrue(p1.statPerkDefense.HasValue);
            Assert.IsTrue(p1.statPerkFlex.HasValue);
            Assert.IsTrue(p1.statPerkOffense.HasValue);
            Assert.IsTrue(p1.perkTree_1.HasValue);
            Assert.IsTrue(p1.perkTree_2.HasValue);
            Assert.IsTrue(p1.perk1_1.HasValue);
            Assert.IsTrue(p1.perk1_2.HasValue);
            Assert.IsTrue(p1.perk1_3.HasValue);
            Assert.IsTrue(p1.perk1_4.HasValue);
            Assert.IsTrue(p1.perk2_1.HasValue);
            Assert.IsTrue(p1.perk2_2.HasValue);
            Assert.IsTrue(p1.summoner1Id.HasValue);
            Assert.IsTrue(p1.summoner2Id.HasValue);
            Assert.IsTrue(p1.win.HasValue);
        }
    }
}
