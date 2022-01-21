using Common_ClassLibrary;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class DdragonRepositoryUpdater : IDdragonRepositoryUpdater
    {
        private IHttpClient HttpClient { get; set; }
        private IWebClient WebClient { get; set; }
        private IFileIO FileIO { get; set; }
        private ILogger Logger { get; set; }
        private string BaseName { get; set; }
        private string RepoPath { get; set; }
        public DdragonRepositoryUpdater(IHttpClient httpClient, IWebClient webClient, IFileIO fileIO, ILogger logger, string repoPath)
        {
            HttpClient = httpClient;
            WebClient = webClient;
            FileIO = fileIO;
            Logger = logger;
            RepoPath = repoPath;
        }

        public async Task GetLatestDdragonFiles()
        {
            JArray versionsJson = await GetLatestVersions();
            string latestVersion = versionsJson[0].ToString();
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
            await WebClient.DownloadFileTaskAsync(downloadLink, $"{baseNameWithTar}");
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
            catch (Exception ex)
            {
                Logger.Log($"There was an error with deleting the downloaded files ({BaseName}). Details:");
                Logger.Log(ex.ToString());
            }
        }

        private async Task<JArray> GetLatestVersions()
        {
            Logger.Log("Getting latest version...");
            HttpRequestMessage message = new(HttpMethod.Get, "https://ddragon.leagueoflegends.com/api/versions.json");
            HttpResponseMessage response = await HttpClient.SendRequest(message);
            string responseMessage = await response.Content.ReadAsStringAsync();
            JArray result = JArray.Parse(responseMessage);
            return result;
        }

        private void ExtractTGZ()
        {
            string baseNameWithTar = GetBaseNameWithTar();
            string baseNameWithFolder = GetBaseNameWithTargetFolder();
            Logger.Log($"Extracting {baseNameWithTar} to {baseNameWithFolder}...");
            Stream inStream = File.OpenRead($"{baseNameWithTar}");
            Stream gzipStream = new GZipInputStream(inStream);

            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents($"{baseNameWithFolder}", true);
            tarArchive.Close();

            gzipStream.Close();
            inStream.Close();
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

        public async Task<List<string>> GetParsedListOfVersions(List<string> unparsedVersions)
        {
            List<string> parsedVersions = new();
            JArray versionsJson = await GetLatestVersions();
            foreach (string unparsedVersion in unparsedVersions)
            {
                int unparsedVersionIndex = int.Parse(unparsedVersion);
                int unparsedVersionUpdatedIndex = unparsedVersionIndex * -1;
                string parsedVersion = versionsJson[unparsedVersionUpdatedIndex].ToString();
                parsedVersions.Add(parsedVersion);
            }
            return parsedVersions;
        }
    }
}
