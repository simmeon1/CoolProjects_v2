using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Common_ClassLibrary;
using OpenQA.Selenium;

namespace MusicPlaylistBuilder_ClassLibrary
{
    public class BillboardScrapper
    {
        private readonly IWebDriver driver;
        private readonly IJavaScriptExecutor js;
        private readonly IHttpClient http;
        private readonly IFileIO fileIo;
        private readonly ILogger logger;

        public BillboardScrapper(IWebDriver driver, IJavaScriptExecutor js, IHttpClient http, IFileIO fileIo, ILogger logger)
        {
            this.driver = driver;
            this.js = js;
            this.http = http;
            this.fileIo = fileIo;
            this.logger = logger;
        }

        public Dictionary<string, ScrappedSong> GetPages()
        {
            DateTime date = new(1958, 8, 2);
            Dictionary<string, ScrappedSong> entries = new();

            try
            {
                while (date.CompareTo(DateTime.Now) < 0)
                {
                    AddEntriesFromDate(date, entries);
                    date = date.AddDays(7);
                }
            }
            catch (Exception e)
            {
                logger.Log(e.ToString());
            }
            logger.Log("Scrapping finished.");
            return entries;
        }

        private void AddEntriesFromDate(DateTime date, Dictionary<string, ScrappedSong> entries)
        {
            string dateStr = date.ToString("yyyy-MM-dd");
            string singlesChartId = "hot-100";
            driver.Navigate().GoToUrl($"https://www.billboard.com/charts/{singlesChartId}/{dateStr}/");
            ReadOnlyCollection<object> trackRows = (ReadOnlyCollection<object>) js.ExecuteScript(
                @"
function getTrackRowTexts() {
    var result = [];
    var tracks = document.querySelectorAll('.o-chart-results-list-row-container');
    tracks.forEach(track => {
        result.push(track.innerText);
    });
    return result;
}
return getTrackRowTexts();"
            );

            for (int i = 0; i < trackRows.Count; i++)
            {
                object row = trackRows[i];
                string rowText = (string) row;
                rowText = rowText.Replace("\t", "-:-");
                rowText = rowText.Replace("\n", "-:-");
                rowText = rowText.Replace("-:--:-", "-:-");
                string[] rowPieces = rowText.Split("-:-");
                int diff = 7 - rowPieces.Length;

                string title = rowPieces[2 - diff];
                string artist = rowPieces[3 - diff];
                int peak = int.Parse(rowPieces[5 - diff]);
                int stay = int.Parse(rowPieces[6 - diff]);

                string trackId = artist + " " + title;
                bool containsEntry = entries.ContainsKey(trackId);
                ScrappedSong entry = containsEntry
                    ? entries[trackId]
                    : new ScrappedSong(title, artist, date);
                entry.SetHigherPeak(peak);
                entry.SetLongerStay(stay);
                if (!containsEntry)
                {
                    entries.Add(trackId, entry);
                    logger.Log($"Added entry {entry}");
                }
                else
                {
                    logger.Log($"Updated entry {entry}");
                }
            }
        }
    }
}