using Common_ClassLibrary;
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

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class LeagueAPI_UnitTests
    {
        private Mock<IHttpClient> HttpClientMock { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            HttpClientMock = new();
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_GetsAccountAfterAnExceiptionThrownBySendRequest()
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
            HttpClientMock.SetupSequence(x => x.SendRequest(It.IsAny<HttpRequestMessage>()).Result)
                .Throws(new Exception("test exception"))
                .Returns(response);

            LeagueAPIClient leagueClient = new(HttpClientMock.Object, "someKey", new Mock<IDelayer>().Object, new Logger_Debug());
            Account account = await leagueClient.GetAccountBySummonerName("someName");
            Assert.IsTrue(account.Id.Equals(testAccount.Id));
            Assert.IsTrue(account.AccountId.Equals(testAccount.AccountId));
            Assert.IsTrue(account.Puuid.Equals(testAccount.Puuid));
            Assert.IsTrue(account.Name.Equals(testAccount.Name));
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_ReturnsNullWhenNotForbidden()
        {
            LeagueAPIClient leagueClient = SetUpHttpClientWithResponse(HttpStatusCode.BadRequest);
            Assert.IsTrue(await leagueClient.GetAccountBySummonerName("someName") == null);
        }

        private LeagueAPIClient SetUpHttpClientWithResponse(HttpStatusCode code, string content = "")
        {
            HttpResponseMessage response = new(code);
            response.Content = new StringContent(content);
            HttpClientMock.Setup(x => x.SendRequest(It.IsAny<HttpRequestMessage>()).Result).Returns(response);
            return new LeagueAPIClient(HttpClientMock.Object, "someKey", new Mock<IDelayer>().Object, new Logger_Debug());
        }

        private LeagueAPIClient SetUpHttpClientWithSuccessfulResponse(string content)
        {
            return SetUpHttpClientWithResponse(HttpStatusCode.OK, content);
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_ThrowsExceptionWhenForbidden()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => SetUpHttpClientWithResponse(HttpStatusCode.Forbidden).GetAccountBySummonerName("someName"));
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_ThrowsExceptionWhenAmbiguous()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => SetUpHttpClientWithResponse(HttpStatusCode.Ambiguous).GetAccountBySummonerName("someName"));
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_TooManyRequest_HasValidRetryHeaderValue()
        {
            HttpResponseMessage response = new(HttpStatusCode.TooManyRequests);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromMilliseconds(1));
            response.Content = new StringContent("");
            await TestRequestWithRetryHeaderValue(response);
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_TooManyRequest_HasInvalidRetryHeaderValue()
        {
            HttpResponseMessage response = new(HttpStatusCode.TooManyRequests);
            response.Content = new StringContent("");
            await TestRequestWithRetryHeaderValue(response);
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_ServerError_WaitAndTryAgain()
        {
            HttpResponseMessage response = new(HttpStatusCode.InternalServerError);
            response.Content = new StringContent("");
            await TestRequestWithRetryHeaderValue(response);
        }

        private async Task TestRequestWithRetryHeaderValue(HttpResponseMessage badResponse)
        {
            string matchId1 = "EUW1_5364680752";
            HttpResponseMessage goodResponse = GetSuccessfulResponse(
                @"[
                    '" + matchId1 + @"'
                  ]"
            );

            HttpClientMock.SetupSequence(x => x.SendRequest(It.IsAny<HttpRequestMessage>()).Result)
                .Returns(badResponse)
                .Returns(goodResponse);
            LeagueAPIClient leagueClient = new(HttpClientMock.Object, "someKey", new Mock<IDelayer>().Object, new Logger_Debug());
            List<string> matchIds = await leagueClient.GetMatchIds("somePuuid", 450);
            Assert.IsTrue(matchIds.Count == 1);
            Assert.IsTrue(matchIds[0].Contains(matchId1));
        }

        [TestMethod]
        public async Task GetMatchIds_GetsResults()
        {
            string matchId1 = "EUW1_5364680752";
            string matchId2 = "EUW1_5357084019";
            LeagueAPIClient leagueClient = SetUpHttpClientWithSuccessfulResponse(@"[
                    '" + matchId1 + @"',
                    '" + matchId2 + @"'
                  ]");
            List<string> matchIds = await leagueClient.GetMatchIds("somePuuid", 450);
            Assert.IsTrue(matchIds.Count == 2);
            Assert.IsTrue(matchIds[0].Contains(matchId1));
            Assert.IsTrue(matchIds[1].Contains(matchId2));
        }

        [TestMethod]
        public async Task GetMatchIds_ReturnsEmptyArrayWhenRequestIsNotFound()
        {
            Assert.IsTrue((await SetUpHttpClientWithResponse(HttpStatusCode.NotFound).GetMatchIds("somePuuid", 450)).Count == 0);
        }

        [TestMethod]
        public async Task GetMatch_GetsAllResults()
        {
            LeagueAPIClient leagueClient = SetUpHttpClientWithSuccessfulResponse(@"{'metadata':{'matchId':'EUW1_5364680752','participants':['TxdGlxaUW6x9KvDuk-FEXYbancmWThQ-PQgfkrKW898JcYyAM43T-Gn0sUi0LbYIsUWDJhgxRS_8Wg','G3_8zPn_vTiFPTElkGg7Q3eLF_b9CQ7XZX8vIKYA-jVn2rk-cCihPCaWbRiOdmeBeEg6XkarJwzmUg']},'info':{'gameVersion':'11.14.385.9967','mapId':12,'participants':[{'championId':1, 'puuid': '9zz9VE1mATZrQfcPdgkRw6EyOIAD4h99mtLx8U3F1kAPz2hbAim92GQYcPjurBMDpIGAFKtzgGNL9Q','item0':20,'item1':21,'item2':22,'item3':23,'item4':24,'item5':25,'item6':26,'perks':{'statPerks':{'defense':100,'flex':101,'offense':102},'styles':[{'selections':[{'perk':200},{'perk':201},{'perk':202},{'perk':203}],'style':2000},{'selections':[{'perk':301},{'perk':302}],'style':3000}]},'summoner1Id':50,'summoner2Id':51,'win':true, 'physicalDamageDealtToChampions':100, 'magicDamageDealtToChampions':200}],'queueId':450,'gameDuration':'600000'}}");
            LeagueMatch match = await leagueClient.GetMatch("EUW1_5364680752");
            Assert.IsTrue(match.matchId.Equals("EUW1_5364680752"));
            Assert.IsTrue(match.gameVersion.Equals("11.14.385.9967"));
            Assert.IsTrue(match.mapId == 12);
            Assert.IsTrue(match.queueId == 450);
            Assert.IsTrue(match.participants.Count == 1);
            Assert.IsTrue(match.duration.TotalMilliseconds == 600000);
            Assert.IsTrue(match.GameIsShorterThanOrEqualToMinutes(10));
            Assert.IsTrue(!match.GameIsShorterThanOrEqualToMinutes(9));

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
            Assert.IsTrue(p1.physicalDamageDealtToChampions == 100);
            Assert.IsTrue(p1.magicDamageDealtToChampions == 200);
        }

        [TestMethod]
        public async Task GetMatch_GetsAllResults_TestTimespanOnly()
        {
            LeagueAPIClient leagueClient = SetUpHttpClientWithSuccessfulResponse(@"{'metadata':{'matchId':'EUW1_5364680752','participants':['TxdGlxaUW6x9KvDuk-FEXYbancmWThQ-PQgfkrKW898JcYyAM43T-Gn0sUi0LbYIsUWDJhgxRS_8Wg','G3_8zPn_vTiFPTElkGg7Q3eLF_b9CQ7XZX8vIKYA-jVn2rk-cCihPCaWbRiOdmeBeEg6XkarJwzmUg']},'info':{'gameVersion':'11.14.385.9967','mapId':12,'participants':[{'championId':1, 'puuid': '9zz9VE1mATZrQfcPdgkRw6EyOIAD4h99mtLx8U3F1kAPz2hbAim92GQYcPjurBMDpIGAFKtzgGNL9Q','item0':20,'item1':21,'item2':22,'item3':23,'item4':24,'item5':25,'item6':26,'perks':{'statPerks':{'defense':100,'flex':101,'offense':102},'styles':[{'selections':[{'perk':200},{'perk':201},{'perk':202},{'perk':203}],'style':2000},{'selections':[{'perk':301},{'perk':302}],'style':3000}]},'summoner1Id':50,'summoner2Id':51,'win':true, 'physicalDamageDealtToChampions':100, 'magicDamageDealtToChampions':200}],'queueId':450,'gameDuration':'1000', 'gameEndTimestamp': '10'}}");
            LeagueMatch match = await leagueClient.GetMatch("EUW1_5364680752");
            Assert.IsTrue(match.duration.TotalSeconds == 1000);
        }

        [TestMethod]
        public async Task GetMatch_ReturnsNullWhenRequestIsUnsuccessful()
        {
            Assert.IsTrue(await SetUpHttpClientWithResponse(HttpStatusCode.BadRequest).GetMatch("EUW1_5364680752") == null);
        }

        [TestMethod]
        public async Task GetLatestVersionsReturnsExpectedResults()
        {
            LeagueAPIClient client = SetUpHttpClientWithSuccessfulResponse(@"[""12.2"",""12.1""]");
            List<string> results = await client.GetLatestVersions();
            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results[0].Equals("12.2"));
            Assert.IsTrue(results[1].Equals("12.1"));

            List<string> parsedVersions = await client.GetParsedListOfVersions(new List<string>() { "0", "-1" });
            Assert.IsTrue(parsedVersions.Count == 2);
            Assert.IsTrue(parsedVersions[0].Equals("12.2"));
            Assert.IsTrue(parsedVersions[1].Equals("12.1"));
        }

        [TestMethod]
        public async Task GetNameOfQueueReturnsExpectedName()
        {
            LeagueAPIClient client = SetUpHttpClientWithSuccessfulResponse(@"[
    {
        ""queueId"": 2,
        ""map"": ""Summoner's Rift""
    },
    {
        ""queueId"": 450,
        ""map"": ""Howling Abyss""
    }
]");
            string name = await client.GetNameOfQueue(10);
            Assert.IsTrue(name.Equals(""));

            name = await client.GetNameOfQueue(450);
            Assert.IsTrue(name.Equals("Howling Abyss"));
        }

        private static HttpResponseMessage GetSuccessfulResponse(string responseContent)
        {
            HttpResponseMessage response = new(HttpStatusCode.OK);
            response.Content = new StringContent(responseContent);
            return response;
        }
        
        [TestMethod]
        public async Task GetSpectatedDataReturnsData()
        {
            LeagueAPIClient client = SetUpHttpClientWithSuccessfulResponse(@"{
    ""participants"": [
        {
            ""teamId"": 100,
            ""championId"": 122,
            ""summonerId"": ""test"",
        }
    ],
}");
            SpectatorData data = await client.GetSpectatorDataByEncryptedSummonerId("test");
            Assert.IsTrue(data.participants.Count == 1);
            Assert.IsTrue(data.participants[0].teamId == 100);
            Assert.IsTrue(data.participants[0].championId == 122);
            Assert.IsTrue(data.participants[0].summonerId.Equals("test"));
        }
        
        [TestMethod]
        public async Task GetSpectatedDataReturnsNoData()
        {
            LeagueAPIClient client = SetUpHttpClientWithResponse(HttpStatusCode.NotFound, "");
            SpectatorData data = await client.GetSpectatorDataByEncryptedSummonerId("test");
            Assert.IsTrue(data == null);
        }
    }
}
