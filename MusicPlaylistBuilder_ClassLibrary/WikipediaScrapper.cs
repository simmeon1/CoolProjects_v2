using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Common_ClassLibrary;
using OpenQA.Selenium;

namespace MusicPlaylistBuilder_ClassLibrary
{
    internal class WikipediaScrapper
    {
        private readonly IWebDriver driver;
        private readonly IJavaScriptExecutor js;
        private readonly IDelayer delayer;

        public WikipediaScrapper(IWebDriver driver, IJavaScriptExecutor js, IDelayer delayer)
        {
            this.driver = driver;
            this.js = js;
            this.delayer = delayer;
        }
        
        public List<Dictionary<string, string>> GetSongsFromLinks(List<string> links)
        {
            List<Dictionary<string, string>> entries = new();
            AddEntriesFromLinks(links, entries);
            return entries;
        }

        private void AddEntriesFromLinks(List<string> links, List<Dictionary<string, string>> entries)
        {
            foreach (string link in links) AddEntriesFromLink(entries, link);
        }

        private void AddEntriesFromLink(List<Dictionary<string, string>> entries, string link)
        {
            List<Dictionary<string, string>> songs = GetSongsFromLink(link);
            entries.AddRange(songs);
        }

        private List<Dictionary<string, string>> GetSongsFromLink(string link)
        {
            driver.Navigate().GoToUrl(link);
            List<Dictionary<string, string>> songs = GetSongsFromPage();
            UpdateSongsWithLinkData(link, songs);
            return songs;
        }

        private List<Dictionary<string, string>> GetSongsFromPage()
        {
            IWebElement songTable = GetSongTable();
            return GetSongsFromTable(songTable);
        }

        private IWebElement GetSongTable()
        {
            ReadOnlyCollection<IWebElement> tables = GetPageTables();
            IWebElement songTable = GetBiggestTable(tables);
            return songTable;
        }

        private List<Dictionary<string, string>> GetSongsFromTable(IWebElement songTable)
        {
            ReadOnlyCollection<IWebElement> rows = GetRows(songTable);
            return GetSongsFromRows(rows);
        }

        private void UpdateSongsWithLinkData(string link, List<Dictionary<string, string>> songs)
        {
            string year = GetYearFromLink(link);
            AddYearToSongs(year, songs);
        }

        private void AddYearToSongs(string year, List<Dictionary<string, string>> songs)
        {
            foreach (Dictionary<string, string> song in songs) song.Add("Year", year);
        }

        private string GetYearFromLink(string link)
        {
            return Regex.Match(link, @"\d\d\d\d").Value;
        }

        private List<Dictionary<string, string>> GetSongsFromRows(ReadOnlyCollection<IWebElement> rows)
        {
            List<Dictionary<string, string>> entries = new();
            IWebElement headerRow = rows[0];

            ReadOnlyCollection<IWebElement> headerRowCells = GetRowCells(headerRow);
            Dictionary<int, string> headerCellNames = new();

            for (int i = 0; i < headerRowCells.Count; i++)
            {
                IWebElement headerRowCell = headerRowCells[i];
                headerCellNames.Add(i, GetInnerTextTrimmed(headerRowCell));
            }

            for (int i = 1; i < rows.Count; i++)
            {
                IWebElement row = rows[i];
                ReadOnlyCollection<IWebElement> rowCells = GetRowCells(row);

                Dictionary<string, string> entry = new();
                for (int j = 0; j < rowCells.Count; j++)
                {
                    IWebElement cell = rowCells[j];
                    long cellRowSpan = GetRowSpan(cell);
                    string cellText = GetInnerTextTrimmed(cell);
                    long countOfRowsToFix = cellRowSpan - 1;
                    if (cellRowSpan != 1) SetRowSpanToOne(cell);
                    for (int k = 1; k <= countOfRowsToFix; k++)
                    {
                        IWebElement rowToFix = rows[i + k];
                        try
                        {
                            InsertCellInRowAtPositionWithText(rowToFix, j, cellText);
                        }
                        catch (Exception e)
                        {
                            break;
                        }
                    }

                    if (rowCells.Count == headerRowCells.Count) entry.Add(headerCellNames[j], cellText);
                }
                if (entry.Count > 0) entries.Add(entry);
            }
            return entries;
        }

        private ReadOnlyCollection<IWebElement> GetPageTables()
        {
            return driver.FindElements(By.CssSelector("table"));
        }

        private void InsertCellInRowAtPositionWithText(IWebElement rowToFix, int position, string cellText)
        {
            ExecuteScript("arguments[0].insertCell(arguments[1]);arguments[0].cells[arguments[1]].innerText = arguments[2]", rowToFix, position, cellText);
        }

        private string GetInnerTextTrimmed(IWebElement el)
        {
            string str = (string)ExecuteScript("return arguments[0].innerText", el);
            str = str.Replace(Environment.NewLine, " ");
            return str.Trim();
        }

        private void SetRowSpanToOne(IWebElement cell)
        {
            ExecuteScript("arguments[0].rowSpan = 1", cell);
        }

        private long GetRowSpan(IWebElement cell)
        {
            return (long)ExecuteScript("return arguments[0].rowSpan", cell);
        }

        private ReadOnlyCollection<IWebElement> GetRowCells(IWebElement row)
        {
            return (ReadOnlyCollection<IWebElement>)ExecuteScript("return arguments[0].cells", row);
        }

        private ReadOnlyCollection<IWebElement> GetRows(IWebElement table)
        {
            return (ReadOnlyCollection<IWebElement>)ExecuteScript("return arguments[0].rows", table);
        }

        private object ExecuteScript(string script, params object[] args)
        {
            while (true)
            {
                try
                {
                    return js.ExecuteScript(script, args);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Only one usage of each socket address")) delayer.Sleep(120000);
                    else throw;
                }
            }
        }

        private IWebElement GetBiggestTable(ReadOnlyCollection<IWebElement> tables)
        {
            IWebElement biggestTable = null;
            long highestRowCount = 0;
            foreach (IWebElement table in tables)
            {
                long rowCount = GetRows(table).Count;
                if (rowCount >= highestRowCount)
                {
                    biggestTable = table;
                    highestRowCount = rowCount;
                }
            }
            return biggestTable;
        }
    }
}