using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common_ClassLibrary;
using OpenQA.Selenium;

namespace MusicPlaylistBuilder_ClassLibrary
{
    public class OfficialChartsScrapper
    {
        private readonly IWebDriver driver;
        private readonly IJavaScriptExecutor js;
        private readonly IHttpClient http;
        private readonly IFileIO fileIo;
        private readonly ILogger logger;

        public OfficialChartsScrapper(IWebDriver driver, IJavaScriptExecutor js, IHttpClient http, IFileIO fileIo, ILogger logger)
        {
            this.driver = driver;
            this.js = js;
            this.http = http;
            this.fileIo = fileIo;
            this.logger = logger;
        }

        public async Task<Dictionary<string, OfficialChartsSongEntry>> GetPages()
        {
            DateTime date = new(1952, 11, 14);
            Dictionary<string, OfficialChartsSongEntry> entries = new();

            try
            {
                while (date.CompareTo(DateTime.Now) < 0)
                {
                    await AddEntriesFromDate(date, entries);
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

        private async Task AddEntriesFromDate(DateTime date, Dictionary<string, OfficialChartsSongEntry> entries)
        {
            string dateStr = date.ToString("yyyyMMdd");
            string singlesChartId = "7501";

            HttpRequestMessage request = new(
                HttpMethod.Get,
                $"https://www.officialcharts.com/charts/singles-chart/{dateStr}/{singlesChartId}/"
            );
            HttpResponseMessage response = await http.SendRequest(request);
            string responseText = await response.Content.ReadAsStringAsync();
            string tableHtml = Regex.Match(
                    responseText,
                    "<section class=.chart.>(.|\n)*?^</section>",
                    RegexOptions.Multiline
                )
                .Value;
            tableHtml = Regex.Replace(tableHtml, "img src=\".*?\"", "img src=\"\"");
            string filePath = "test.html";
            fileIo.WriteAllText(filePath, tableHtml);
            string fileUri = new Uri(Path.GetFullPath(filePath)).AbsoluteUri;
            driver.Navigate().GoToUrl(fileUri);

            ReadOnlyCollection<object> trackRows = (ReadOnlyCollection<object>) js.ExecuteScript(
                @"
function getTrackRowTexts() {
    var result = [];
    var tracks = document.querySelectorAll('.track');
    tracks.forEach(track => {
        result.push(track.parentElement.parentElement.innerText);
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

                string title = rowPieces[2];
                string artist = rowPieces[3];
                int peak = int.Parse(rowPieces[5]);
                int stay = int.Parse(rowPieces[6]);

                string trackId = artist + " " + title;
                bool containsEntry = entries.ContainsKey(trackId);
                OfficialChartsSongEntry entry = containsEntry
                    ? entries[trackId]
                    : new OfficialChartsSongEntry(title, artist, date);
                entry.SetPeak(peak);
                entry.SetStay(stay);
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

    public class OfficialChartsSongEntry
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public DateTime Date { get; set; }
        public int Peak { get; set; } = int.MaxValue;
        public int Stay { get; set; } = int.MinValue;

        public OfficialChartsSongEntry(string title, string artist, DateTime date)
        {
            Title = title;
            Artist = artist;
            Date = date;
        }

        public void SetPeak(int peak)
        {
            if (peak < Peak) Peak = peak;
        }

        public void SetStay(int stay)
        {
            if (stay > Stay) Stay = stay;
        }

        public override string ToString()
        {
            return $"{Artist} - {Title} - {Peak} - {Stay} - {Date}";
        }
    }
}