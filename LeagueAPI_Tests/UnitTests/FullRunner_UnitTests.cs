using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class FullRunner_UnitTests
    {
        [TestMethod]
        public async Task FullRunner_ExpectedFileNames()
        {
            Parameters paramms = GetParams();
            List<string> result = await SetupFullRunner().DoFullRun(
                paramms.OutputDirectory,
                paramms.QueueId,
                paramms.AccountPuuid,
                paramms.RangeOfTargetVersions,
                10,
                includeWinRatesForMinutes: paramms.IncludeWinRatesForMinutes,
                null,
                false);
            DefaultAssert(result);
        }
        
        [TestMethod]
        public async Task FullRunner_ExpectedFileNames_MatchesProvided()
        {
            Parameters paramms = GetParams();
            List<string> result = await SetupFullRunner().DoFullRun(
                paramms.OutputDirectory,
                paramms.QueueId,
                paramms.AccountPuuid,
                paramms.RangeOfTargetVersions,
                10,
                includeWinRatesForMinutes: paramms.IncludeWinRatesForMinutes,
                paramms.ExistingMatchesFile,
                false);
            DefaultAssert(result);
        }

        private static void DefaultAssert(List<string> result)
        {
            Assert.IsTrue(result.Count == 5);
            Assert.IsTrue(result[0].Equals($"C:\\Results_HA_12.2,12.1_2020-02-02--00-00-00\\Matches_HA_12.2,12.1_2020-02-02--00-00-00.json"));
            Assert.IsTrue(result[1].Equals($"C:\\Results_HA_12.2,12.1_2020-02-02--00-00-00\\ItemSet_All_HA_12.2,12.1_2020-02-02--00-00-00.json"));
            Assert.IsTrue(result[2].Equals($"C:\\Results_HA_12.2,12.1_2020-02-02--00-00-00\\ItemSet_Sub20_HA_12.2,12.1_2020-02-02--00-00-00.json"));
            Assert.IsTrue(result[3].Equals($"C:\\Results_HA_12.2,12.1_2020-02-02--00-00-00\\Stats_HA_12.2,12.1_2020-02-02--00-00-00.xlsx"));
            Assert.IsTrue(result[4].Equals($"C:\\Results_HA_12.2,12.1_2020-02-02--00-00-00\\Log_HA_12.2,12.1_2020-02-02--00-00-00.txt"));
        }

        [TestMethod]
        public async Task FullRunner_ExpectedFileNames_NoWinRatesIncluded()
        {
            Parameters paramms = GetParams();
            List<string> result = await SetupFullRunner().DoFullRun(paramms.OutputDirectory, paramms.QueueId, paramms.AccountPuuid, paramms.RangeOfTargetVersions, 10, null, null, true);
            Assert.IsTrue(result.Count == 4);
            Assert.IsTrue(result[0].Equals($"C:\\Results_HA_12.2,12.1_2020-02-02--00-00-00\\Matches_HA_12.2,12.1_2020-02-02--00-00-00.json"));
            Assert.IsTrue(result[1].Equals($"C:\\Results_HA_12.2,12.1_2020-02-02--00-00-00\\ItemSet_All_HA_12.2,12.1_2020-02-02--00-00-00.json"));
            Assert.IsTrue(result[2].Equals($"C:\\Results_HA_12.2,12.1_2020-02-02--00-00-00\\Stats_HA_12.2,12.1_2020-02-02--00-00-00.xlsx"));
            Assert.IsTrue(result[3].Equals($"C:\\Results_HA_12.2,12.1_2020-02-02--00-00-00\\Log_HA_12.2,12.1_2020-02-02--00-00-00.txt"));
        }

        [TestMethod]
        public async Task FullRunner_ExpectedFileNames_ExceptionDuringMatchCollection()
        {

            Parameters paramms = GetParams();
            List<string> result = await SetupFullRunner(throwExceptionOnMatchCollection: true).DoFullRun(paramms.OutputDirectory, paramms.QueueId, paramms.AccountPuuid, paramms.RangeOfTargetVersions, 10, null, null, paramms.GetLatestDdragonData);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Equals($"C:\\Results_HA_12.2,12.1_2020-02-02--00-00-00\\Log_HA_12.2,12.1_2020-02-02--00-00-00.txt"));
        }

        private static Parameters GetParams()
        {
            return new()
            {
                Name = "ss",
                AccountPuuid = "ss",
                DdragonJsonFilesDirectoryPath = "ss",
                MaxCount = 1,
                OutputDirectory = "C:\\",
                QueueId = 450,
                RangeOfTargetVersions = new List<string> { "0", "-1" },
                Token = "ss",
                IncludeWinRatesForMinutes = new List<int>() { 20 },
                ExistingMatchesFile = "gg"
            };
        }

        private static FullRunner SetupFullRunner(bool throwExceptionOnMatchCollection = false, bool throwExceptionOnMatchFileRead = false)
        {
            Logger_Debug logger = new();

            Mock<IMatchCollector> collector = new();

            if (throwExceptionOnMatchCollection) collector.Setup(x => x.GetMatches(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<string>>(), It.IsAny<int>(), It.IsAny<List<LeagueMatch>>()).Result).Throws(new Exception("ex"));
            else collector.Setup(x => x.GetMatches(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<string>>(), It.IsAny<int>(), It.IsAny<List<LeagueMatch>>()).Result).Returns(new List<LeagueMatch>());

            Mock<IDDragonRepository> repo = new();
            Mock<IFileIO> fileIO = new();

            if (throwExceptionOnMatchFileRead) fileIO.Setup(x => x.ReadAllText(It.IsAny<string>())).Throws(new Exception("ex"));
            else fileIO.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("[]");

            Mock<IDateTimeProvider> dateTimeProvider = new();
            dateTimeProvider.Setup(x => x.Now()).Returns(new DateTime(2020, 2, 2));

            Mock<IGuidProvider> guidProvider = new();
            guidProvider.Setup(x => x.NewGuid()).Returns("someGuid");

            Mock<IExcelPrinter> excelPrinter = new();

            Mock<ILeagueAPIClient> leagueApiClient = new();
            leagueApiClient.Setup(x => x.GetNameOfQueue(450).Result).Returns("HA");
            leagueApiClient.Setup(x => x.GetParsedListOfVersions(It.IsAny<List<string>>()).Result).Returns(new List<string>() { "12.2", "12.1" });


            FullRunner runner = new(leagueApiClient.Object, collector.Object, repo.Object, fileIO.Object, dateTimeProvider.Object, guidProvider.Object, excelPrinter.Object, logger, new Mock<IDdragonRepositoryUpdater>().Object);
            return runner;
        }
    }
}
