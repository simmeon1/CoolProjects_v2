using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LeagueAPI_Tests
{
    [TestClass]
    public class LeagueAPI_UnitTests
    {
        private Mock<IHttpClient> ClientMock { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            ClientMock = new();
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_GetsAccount()
        {
            Account testAccount = new("1", "2", "3", "4");
            HttpResponseMessage response = GetSuccessfulResponse(
                @"{
                    'id': '" + testAccount.Id + @"',
                    'accountId': '" + testAccount.AccountId + @"',
                    'puuid': '" + testAccount.Puuid + @"',
                    'name': '" + testAccount.Name + @"'
                }"
            );
            ClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);

            LeagueAPIClient leagueClient = new(ClientMock.Object, "someKey");
            Account account = await leagueClient.GetAccountBySummonerName("someName");
            Assert.IsTrue(account.Id.Equals(testAccount.Id));
            Assert.IsTrue(account.AccountId.Equals(testAccount.AccountId));
            Assert.IsTrue(account.Puuid.Equals(testAccount.Puuid));
            Assert.IsTrue(account.Name.Equals(testAccount.Name));
        }
        
        [TestMethod]
        public async Task GetAccountBySummonerName_ThrowsException()
        {
            HttpResponseMessage response = new(HttpStatusCode.BadRequest);
            response.Content = new StringContent("");
            ClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);
            LeagueAPIClient leagueClient = new(ClientMock.Object, "someKey");
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => leagueClient.GetAccountBySummonerName("someName"));
        }
        
        [TestMethod]
        public async Task GetAccountBySummonerName_TooManyRequest_HasValidRetryHeaderValue()
        {
            await TestRequestWithRetryHeaderValue(new RetryConditionHeaderValue(TimeSpan.FromMilliseconds(1)));
        }
        
        [TestMethod]
        public async Task GetAccountBySummonerName_TooManyRequest_HasInvalidRetryHeaderValue()
        {
            await TestRequestWithRetryHeaderValue(null);
        }

        private async Task TestRequestWithRetryHeaderValue(RetryConditionHeaderValue retryConditionHeaderValue)
        {
            HttpResponseMessage tooManyRequestsResponse = new(HttpStatusCode.TooManyRequests);
            tooManyRequestsResponse.Headers.RetryAfter = retryConditionHeaderValue;
            tooManyRequestsResponse.Content = new StringContent("");

            string matchId1 = "EUW1_5364680752";
            HttpResponseMessage response = GetSuccessfulResponse(
                @"[
                    '" + matchId1 + @"'
                  ]"
            );

            ClientMock.SetupSequence(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result)
                .Returns(tooManyRequestsResponse)
                .Returns(response);
            LeagueAPIClient leagueClient = new(ClientMock.Object, "someKey");
            List<string> matchIds = await leagueClient.GetMatchIds(450, "somePuuid");
            Assert.IsTrue(matchIds.Count == 1);
            Assert.IsTrue(matchIds[0].Contains(matchId1));
        }

        [TestMethod]
        public async Task GetMatchIds_GetsResults()
        {
            string matchId1 = "EUW1_5364680752";
            string matchId2 = "EUW1_5357084019";
            HttpResponseMessage response = GetSuccessfulResponse(
                @"[
                    '" + matchId1 + @"',
                    '" + matchId2 + @"'
                  ]"
            );

            ClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);
            LeagueAPIClient leagueClient = new(ClientMock.Object, "someKey");
            List<string> matchIds = await leagueClient.GetMatchIds(450, "somePuuid");
            Assert.IsTrue(matchIds.Count == 2);
            Assert.IsTrue(matchIds[0].Contains(matchId1));
            Assert.IsTrue(matchIds[1].Contains(matchId2));
        }
        
        [TestMethod]
        public async Task GetMatches_GetsAllResults()
        {
            HttpResponseMessage response = GetSuccessfulResponse(
                @"{'metadata':{'matchId':'EUW1_5364680752','participants':['TxdGlxaUW6x9KvDuk-FEXYbancmWThQ-PQgfkrKW898JcYyAM43T-Gn0sUi0LbYIsUWDJhgxRS_8Wg','G3_8zPn_vTiFPTElkGg7Q3eLF_b9CQ7XZX8vIKYA-jVn2rk-cCihPCaWbRiOdmeBeEg6XkarJwzmUg']},'info':{'gameVersion':'11.14.385.9967','mapId':12,'participants':[{'championId':1, 'puuid': '9zz9VE1mATZrQfcPdgkRw6EyOIAD4h99mtLx8U3F1kAPz2hbAim92GQYcPjurBMDpIGAFKtzgGNL9Q','item0':20,'item1':21,'item2':22,'item3':23,'item4':24,'item5':25,'item6':26,'perks':{'statPerks':{'defense':100,'flex':101,'offense':102},'styles':[{'selections':[{'perk':200},{'perk':201},{'perk':202},{'perk':203}],'style':2000},{'selections':[{'perk':301},{'perk':302}],'style':3000}]},'summoner1Id':50,'summoner2Id':51,'win':true}],'queueId':450}}"
            );

            ClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);
            LeagueAPIClient leagueClient = new(ClientMock.Object, "someKey");
            LeagueMatch match = await leagueClient.GetMatch("EUW1_5364680752");
            Assert.IsTrue(match.matchId.Equals("EUW1_5364680752"));
            Assert.IsTrue(match.gameVersion.Equals("11.14.385.9967"));
            Assert.IsTrue(match.mapId == 12);
            Assert.IsTrue(match.queueId == 450);
            Assert.IsTrue(match.participants.Count == 1);

            Participant p1 = match.participants[0];
            Assert.IsTrue(p1.championId == 1);
            Assert.IsTrue(p1.puuid.Equals("9zz9VE1mATZrQfcPdgkRw6EyOIAD4h99mtLx8U3F1kAPz2hbAim92GQYcPjurBMDpIGAFKtzgGNL9Q"));
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

        private static HttpResponseMessage GetSuccessfulResponse(string responseContent)
        {
            HttpResponseMessage response = new(HttpStatusCode.OK);
            response.Content = new StringContent(responseContent);
            return response;
        }
    }
}
