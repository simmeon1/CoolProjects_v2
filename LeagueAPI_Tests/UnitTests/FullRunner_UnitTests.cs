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
        public async Task FullRunner_ExpectedFileNames_MatchesNotProvided()
        {
            Parameters paramms = GetParams();
            List<string> result = await SetupFullRunner().DoFullRun(paramms.OutputDirectory, paramms.QueueId, paramms.AccountPuuid);
            Assert.IsTrue(result.Count == 4);
            Assert.IsTrue(result[0].Equals($"C:\\Matches_2020-02-02--00-00-00_someGuid.json"));
            Assert.IsTrue(result[1].Equals($"C:\\ItemSet_2020-02-02--00-00-00_someGuid.json"));
            Assert.IsTrue(result[2].Equals($"C:\\Stats_2020-02-02--00-00-00_someGuid.xlsx"));
            Assert.IsTrue(result[3].Equals($"C:\\Log_2020-02-02--00-00-00_someGuid.txt"));
        }

        [TestMethod]
        public async Task FullRunner_ExpectedFileNames_ExceptionDuringMatchCollection()
        {

            Parameters paramms = GetParams();
            List<string> result = await SetupFullRunner(throwExceptionOnMatchCollection: true).DoFullRun(paramms.OutputDirectory, paramms.QueueId, paramms.AccountPuuid);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Equals($"C:\\Log_2020-02-02--00-00-00_someGuid.txt"));
        }

        [TestMethod]
        public void FullRunner_ExpectedFileNames_MatchesProvided()
        {

            List<string> result = SetupFullRunner().DoFullRun(GetParams().OutputDirectory, "matchesPath");
            Assert.IsTrue(result.Count == 3);
            Assert.IsTrue(result[0].Equals($"C:\\ItemSet_2020-02-02--00-00-00_someGuid.json"));
            Assert.IsTrue(result[1].Equals($"C:\\Stats_2020-02-02--00-00-00_someGuid.xlsx"));
            Assert.IsTrue(result[2].Equals($"C:\\Log_2020-02-02--00-00-00_someGuid.txt"));
        }

        [TestMethod]
        public void FullRunner_ExpectedFileNames_ExceptionDuringMatchFileReading()
        {

            List<string> result = SetupFullRunner(throwExceptionOnMatchFileRead: true).DoFullRun(GetParams().OutputDirectory, "matchesPath");
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Equals($"C:\\Log_2020-02-02--00-00-00_someGuid.txt"));
        }

        private static Parameters GetParams()
        {
            return new()
            {
                AccountName = "ss",
                AccountPuuid = "ss",
                DdragonJsonFilesDirectoryPath = "ss",
                MaxCount = 1,
                OutputDirectory = "C:\\",
                QueueId = 1,
                TargetVersion = "ss",
                Token = "ss"
            };
        }

        private static FullRunner SetupFullRunner(bool throwExceptionOnMatchCollection = false, bool throwExceptionOnMatchFileRead = false)
        {
            Logger_Debug logger = new();

            Mock<IMatchCollector> collector = new();

            if (throwExceptionOnMatchCollection) collector.Setup(x => x.GetMatches(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()).Result).Throws(new Exception("ex"));
            else collector.Setup(x => x.GetMatches(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()).Result).Returns(new List<LeagueMatch>());

            Mock<IDDragonRepository> repo = new();
            Mock<IFileIO> fileIO = new();

            if (throwExceptionOnMatchFileRead) fileIO.Setup(x => x.ReadAllText(It.IsAny<string>())).Throws(new Exception("ex"));
            else fileIO.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("[]");

            Mock<IDateTimeProvider> dateTimeProvider = new();
            dateTimeProvider.Setup(x => x.Now()).Returns(new DateTime(2020, 2, 2));

            Mock<IGuidProvider> guidProvider = new();
            guidProvider.Setup(x => x.NewGuid()).Returns("someGuid");

            Mock<IExcelPrinter> excelPrinter = new();
            FullRunner runner = new(collector.Object, repo.Object, fileIO.Object, dateTimeProvider.Object, guidProvider.Object, excelPrinter.Object, logger);
            return runner;
        }
    }
}
