using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common_ClassLibrary;
using MusicPlaylistBuilder_ClassLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;

namespace MusicPlaylistBuilder_Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("headless");
            ChromeDriver driver = new(chromeOptions);
            RealFileIO fileIo = new();
            Logger_Console logger = new();
            OfficialChartsScrapper scrapper = new(driver, driver, new RealHttpClient(), fileIo, logger);
            Dictionary<string, OfficialChartsSongEntry> entries = await scrapper.GetPages();
            fileIo.WriteAllText("results.json", entries.SerializeObject(Formatting.Indented));
            fileIo.WriteAllText("log.txt", logger.GetContent());
            Console.ReadKey();
        }
    }
}