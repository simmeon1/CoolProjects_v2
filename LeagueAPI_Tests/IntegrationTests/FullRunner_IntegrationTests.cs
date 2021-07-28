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
        private Parameters TestData { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            TestData = File.ReadAllText((string)TestContext.Properties["integrationTestDataPath"]).DeserializeObject<Parameters>();
        }
        
        [TestMethod]
        public async Task FullRun_DoTest_NoPredefinedMatchFile()
        {
            RealHttpClient http = new();
            Delayer delayer = new();
            Logger_Debug logger = new();
            LeagueAPIClient client = new(http, TestData.Token, delayer, logger);
            MatchCollector collector = new(client, logger);
            DdragonRepository repo = new(new RealFileIO(), TestData.DdragonJsonFilesDirectoryPath);
            RealFileIO fileIO = new();
            RealDateTimeProvider dateTimeProvider = new();
            RealGuidProvider guidProvider = new();
            ExcelPrinter printer = new();
            FullRunner runner = new(collector, repo, fileIO, dateTimeProvider, guidProvider, printer);
            List<string> createdFiles = await runner.DoFullRun(TestData.OutputDirectory, 450, TestData.AccountPuuid, maxCount: 1);
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
            LeagueAPIClient client = new(http, TestData.Token, delayer, logger);
            MatchCollector collector = new(client, logger);
            DdragonRepository repo = new(new RealFileIO(), TestData.DdragonJsonFilesDirectoryPath);
            RealFileIO fileIO = new();
            RealDateTimeProvider dateTimeProvider = new();
            ExcelPrinter printer = new();
            RealGuidProvider guidProvider = new();
            FullRunner runner = new(collector, repo, fileIO, dateTimeProvider, guidProvider, printer);
            List<string> createdFiles = runner.DoFullRun(TestData.OutputDirectory, Path.Combine(TestData.OutputDirectory, "matches.json"));
            Assert.IsTrue(createdFiles.Count == 2);
            foreach (string file in createdFiles)
            {
                try {File.Delete(file);}
                catch (Exception) {}
            }
        }
    }
}