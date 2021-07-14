using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueAPI_Tests
{
    [TestClass]
    public class LeagueAPI_UnitTests
    {
        private Account Account { get; set; }
        private Mock<IHttpClient> ClientMock { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Account = new("1", "2", "3", "4");
            ClientMock = new();
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_GetsAccount()
        {
            HttpResponseMessage response = GetSuccessfulResponse(
                @"{
                    'id': '" + Account.Id + @"',
                    'accountId': '" + Account.AccountId + @"',
                    'puuid': '" + Account.Puuid + @"',
                    'name': '" + Account.Name + @"'
                }"
            );
            ClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);

            LeagueAPIClient leagueClient = await LeagueAPIClient.GetClientInstance(ClientMock.Object, "someKey", "someName");
            Assert.IsTrue(leagueClient.Account.Id.Equals(Account.Id));
            Assert.IsTrue(leagueClient.Account.AccountId.Equals(Account.AccountId));
            Assert.IsTrue(leagueClient.Account.Puuid.Equals(Account.Puuid));
            Assert.IsTrue(leagueClient.Account.Name.Equals(Account.Name));
        }
        
        [TestMethod]
        public async Task GetAccountBySummonerName_ThrowsException()
        {
            HttpResponseMessage response = new(HttpStatusCode.BadRequest);
            response.Content = new StringContent("");
            ClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => LeagueAPIClient.GetClientInstance(ClientMock.Object, "", ""));
        }

        [TestMethod]
        public async Task GetMatchIds_GetsResults()
        {
            string matchId1 = "EUW1_5364680752";
            string matchId2 = "EUW1_5357084019";
            HttpResponseMessage response = new(HttpStatusCode.OK);
            response.Content = new StringContent(
                @"[
                    '" + matchId1 + @"',
                    '" + matchId2 + @"'
                  ]"
            );

            ClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);
            LeagueAPIClient leagueClient = LeagueAPIClient.GetClientInstance(ClientMock.Object, "someKey", Account);
            List<string> matchIds = await leagueClient.GetMatchIds(450);
            Assert.IsTrue(matchIds.Count == 2);
            Assert.IsTrue(matchIds[0].Contains(matchId1));
            Assert.IsTrue(matchIds[1].Contains(matchId2));
        }
        
        [TestMethod]
        public async Task GetMatches_GetsAllResults()
        {
            HttpResponseMessage response = GetSuccessfulResponse(
                @"{'metadata':{'matchId':'EUW1_5364680752','participants':['TxdGlxaUW6x9KvDuk-FEXYbancmWThQ-PQgfkrKW898JcYyAM43T-Gn0sUi0LbYIsUWDJhgxRS_8Wg','G3_8zPn_vTiFPTElkGg7Q3eLF_b9CQ7XZX8vIKYA-jVn2rk-cCihPCaWbRiOdmeBeEg6XkarJwzmUg']},'info':{'gameVersion':'11.14.385.9967','mapId':12,'participants':[{'championId':1,'item0':20,'item1':21,'item2':22,'item3':23,'item4':24,'item5':25,'item6':26,'perks':{'statPerks':{'defense':100,'flex':101,'offense':102},'styles':[{'selections':[{'perk':200},{'perk':201},{'perk':202},{'perk':203}],'style':2000},{'selections':[{'perk':301},{'perk':302}],'style':3000}]},'summoner1Id':50,'summoner2Id':51,'win':true}],'queueId':450}}"
            );

            ClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);
            LeagueAPIClient leagueClient = LeagueAPIClient.GetClientInstance(ClientMock.Object, "someKey", Account);
            List<LeagueMatch> matches = await leagueClient.GetMatches(new List<string>() { "EUW1_5364680752" });
            Assert.IsTrue(matches.Count == 1);

            LeagueMatch match = matches.FirstOrDefault();
            Assert.IsTrue(match.matchId.Equals("EUW1_5364680752"));
            Assert.IsTrue(match.gameVersion.Equals("11.14.385.9967"));
            Assert.IsTrue(match.mapId == 12);
            Assert.IsTrue(match.queueId == 450);
            Assert.IsTrue(match.participants.Count == 1);

            Participant p1 = match.participants[0];
            Assert.IsTrue(p1.championId == 1);
            Assert.IsTrue(p1.item0 == 20);
            Assert.IsTrue(p1.item1 == 21);
            Assert.IsTrue(p1.item2 == 22);
            Assert.IsTrue(p1.item3 == 23);
            Assert.IsTrue(p1.item4 == 24);
            Assert.IsTrue(p1.item5 == 25);
            Assert.IsTrue(p1.item6 == 26);
            Assert.IsTrue(p1.statPerkDefense == 100);
            Assert.IsTrue(p1.statPerkFlex == 101);
            Assert.IsTrue(p1.statPerkOffense == 102);
            Assert.IsTrue(p1.perkTree_1 == 2000);
            Assert.IsTrue(p1.perkTree_2 == 3000);
            Assert.IsTrue(p1.perk1_1 == 200);
            Assert.IsTrue(p1.perk1_2 == 201);
            Assert.IsTrue(p1.perk1_3 == 202);
            Assert.IsTrue(p1.perk1_4 == 203);
            Assert.IsTrue(p1.perk2_1 == 301);
            Assert.IsTrue(p1.perk2_2 == 302);
            Assert.IsTrue(p1.summoner1Id == 50);
            Assert.IsTrue(p1.summoner2Id == 51);
            Assert.IsTrue(p1.win == true);
        }

        [TestMethod]
        public async Task GetMatches_GetsResultsBasedOnVersion()
        {
            HttpResponseMessage response1 = GetSuccessfulResponse(@"{'info':{'gameVersion':'11.16','participants':[]}}");
            HttpResponseMessage response2 = GetSuccessfulResponse(@"{'metadata':{'matchId':'EUW1_5364680752'},'info':{'gameVersion':'11.15','mapId':12,'participants':[],'queueId':450}}");
            HttpResponseMessage response3 = GetSuccessfulResponse(@"{'info':{'gameVersion':'11.14','participants':[]}}");
            HttpResponseMessage response4 = GetSuccessfulResponse(@"");
            ClientMock.SetupSequence(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result)
                .Returns(response1)
                .Returns(response2)
                .Returns(response3)
                .Returns(response4);
            LeagueAPIClient leagueClient = LeagueAPIClient.GetClientInstance(ClientMock.Object, "someKey", Account);
            List<LeagueMatch> matches = await leagueClient.GetMatches(new List<string>() { "1", "2", "3", "4" }, "11.15");
            Assert.IsTrue(matches.Count == 1);
            Assert.IsTrue(matches.FirstOrDefault().gameVersion.Equals("11.15"));
        }

        private static HttpResponseMessage GetSuccessfulResponse(string responseContent)
        {
            HttpResponseMessage response1 = new(HttpStatusCode.OK);
            response1.Content = new StringContent(responseContent);
            return response1;
        }

        [TestMethod]
        public void CompareTargetVersionAgainstGameVersion_DoesCorrectComparisons()
        {
            Assert.IsTrue(LeagueAPIClient.CompareTargetVersionAgainstGameVersion("11.14", "11.14.56") == -1);
            Assert.IsTrue(LeagueAPIClient.CompareTargetVersionAgainstGameVersion("11.13", "11.14.56") == -1);
            Assert.IsTrue(LeagueAPIClient.CompareTargetVersionAgainstGameVersion("11.15", "11.14.56") == 1);
            Assert.IsTrue(LeagueAPIClient.CompareTargetVersionAgainstGameVersion("11.14.5", "11.14") == 1);
            Assert.IsTrue(LeagueAPIClient.CompareTargetVersionAgainstGameVersion("11.13.5", "11.14") == -1);
            Assert.IsTrue(LeagueAPIClient.CompareTargetVersionAgainstGameVersion("11.15.5", "11.14") == 1);
            Assert.IsTrue(LeagueAPIClient.CompareTargetVersionAgainstGameVersion("11.15", "11.15.0") == 0);
            Assert.IsTrue(LeagueAPIClient.CompareTargetVersionAgainstGameVersion("11.15", "11.15") == 0);
            Assert.IsTrue(LeagueAPIClient.CompareTargetVersionAgainstGameVersion("12.0", "11.15") == 1);
            Assert.IsTrue(LeagueAPIClient.CompareTargetVersionAgainstGameVersion("11.9", "12") == -1);
        }
    }
}
