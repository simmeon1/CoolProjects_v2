using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueAPI_Tests
{
    [TestClass]
    public class LeagueAPI_UnitTests
    {
        [TestMethod]
        public async Task GetAccountBySummonerName_GetsAccount()
        {
            string id = "1";
            string accountId = "2";
            string puuid = "3";
            string name = "4";
            Account testAccount = new(id, accountId, puuid, name);

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

            LeagueAPIClient leagueClient = new(httpClientMock.Object, "someKey");
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
            Mock<IHttpClient> httpClientMock = new();
            httpClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()).Result).Returns(response);

            LeagueAPIClient leagueClient = new(httpClientMock.Object, "someKey");
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => leagueClient.GetAccountBySummonerName("someName"));
        }
    }
}
