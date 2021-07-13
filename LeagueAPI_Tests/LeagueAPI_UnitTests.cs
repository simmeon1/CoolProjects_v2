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
            HttpResponseMessage response = new(HttpStatusCode.OK);
            response.Content = new StringContent(
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
        public async Task GetMatches_GetsResults()
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
    }
}
