using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueAPI_Tests
{
    [TestClass]
    public class LeagueAPI_UnitTests
    {
        private const string id = "1";
        private const string accountId = "2";
        private const string puuid = "3";
        private const string name = "4";
        private Account Account { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Account = new(id, accountId, puuid, name);
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_GetsAccount()
        {
            HttpResponseMessage response = new(HttpStatusCode.OK);
            response.Content = new StringContent(
                @"{
                    'id': '" + id + @"',
                    'accountId': '" + accountId + @"',
                    'puuid': '" + puuid + @"',
                    'name': '" + name + @"'
                }"
            );
            Mock<IHttpClient> httpClientMock = new();
            httpClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);

            LeagueAPIClient leagueClient = await LeagueAPIClient.GetClientInstance(httpClientMock.Object, "someKey", "someName");
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
            Mock<IHttpClient> httpClientMock = new();
            httpClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => LeagueAPIClient.GetClientInstance(httpClientMock.Object, "", ""));
        }

        [TestMethod]
        public async Task GetMatches_GetsResults()
        {
            HttpResponseMessage response = new(HttpStatusCode.OK);
            string matchId1 = "EUW1_5364680752";
            string matchId2 = "EUW1_5357084019";
            response.Content = new StringContent(
                @"[
                    '" + matchId1 + @"',
                    '" + matchId2 + @"'
                  ]"
            );

            Mock<IHttpClient> httpClientMock = new();
            httpClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);
            LeagueAPIClient leagueClient = LeagueAPIClient.GetClientInstance(httpClientMock.Object, "someKey", Account);
            List<string> matchIds = await leagueClient.GetMatchIds(450);
            Assert.IsTrue(matchIds.Count == 2);
            Assert.IsTrue(matchIds[0].Contains(matchId1));
            Assert.IsTrue(matchIds[1].Contains(matchId2));
        }
    }
}
