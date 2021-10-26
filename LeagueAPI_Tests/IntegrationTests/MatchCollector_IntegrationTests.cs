using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.IntegrationTests
{
    [TestClass]
    public class MatchCollector_IntegrationTests
    {
        public TestContext TestContext { get; set; }
        private Parameters TestData { get; set; }
        private IHttpClient HttpClient { get; set; }
        private LeagueAPIClient LeagueAPIClient { get; set; }
        private MatchCollector MatchCollector { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            TestData = JsonConvert.DeserializeObject<Parameters>(File.ReadAllText((string)TestContext.Properties["integrationTestDataPath"]));
            HttpClient = new RealHttpClient();
            LeagueAPIClient = new(HttpClient, TestData.Token, new RealDelayer(), new Logger_Debug());
            MatchCollector = new MatchCollector(LeagueAPIClient, new Logger_Debug());
        }

        [TestMethod]
        public async Task CollectMatches_GetsResults_VersionProvided()
        {
            int maxCount = 1;
            List<LeagueMatch> matches = await MatchCollector.GetMatches(TestData.AccountPuuid, 450, TestData.RangeOfTargetVersions, maxCount: maxCount);
            Assert.IsTrue(matches.Count == maxCount);
            Assert.IsTrue(matches.Select(m => m.matchId).Distinct().Count() == maxCount);
            DataCollector dataCollector = new();
            DataCollectorResults results = dataCollector.GetData(matches);
            Assert.IsTrue(results.GetChampionData().Count > 0);
            Assert.IsTrue(results.GetItemData().Count > 0);
            Assert.IsTrue(results.GetRuneData().Count > 0);
            Assert.IsTrue(results.GetRuneTreeData().Count > 0);
            Assert.IsTrue(results.GetSpellData().Count > 0);
            Assert.IsTrue(results.GetStatPerkData().Count > 0);
        }

        [Ignore]
        [TestMethod]
        public async Task CollectMatches_FullTest()
        {
            List<LeagueMatch> matches = await MatchCollector.GetMatches(TestData.AccountPuuid, 450, TestData.RangeOfTargetVersions, maxCount: 0);
            Assert.IsTrue(matches.Count > 0);
        }
    }
}