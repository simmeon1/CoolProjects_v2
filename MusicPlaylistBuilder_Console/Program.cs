using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.RegularExpressions;
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
            RealFileIO fileIo = new();
            Logger_Console logger = new();
            RealHttpClient http = new();
            RealDelayer delayer = new();
            
            // GetSongsWithScrapper(fileIo, logger);
            //await GetSpotifyIds(fileIo, http, delayer);

            await NewMethod(fileIo, http, delayer);

            var x = 2;

            // Console.ReadKey();

        }

        private static async Task NewMethod(RealFileIO fileIo, RealHttpClient http, RealDelayer delayer)
        {
            List<(ScrappedSong, SpotifySong)> col = fileIo.ReadAllText("spotify_ids.json")
                .DeserializeObject<List<(ScrappedSong, SpotifySong)>>();

            Dictionary<ScrappedSong, SpotifySong> colParsed = new();
            foreach ((ScrappedSong, SpotifySong) pair in col)
            {
                // Match match = Regex.Match(pair.Item1., @"^(.*?) - (.*?) - (\d+) - (\d+) - (\d\d/\d\d/.*)");
                // ScrappedSong song = new(match.Groups[2].Value, match.Groups[1].Value, DateTime.Parse(match.Groups[5].Value));
                // song.SetHigherPeak(int.Parse(match.Groups[3].Value));
                // song.SetLongerStay(int.Parse(match.Groups[4].Value));

                colParsed.Add(pair.Item1, pair.Item2);
            }

            // Dictionary<ScrappedSong, SpotifySong> filtered = new();
            // foreach (KeyValuePair<ScrappedSong, SpotifySong> pair in colParsed)
            // {
            //     SpotifySong spotifySong = pair.Value;
            //     bool checkSong = pair.Key.Peak <= 20 && (spotifySong == null || spotifySong.ToString().Contains("araoke") || spotifySong.ToString().Contains("meritz"));
            //     
            //     // if (spotifySong != null && pair.Key.Peak <= 20 &&
            //     //     (spotifySong.ToString().Contains("araoke") || spotifySong.ToString().Contains("meritz")))
            //         // if (pair.Value == null && pair.Key.Peak <= 20)
            //     if (checkSong)
            //     {
            //         filtered.Add(pair.Key, spotifySong);
            //     }
            // }

            Dictionary<string, SpotifySong> spotifySongMap = new();
            Dictionary<string, List<ScrappedSong>> scrappedSongsBySpotifyId = new();
            foreach (KeyValuePair<ScrappedSong, SpotifySong> pair in colParsed)
            {
                SpotifySong spotifySong = pair.Value;
                if (spotifySong == null || spotifySong.ToString().Contains("araoke") || spotifySong.ToString().Contains("meritz")) continue;
                
                if (!spotifySongMap.ContainsKey(spotifySong.Id))
                {
                    spotifySongMap.Add(spotifySong.Id, spotifySong);
                }
                
                if (!scrappedSongsBySpotifyId.ContainsKey(spotifySong.Id))
                {
                    scrappedSongsBySpotifyId.Add(spotifySong.Id, new List<ScrappedSong> { pair.Key });
                }
                else
                {
                    scrappedSongsBySpotifyId[spotifySong.Id].Add(pair.Key);
                }
            }
            
            Dictionary<string, ScrappedSong> mergedSongsBySpotifyId = new();
            foreach (KeyValuePair<string, List<ScrappedSong>> pair in scrappedSongsBySpotifyId)
            {
                int highestPeak = int.MaxValue;
                int longestStay = int.MinValue;
                DateTime earliestRelease = DateTime.MaxValue;
                foreach (ScrappedSong scrappedSong in pair.Value)
                {
                    if (scrappedSong.Peak < highestPeak) highestPeak = scrappedSong.Peak;
                    if (scrappedSong.Stay > longestStay) longestStay = scrappedSong.Stay;
                    if (scrappedSong.Date.CompareTo(earliestRelease) < 0) earliestRelease = scrappedSong.Date;
                }

                string id = pair.Key;
                SpotifySong spotifySong = spotifySongMap[id];
                ScrappedSong song = new(spotifySong.Name, spotifySong.Artists[0], earliestRelease);
                song.SetHigherPeak(highestPeak);
                song.SetLongerStay(longestStay);
                mergedSongsBySpotifyId.Add(id, song);
            }
            
            //mergedSongsBySpotifyId.Where(x => x.Value.Peak <= 15 && x.Value.Stay >= 10 && x.Value.Date.Year >= 1975).Count()
            List<string> songsToAdd = new();
            foreach (KeyValuePair<string, ScrappedSong> pair in mergedSongsBySpotifyId)
            {
                ScrappedSong song = pair.Value;
                if (song.Peak <= 15 && song.Stay >= 10 && song.Date.Year >= 1975)
                {
                    songsToAdd.Add(pair.Key);
                }
            }
            
            SpotifyAPIClient client = await GetSpotifyApiClient(http, delayer);
            string playlistId = await client.CreatePlaylist("test4", await client.GetUserId());
            await client.AddSongsToPlaylist(playlistId, songsToAdd);
        }

        private static void GetSongsWithScrapper(RealFileIO fileIo, Logger_Console logger)
        {
            ChromeOptions chromeOptions = new();
            // chromeOptions.AddArgument("headless");
            ChromeDriver driver = new(chromeOptions);

            BillboardScrapper scrapper = new(driver, driver, new RealHttpClient(), fileIo, logger);
            Dictionary<string, ScrappedSong> entries = scrapper.GetPages();

            fileIo.WriteAllText("results.json", entries.SerializeObject(Formatting.Indented));
            fileIo.WriteAllText("log.txt", logger.GetContent());
        }

        private static async Task GetSpotifyIds(RealFileIO fileIo, RealHttpClient http, RealDelayer delayer)
        {
            string ukEntriesPath =
                @"C:\Users\simme\source\repos\CoolProjects_v2\MusicPlaylistBuilder_Console\bin\Debug\net5.0\officialcharts_full_results\results.json";
            string usEntriesPath =
                @"C:\Users\simme\source\repos\CoolProjects_v2\MusicPlaylistBuilder_Console\bin\Debug\net5.0\billboardcharts_full_results\results.json";

            Dictionary<string, ScrappedSong> ukEntries =
                fileIo.ReadAllText(ukEntriesPath).DeserializeObject<Dictionary<string, ScrappedSong>>();
            Dictionary<string, ScrappedSong> usEntries =
                fileIo.ReadAllText(usEntriesPath).DeserializeObject<Dictionary<string, ScrappedSong>>();

            List<ScrappedSong> allSongs = new();
            allSongs.AddRange(usEntries.Values.ToList());
            allSongs.AddRange(ukEntries.Values.ToList());

            SpotifyAPIClient client = await GetSpotifyApiClient(http, delayer);

            List<(ScrappedSong, SpotifySong)> spotifySongs = new();
            int count = allSongs.Count;
            for (int i = 0; i < count; i++)
            {
                ScrappedSong scrappedSong = allSongs[i];
                (ScrappedSong, SpotifySong) tuple = new(
                    scrappedSong,
                    await client.GetFirstSearchResultEntry(GetSearchTerms(scrappedSong))
                );
                spotifySongs.Add(tuple);
                Debug.WriteLine(i + "/" + count);
            }

            string json = spotifySongs.SerializeObject(Formatting.Indented);
            fileIo.WriteAllText("spotify_ids.json", json);
        }

        private static async Task<SpotifyAPIClient> GetSpotifyApiClient(RealHttpClient http, RealDelayer delayer)
        {
            SpotifyCredentials credentials = (await File
                    .ReadAllTextAsync(@"C:\Users\simme\source\repos\CoolProjects_v2\MusicPlaylistBuilder\credentials.json"))
                .DeserializeObject<SpotifyCredentials>();
            SpotifyAPIClient client = new(http, delayer, credentials);
            return client;
        }

        private static string GetSearchTerms(ScrappedSong scrappedSong)
        {
            string artist = scrappedSong.Artist.ToLower();
            string title = scrappedSong.Title.ToLower();
            List<string> trails = new()
            {
                "/", " ft ", " featuring ", " pts ", @"\(", "{", " with ", " vs ", " v ", " starring ", " & ", " and ",
                " a duet ", " duet ", " part ", " pt ", " x "
            };
            
            foreach (string trail in trails)
            {
                artist = Regex.Replace(artist, $"{trail}.*", "");
                title = Regex.Replace(title, $"{trail}.*", "");
            }
            return artist.Trim() + " " + title.Trim();
        }
        

        private static void WriteToClipboard(string str)
        {
            File.WriteAllText("clipboard.json", str);
        }
    }
}