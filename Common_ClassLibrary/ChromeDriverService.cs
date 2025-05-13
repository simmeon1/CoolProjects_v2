using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Common_ClassLibrary
{
    public static class ChromeDriverService
    {
        public static async Task GetLatestChromeDriver(ILogger logger, IHttpClient httpClient, string savePath, IFileIO fileIo)
        {
            //Download latest web driver

            // var x = await (await httpClient.GetAsync(
            //     "https://raw.githubusercontent.com/mhollingshead/billboard-hot-100/main/all.json"
            // )).Content.ReadAsStringAsync();
            // File.WriteAllText($"billboard_new.json", x);
            
            logger.Log("Getting latest chromedriver.");
            var latestVersionsJson = await (await httpClient.GetAsync(
                "https://googlechromelabs.github.io/chrome-for-testing/last-known-good-versions-with-downloads.json"
            )).Content.ReadAsStringAsync();
            JToken latestStableVersionDownloads = JObject.Parse(latestVersionsJson)["channels"]["Stable"]["downloads"]["chromedriver"];
            string downloadLink = "";
            foreach (JToken download in latestStableVersionDownloads)
            {
                if (download["platform"].ToString() == "win64")
                {
                    downloadLink = download["url"].ToString();
                    break;
                }
            }
            
            WebClient webClient = new();
            var downloadedFile = $"{savePath}\\chromedriver.zip";
            await webClient.DownloadFileTaskAsync(new Uri(downloadLink), downloadedFile);
            var destinationDirectoryName = $"{savePath}\\chromeDriverFolder";
            if (fileIo.DirectoryExists(destinationDirectoryName))  fileIo.DeleteFolder(destinationDirectoryName);
            ZipFile.ExtractToDirectory(downloadedFile, destinationDirectoryName);
        }
    }
}