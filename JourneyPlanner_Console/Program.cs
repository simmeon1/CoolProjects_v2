using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.FlightConnectionsDotCom;
using JourneyPlanner_ClassLibrary.Workers;
using Newtonsoft.Json.Linq;

namespace JourneyPlanner_Console
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string parametersPath = "";
            foreach (string arg in args)
            {
                Match match = Regex.Match(arg, "parametersPath-(.*)");
                if (match.Success) parametersPath = match.Groups[1].Value;
            }
            Parameters parameters = (await File.ReadAllTextAsync(parametersPath)).DeserializeObject<Parameters>();
            
            RealHttpClient httpClient = new();
            Logger_Console logger = new();
            
            logger.Log("Getting latest chromedriver.");
            
            //Download latest web driver
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
            string savePath = parameters.FileSavePath;
            await webClient.DownloadFileTaskAsync(new Uri(downloadLink), $"{savePath}\\chromedriver.zip");
            if (Directory.Exists($"{savePath}\\chromeDriverFolder"))  Directory.Delete($"{savePath}\\chromeDriverFolder", true);
            ZipFile.ExtractToDirectory($"{savePath}\\chromedriver.zip", $"{savePath}\\chromeDriverFolder");
            
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("--log-level=3");
            chromeOptions.AddArgument("window-size=1280,800");
            if (parameters.Headless) chromeOptions.AddArgument("headless");
            ChromeDriver driver = new($"{savePath}\\chromeDriverFolder\\chromedriver-win64", chromeOptions);

            RealWebDriverWaitProvider webDriverWait = new(driver);
            FlightConnectionsDotComWorker worker = new(logger, driver, webDriverWait);

            FullRunner runner = new(
                driver,
                logger,
                webDriverWait,
                new RealDelayer(),
                httpClient,
                driver,
                fileIo: new RealFileIO(),
                dateTimeProvider: new RealDateTimeProvider(),
                printer: new ExcelPrinter(),
                airportCollector: new FlightConnectionsDotComWorkerAirportCollector(worker),
                airportPopulator: new FlightConnectionsDotComWorkerAirportPopulator(worker)
            );

            bool success = false;
            try
            {
                await runner.DoRun(parameters);
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception appeared during run. Details:");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (success || parameters.Headless) driver.Quit();
                Console.WriteLine("Run finished. Press any key to continue");
                Console.ReadKey();
            }
        }
    }
}
