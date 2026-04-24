using System;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Common_ClassLibrary
{
    public static class ChromeDriverService
    {
        public static async Task GetLatestChromeDriver(
            ILogger logger,
            IHttpClient httpClient,
            string savePath,
            IFileIO fileIo,
            string? chromeVersion
        )
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
            var latestStableVersionDownloads =
                JObject.Parse(latestVersionsJson)["channels"]["Stable"]["downloads"]["chromedriver"];

            var downloadLink = "";
            foreach (var download in latestStableVersionDownloads)
            {
                if (download["platform"].ToString() == "win64")
                {
                    downloadLink = download["url"].ToString();
                    break;
                }
            }

            if (chromeVersion is not null)
            {
                downloadLink = Regex.Replace(downloadLink, @"\d+.\d+\.\d+\.\d+", chromeVersion);
            }

            WebClient webClient = new ();
            var downloadedFile = $"{savePath}\\chromedriver.zip";
            await webClient.DownloadFileTaskAsync(new Uri(downloadLink), downloadedFile);
            var destinationDirectoryName = $"{savePath}\\chromeDriverFolder";
            if (fileIo.DirectoryExists(destinationDirectoryName))
            {
                fileIo.DeleteFolder(destinationDirectoryName);
            }
            ZipFile.ExtractToDirectory(downloadedFile, destinationDirectoryName);
        }
    }
}