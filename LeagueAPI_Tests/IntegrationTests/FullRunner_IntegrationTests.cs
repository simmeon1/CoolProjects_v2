using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
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
            RealDelayer delayer = new();
            Logger_Debug logger = new();
            RealWebClient webClient = new();
            LeagueAPIClient client = new(http, TestData.Token, delayer, logger);
            MatchCollector collector = new(client, logger);
            RealFileIO fileIO = new();
            DdragonRepository repo = new(fileIO, TestData.DdragonJsonFilesDirectoryPath);
            ArchiveExtractor extractor = new();
            DdragonRepositoryUpdater repoUpdater = new(http, webClient, fileIO, logger, extractor, TestData.DdragonJsonFilesDirectoryPath);
            RealDateTimeProvider dateTimeProvider = new();
            RealGuidProvider guidProvider = new();
            ExcelPrinter printer = new();
            FullRunner runner = new(collector, repo, fileIO, dateTimeProvider, guidProvider, printer, logger, repoUpdater);
            List<string> createdFiles = await runner.DoFullRun(TestData.OutputDirectory, 450, TestData.AccountPuuid, TestData.RangeOfTargetVersions, maxCount: 1, null, null, false);
            Assert.IsTrue(createdFiles.Count == 4);
            DeleteCreatedFiles(createdFiles);
        }

        private static void DeleteCreatedFiles(List<string> createdFiles)
        {
            foreach (string file in createdFiles)
            {
                try { File.Delete(file); }
                catch (Exception) { }
            }
        }
    }
}