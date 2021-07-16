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
    public class LeagueAPI_IntegrationTests
    {
        public TestContext TestContext { get; set; }
        private string Token { get; set; }
        private string AccountName { get; set; }
        private IHttpClient HttpClient { get; set; }

        public LeagueAPI_IntegrationTests()
        {
            HttpClient = new RealHttpClient();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Token = (string)TestContext.Properties["riotToken"];
            AccountName = (string)TestContext.Properties["accountName"];
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_KeyIsValid()
        {
            LeagueAPIClient leagueClient = new(HttpClient, Token);
            Account acc = await leagueClient.GetAccountBySummonerName(AccountName);
            Assert.IsTrue(acc.Name.Equals(AccountName));
            Assert.IsTrue(acc.AccountId.Length > 0);
            Assert.IsTrue(acc.Id.Length > 0);
            Assert.IsTrue(acc.Puuid.Length > 0);
        }
    }
}
