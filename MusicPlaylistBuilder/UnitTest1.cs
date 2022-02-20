using Common_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MusicPlaylistBuilder
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1Async()
        {
            List<string> usLinks = new()
            {
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1958",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1959",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1960",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1961",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1962",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1963",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1964",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1965",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1966",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1967",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1968",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1969",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1970",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1971",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1972",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1973",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1974",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1975",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1976",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1977",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1978",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1979",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1980",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1981",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1982",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1983",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1984",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1985",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1986",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1987",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1988",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1989",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1990",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1991",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1992",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1993",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1994",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1995",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1996",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1997",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1998",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_1999",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2000",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2001",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2002",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2003",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2004",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2005",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2006",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2007",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2008",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2009",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2010",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2011",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2012",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2013",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2014",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2015",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2016",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2017",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2018",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2019",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2020",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2021",
                "https://en.wikipedia.org/wiki/List_of_Billboard_Hot_100_top-ten_singles_in_2022"
            };
            List<string> ukLinks = new()
            {
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1952",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1953",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1954",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1955",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1956",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1957",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1958",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1959",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1960",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1961",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1962",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1963",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1964",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1965",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1966",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1967",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1968",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1969",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1970",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1971",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1972",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1973",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1974",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1975",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1976",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1977",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1978",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1979",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1980",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1981",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1982",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1983",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1984",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1985",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1986",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1987",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1988",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1989",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1990",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1991",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1992",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1993",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1994",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1995",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1996",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1997",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1998",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_1999",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2000",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2001",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2002",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2003",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2004",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2005",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2006",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2007",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2008",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2009",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2010",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2011",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2012",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2013",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2014",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2015",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2016",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2017",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2018",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2019",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2020",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2021",
                "https://en.wikipedia.org/wiki/List_of_UK_top-ten_singles_in_2022"
            };
            RealDelayer delayer = new();

            //ChromeDriver driver = new();
            //WikipediaScrapper scrapper = new(driver, driver, delayer);
            //List<Dictionary<string, string>> songEntries = scrapper.GetSongsFromLinks(ukLinks);
            //string x = songEntries.SerializeObject(Formatting.Indented);

            List<Dictionary<string, string>> ukSongEntries = File.ReadAllText(@"C:\Users\simme\OneDrive\Desktop\music_uk_results.json").DeserializeObject<List<Dictionary<string, string>>>();
            Dictionary<string, string> ukSongPropertyMappings = GetUkSinglesTopTenChartMappings();
            ukSongEntries = GetSongsWithRemappedProperties(ukSongEntries, ukSongPropertyMappings);
            List<SongCLS> ukSongs = GetSongObjectsFromEntries(ukSongEntries);

            List<Dictionary<string, string>> usSongEntries = File.ReadAllText(@"C:\Users\simme\OneDrive\Desktop\music_us_results.json").DeserializeObject<List<Dictionary<string, string>>>();
            Dictionary<string, string> usSongPropertyMappings = GetBillboardHot100TopTenChartMappings();
            usSongEntries = GetSongsWithRemappedProperties(usSongEntries, usSongPropertyMappings);
            List<SongCLS> usSongs = GetSongObjectsFromEntries(usSongEntries);

            SpotifyCredentials credentials = File.ReadAllText(@"C:\Users\simme\source\repos\CoolProjects_v2\SpotifyAPI_Tests\credentials.json").DeserializeObject<SpotifyCredentials>();
            SpotifyAPIClient client = new(new RealHttpClient(), delayer, credentials);

            HashSet<string> ukSearchTerms = GetSearchTerms(ukSongs);
            HashSet<string> usSearchTerms = GetSearchTerms(usSongs);
            HashSet<string> combinedSearchTerms = MergeSearchTerms(new List<HashSet<string>>() { ukSearchTerms, usSearchTerms });
            Dictionary<string, string> termsAndSpotifyIds = await GetSpotifyIds(combinedSearchTerms, client);

            //string songId = await client.GetIdOfFirstResultOfSearch("dio holy diver");
            //string userId = await client.GetUserId();
            //string playlistId = await client.CreatePlaylist("test1", userId);
            //await client.AddSongsToPlaylist(playlistId, new() { songId });


            //Dictionary<SongCLS, string> songSearchTerms = GetSearchTermsForSongs(songs);
            Dictionary<SongCLS, double> songScores = AssignScoreToSongs2(ukSongs, 10);
            List<SongCLS> orderedSongs = ukSongs.OrderByDescending(s => songScores[s]).ToList();
            string a = orderedSongs.SerializeObject(Formatting.Indented);
            string b = orderedSongs.Select(s => s.ToString() + " - " + songScores[s]).ToList().SerializeObject(Formatting.Indented);
        }

        private async Task<Dictionary<string, string>> GetSpotifyIds(HashSet<string> terms, SpotifyAPIClient client)
        {
            Dictionary<string, string> result = new();

            int counter = 0;
            foreach (string term in terms)
            {
                result.Add(term, await client.GetIdOfFirstResultOfSearch(term));
                Debug.WriteLine($"{++counter}/{terms.Count}");
            }
            return result;
        }

        private HashSet<string> MergeSearchTerms(List<HashSet<string>> termSets)
        {
            HashSet<string> result = new();
            foreach (HashSet<string> set in termSets)
            {
                foreach (string terms in set)
                {
                    result.Add(terms);
                }
            }
            return result;
        }

        private static HashSet<string> GetSearchTerms(List<SongCLS> songs)
        {
            HashSet<string> searchTerms = new();
            foreach (SongCLS song in songs) searchTerms.Add(song.GetSearchTerms());
            return searchTerms;
        }

        private List<SongCLS> GetSongObjectsFromEntries(List<Dictionary<string, string>> songsEntries)
        {
            List<SongCLS> results = new();
            foreach (Dictionary<string, string> entry in songsEntries)
            {

                string artist = entry["Artist"];
                artist = CleanUpArtist(artist);

                string songName = entry["Song"];
                songName = CleanUpSong(songName);

                SongCLS song = new();
                song.Artist = artist;
                song.Song = songName;
                song.Year = int.Parse(entry["Year"]);
                song.Peak = int.Parse(Regex.Replace(entry["Peak"], @"TBA", "1"));
                int stay = int.Parse(Regex.Replace(entry["Weeks in top ten"], @"(\d+).*", "$1"));
                song.WeeksInTopTen = stay;
                results.Add(song);
            }
            return results;
        }

        private Dictionary<SongCLS, double> AssignScoreToSongs2(List<SongCLS> songs, int yearDivider)
        {
            Dictionary<SongCLS, double> result = new();
            Dictionary<int, List<int>> yearAndWeekStay = GetWeekStaysOfSongsGroupedByYear(songs, yearDivider);
            Dictionary<int, List<int>> yearAndWeekStaysOrdered = new();
            foreach (KeyValuePair<int, List<int>> kvp in yearAndWeekStay)
            {
                yearAndWeekStaysOrdered.Add(kvp.Key, kvp.Value.OrderByDescending(s => s).Distinct().ToList());
            }

            foreach (SongCLS song in songs)
            {
                int year = song.Year / yearDivider;
                int peak = song.Peak;
                int stay = song.WeeksInTopTen;
                double peakScore = ((double)peak - 5) / 5 * 100 * -1;
                List<int> yearValues = yearAndWeekStaysOrdered[year];
                int yearStayingPosition = yearValues.IndexOf(stay) + 1;
                double stayScore = (yearStayingPosition - ((double)yearValues.Count / 2)) / yearValues.Count * 100 * -2;
                result.Add(song, (stayScore + peakScore));
            }
            return result;
        }

        private static Dictionary<int, List<int>> GetWeekStaysOfSongsGroupedByYear(List<SongCLS> songs, int yearDivider)
        {
            Dictionary<int, List<int>> yearAndWeekStay = new();
            foreach (SongCLS song in songs)
            {
                int year = song.Year / yearDivider;
                int stay = song.WeeksInTopTen;
                if (yearAndWeekStay.ContainsKey(year)) yearAndWeekStay[year].Add(stay);
                else yearAndWeekStay.Add(year, new List<int>() { stay });
            }
            return yearAndWeekStay;
        }

        private string CleanUpSong(string songName)
        {
            string result = songName;
            result = Regex.Replace(result, "^\"(.*?)\".*", "$1");
            result = RemoveCharsAndTrim(result);
            return result;
        }

        private string CleanUpArtist(string artist)
        {
            string result = artist;
            List<string> joiningWords = new() { "and", "featuring", "presents", "presents", "with" };
            foreach (string word in joiningWords) result = Regex.Replace(result, "(.*)" + " " + word + ".*", "$1");
            result = Regex.Replace(result, @"([a-z])\d$", "$1");
            result = RemoveCharsAndTrim(result);
            return result;
        }

        private static string RemoveCharsAndTrim(string result)
        {
            List<string> toReplaceWithSpace = new() { "-", "�", ":", "+", "\"", ",", "!", "?", ".", "'", "(", ")", "*", "/", "\\", "&" };
            foreach (string ch in toReplaceWithSpace) result = result.Replace(ch, " ");
            result = Regex.Replace(result, "\\s+", " ");
            return result.Trim();
        }

        private static Dictionary<string, string> GetBillboardHot100TopTenChartMappings()
        {
            Dictionary<string, string> songPropertyMappings = new();
            songPropertyMappings.Add("Artist(s)", "Artist");
            songPropertyMappings.Add("Artist", "Artist");
            songPropertyMappings.Add("Single", "Song");
            songPropertyMappings.Add("Year", "Year");
            songPropertyMappings.Add("Peak", "Peak");
            songPropertyMappings.Add("Weeks in top ten", "Weeks in top ten");
            return songPropertyMappings;
        }
        
        private static Dictionary<string, string> GetUkSinglesTopTenChartMappings()
        {
            Dictionary<string, string> songPropertyMappings = new();
            songPropertyMappings.Add("Artist", "Artist");
            songPropertyMappings.Add("Single", "Song");
            songPropertyMappings.Add("Year", "Year");
            songPropertyMappings.Add("Peak", "Peak");
            songPropertyMappings.Add("Weeks in top 10", "Weeks in top ten");
            return songPropertyMappings;
        }

        private List<Dictionary<string, string>> GetSongsWithRemappedProperties(List<Dictionary<string, string>> songs, Dictionary<string, string> songPropertyMappings)
        {
            List<Dictionary<string, string>> updatedSongs = new();
            foreach (Dictionary<string, string> song in songs)
            {
                Dictionary<string, string> updatedSong = new();
                foreach (KeyValuePair<string, string> mapping in songPropertyMappings)
                {
                    string existingPropertyName = mapping.Key;
                    string newPropertyName = mapping.Value;

                    string value = song.ContainsKey(existingPropertyName) ? song[existingPropertyName] : "";
                    if (!value.IsNullOrEmpty()) updatedSong.Add(newPropertyName, value);
                }
                updatedSongs.Add(updatedSong);
            }
            return updatedSongs;
        }
    }
}
