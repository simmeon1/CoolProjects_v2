using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.IntegrationTests
{
    [TestClass]
    public class FullRunner_IntegrationTests
    {
        public TestContext TestContext { get; set; }
        private IntegrationTestData IntegrationTestData { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            IntegrationTestData = File.ReadAllText((string)TestContext.Properties["integrationTestDataPath"]).DeserializeObject<IntegrationTestData>();
        }
        
        [TestMethod]
        public async Task FullRun_DoTest_NoPredefinedMatchFile()
        {
            RealHttpClient http = new();
            Delayer delayer = new();
            Logger_Debug logger = new();
            LeagueAPIClient client = new(http, IntegrationTestData.Token, delayer, logger);
            MatchCollector collector = new(client, logger);
            DdragonRepository repo = new(IntegrationTestData.DdragonJsonFilesDirectoryPath);
            FullRunner runner = new(collector, repo);
            List<string> createdFiles = await runner.DoFullRun(IntegrationTestData.OutputDirectory, 450, IntegrationTestData.AccountPuuid, maxCount: 1);
            Assert.IsTrue(createdFiles.Count == 3);
            foreach (string file in createdFiles)
            {
                try {File.Delete(file);}
                catch (Exception) {}
            }
        }
        
        [TestMethod]
        public void FullRun_DoTest_HasPredefinedMatchFile()
        {
            RealHttpClient http = new();
            Delayer delayer = new();
            Logger_Debug logger = new();
            LeagueAPIClient client = new(http, IntegrationTestData.Token, delayer, logger);
            MatchCollector collector = new(client, logger);
            DdragonRepository repo = new(IntegrationTestData.DdragonJsonFilesDirectoryPath);
            FullRunner runner = new(collector, repo);
            List<string> createdFiles = runner.DoFullRun(IntegrationTestData.OutputDirectory, Path.Combine(IntegrationTestData.OutputDirectory, "matches.json"));
            Assert.IsTrue(createdFiles.Count == 2);
            foreach (string file in createdFiles)
            {
                try {File.Delete(file);}
                catch (Exception) {}
            }
        }
    }
}