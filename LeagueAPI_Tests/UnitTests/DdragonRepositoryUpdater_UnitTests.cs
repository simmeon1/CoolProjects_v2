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
    public class DdragonRepositoryUpdater_UnitTests
    {
        [TestMethod]
        public async Task FunctionsWorksAsExpected()
        {
            string folderName = "dragontail-12.2-extracted";
            string archiveName = "dragontail-12.2.tgz";
            Mock<IWebClient> web = new();
            Mock<IFileIO> fileIO = new();
            Mock<IArchiveExtractor> extractor = new();
            Mock<ILeagueAPIClient> leagueAPI = new();
            leagueAPI.Setup(x => x.GetLatestVersions().Result).Returns(new List<string>() { "12.2", "12.1" });

            fileIO.SetupSequence(x => x.DeleteFolder(It.IsAny<string>())).Pass().Throws(new Exception());

            DdragonRepositoryUpdater updater = new(
                leagueAPIClient: leagueAPI.Object,
                webClient: web.Object,
                fileIO: fileIO.Object,
                logger: new Mock<ILogger>().Object,
                archiveExtractor: extractor.Object,
                repoPath: "repoPath"
            );

            await updater.GetLatestDdragonFiles();

            fileIO.Verify(x => x.DeleteFolder(It.IsAny<string>()), Times.Exactly(2));
            fileIO.Verify(x => x.DeleteFolder(folderName), Times.Exactly(2));

            fileIO.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Exactly(1));
            fileIO.Verify(x => x.DeleteFile(archiveName), Times.Exactly(1));

            web.Verify(x => x.DownloadFileTaskAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            web.Verify(x => x.DownloadFileTaskAsync($"https://ddragon.leagueoflegends.com/cdn/{archiveName}", archiveName), Times.Once());
            
            extractor.Verify(x => x.ExtractTar(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            extractor.Verify(x => x.ExtractTar(archiveName, folderName), Times.Once());

            fileIO.Verify(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Exactly(4));
            fileIO.Verify(x => x.Copy($@"{folderName}\12.2\data\en_US\champion.json", @"repoPath\champion.json", true), Times.Once());
            fileIO.Verify(x => x.Copy($@"{folderName}\12.2\data\en_US\item.json", @"repoPath\item.json", true), Times.Once());
            fileIO.Verify(x => x.Copy($@"{folderName}\12.2\data\en_US\runesReforged.json", @"repoPath\runesReforged.json", true), Times.Once());
            fileIO.Verify(x => x.Copy($@"{folderName}\12.2\data\en_US\summoner.json", @"repoPath\summoner.json", true), Times.Once());
        }
    }
}
