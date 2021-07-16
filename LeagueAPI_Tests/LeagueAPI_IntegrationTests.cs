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

namespace LeagueAPI_Tests
{
    [TestClass]
    public class LeagueAPI_IntegrationTests
    {
        public TestContext TestContext { get; set; }
        private Credentials Credentials { get; set; }
        private IHttpClient HttpClient { get; set; }

        public LeagueAPI_IntegrationTests()
        {
            HttpClient = new RealHttpClient();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Credentials = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText((string)TestContext.Properties["credentialsPath"]));
        }

        [TestMethod]
        public async Task GetAccountBySummonerName_KeyIsValid()
        {
            LeagueAPIClient leagueClient = new(HttpClient, Credentials.Token);
            Account acc = await leagueClient.GetAccountBySummonerName(Credentials.AccountName);
            Assert.IsTrue(acc.Name.Equals(Credentials.AccountName));
            Assert.IsTrue(acc.AccountId.Length > 0);
            Assert.IsTrue(acc.Id.Length > 0);
            Assert.IsTrue(acc.Puuid.Length > 0);
        }
    }
}
