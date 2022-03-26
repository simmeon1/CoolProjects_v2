using Common_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace MusicPlaylistBuilder
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public async Task TestMethod1Async()
        {
            ChromeDriver driver = new();
            HttpClient client = new();

            List<int> chapters = new();
            for (int i = 1013; i < 1044; i++)
            {
                chapters.Add(i);
            }

            List<string> images = new();
            foreach (int chapter in chapters)
            {
                driver.Navigate().GoToUrl($"https://myonepiecemanga.com/manga/one-piece-chapter-{chapter}/");
                ReadOnlyCollection<IWebElement> lazySources = driver.FindElementsByCssSelector("[data-lazy-src]");
                foreach (IWebElement lazySource in lazySources)
                {
                    IWebElement parentElement = (IWebElement) driver.ExecuteScript(
                        "return arguments[0].parentElement",
                        lazySource
                    );

                    string link = parentElement.GetAttribute("href");
                    if (link == null) continue;
                    
                    string pageContent = await client.GetStringAsync(link);
                    string img = Regex.Match(pageContent, "image_src..href=\"(.*?)\".").Groups[1].Value;
                    images.Add(img);
                }
            }

            string serialized = images.SerializeObject(Formatting.Indented);
        }
    }
}