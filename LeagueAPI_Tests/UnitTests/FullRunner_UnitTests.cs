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
            Mock<IMatchCollector> collector = new();
            collector.Setup(x => x.GetMatches(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()).Result).Returns(new List<LeagueMatch>());

            Mock<IDDragonRepository> repo = new();
            Mock<IFileIO> fileIO = new();

            Mock<IDateTimeProvider> dateTimeProvider = new();
            dateTimeProvider.Setup(x => x.Now()).Returns(new DateTime(2020, 2, 2));

            Mock<IGuidProvider> guidProvider = new();
            guidProvider.Setup(x => x.NewGuid()).Returns("someGuid");

            Mock<IExcelPrinter> excelPrinter = new();
            FullRunner runner = new(collector.Object, repo.Object, fileIO.Object, dateTimeProvider.Object, guidProvider.Object, excelPrinter.Object);

            Parameters paramms = new()
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

            List<string> result = await runner.DoFullRun(paramms.OutputDirectory, paramms.QueueId, paramms.AccountPuuid);
            Assert.IsTrue(result.Count == 3);
            Assert.IsTrue(result[0].Equals($"C:\\Matches_2020-02-02--00-00-00_someGuid.json"));
            Assert.IsTrue(result[1].Equals($"C:\\ItemSet_2020-02-02--00-00-00_someGuid.json"));
            Assert.IsTrue(result[2].Equals($"C:\\Stats_2020-02-02--00-00-00_someGuid.xlsx"));
        }

        [TestMethod]
        public void FullRunner_ExpectedFileNames_MatchesProvided()
        {
            Mock<IMatchCollector> collector = new();
            collector.Setup(x => x.GetMatches(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()).Result).Returns(new List<LeagueMatch>());

            Mock<IDDragonRepository> repo = new();
            Mock<IFileIO> fileIO = new();
            fileIO.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("[]");

            Mock<IDateTimeProvider> dateTimeProvider = new();
            dateTimeProvider.Setup(x => x.Now()).Returns(new DateTime(2020, 2, 2));

            Mock<IGuidProvider> guidProvider = new();
            guidProvider.Setup(x => x.NewGuid()).Returns("someGuid");

            Mock<IExcelPrinter> excelPrinter = new();
            FullRunner runner = new(collector.Object, repo.Object, fileIO.Object, dateTimeProvider.Object, guidProvider.Object, excelPrinter.Object);

            Parameters paramms = new()
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

            List<string> result = runner.DoFullRun(paramms.OutputDirectory, "matchesPath");
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[0].Equals($"C:\\ItemSet_2020-02-02--00-00-00_someGuid.json"));
            Assert.IsTrue(result[1].Equals($"C:\\Stats_2020-02-02--00-00-00_someGuid.xlsx"));
        }
    }
}
