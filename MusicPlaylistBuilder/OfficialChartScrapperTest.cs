using Common_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicPlaylistBuilder_ClassLibrary;

namespace MusicPlaylistBuilder
{
    [TestClass]
    public class OfficialChartScrapperTest
    {
        [TestMethod]
        public async Task TestMethod1Async()
        {
            ChromeDriver driver = new();
            RealFileIO fileIo = new();
            OfficialChartsScrapper scrapper = new(driver, driver, new RealHttpClient(), fileIo, new Logger_Console());
            Dictionary<string, OfficialChartsSongEntry> entries = await scrapper.GetPages();
            fileIo.WriteAllText("results.json", entries.SerializeObject(Formatting.Indented));
        }
    }
}