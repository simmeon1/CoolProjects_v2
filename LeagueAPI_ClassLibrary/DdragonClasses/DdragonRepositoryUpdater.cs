using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class DdragonRepositoryUpdater : IDdragonRepositoryUpdater
    {
        private ILeagueAPIClient LeagueAPIClient { get; set; }
        private IWebClient WebClient { get; set; }
        private IFileIO FileIO { get; set; }
        private ILogger Logger { get; set; }
        private IArchiveExtractor ArchiveExtractor { get; set; }

        private string BaseName { get; set; }
        private string RepoPath { get; set; }
        public DdragonRepositoryUpdater(ILeagueAPIClient leagueAPIClient, IWebClient webClient, IFileIO fileIO, ILogger logger, IArchiveExtractor archiveExtractor, string repoPath)
        {
            LeagueAPIClient = leagueAPIClient;
            WebClient = webClient;
            FileIO = fileIO;
            Logger = logger;
            RepoPath = repoPath;
            ArchiveExtractor = archiveExtractor;
        }

        public async Task GetLatestDdragonFiles()
        {
            Logger.Log("Getting latest version...");
            List<string> versionsJson = await GetLatestVersions();
            string latestVersion = versionsJson[0];
            Logger.Log($"Latest versions retrieved ({latestVersion})");
            BaseName = $"dragontail-{latestVersion}";
            CleanUpFiles();
            await Download();
            ExtractTGZ();
            CopyFiles(latestVersion);
            CleanUpFiles();
        }

        private void CopyFiles(string latestVersion)
        {
            Logger.Log($"Copying files...");
            string downloadedJsonsPath = $@"{GetBaseNameWithTargetFolder()}\{latestVersion}\data\en_US\";
            UpdateJson(downloadedJsonsPath, "champion.json");
            UpdateJson(downloadedJsonsPath, "item.json");
            UpdateJson(downloadedJsonsPath, "runesReforged.json");
            UpdateJson(downloadedJsonsPath, "summoner.json");
            Logger.Log($"Copied files.");
        }

        private void UpdateJson(string downloadedJsonsPath, string jsonToUpdate)
        {
            FileIO.Copy(Path.Combine(downloadedJsonsPath, jsonToUpdate), Path.Combine(RepoPath, jsonToUpdate), true);
        }

        private async Task Download()
        {
            string baseNameWithTar = GetBaseNameWithTar();
            string downloadLink = $"https://ddragon.leagueoflegends.com/cdn/{baseNameWithTar}";
            Logger.Log($"Downloading {downloadLink}...");
            await WebClient.DownloadFileTaskAsync(downloadLink, baseNameWithTar);
            Logger.Log($"Downloaded {downloadLink}.");
        }

        private void CleanUpFiles()
        {
            try
            {
                Logger.Log($"Deleting downloaded files...");
                FileIO.DeleteFolder(GetBaseNameWithTargetFolder());
                FileIO.DeleteFile(GetBaseNameWithTar());
                Logger.Log($"Deleted downloaded files.");
            }
            catch (Exception)
            {
                Logger.Log($"Failed deleting the downloaded files ({BaseName}). They might be deleted already.");
            }
        }

        private async Task<List<string>> GetLatestVersions()
        {
            return await LeagueAPIClient.GetLatestVersions();
        }

        private void ExtractTGZ()
        {
            string baseNameWithTar = GetBaseNameWithTar();
            string baseNameWithFolder = GetBaseNameWithTargetFolder();
            Logger.Log($"Extracting {baseNameWithTar} to {baseNameWithFolder}...");
            ArchiveExtractor.ExtractTar(baseNameWithTar, baseNameWithFolder);
            Logger.Log($"Extracted {baseNameWithTar} to {baseNameWithFolder}.");
        }

        private string GetBaseNameWithTar()
        {
            return $"{BaseName}.tgz";
        }
        
        private string GetBaseNameWithTargetFolder()
        {
            return $"{BaseName}-extracted";
        }
    }
}
