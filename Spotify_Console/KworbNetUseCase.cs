using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Common_ClassLibrary;
using Spotify_ClassLibrary;

namespace Spotify_Console;

public class KworbNetUseCase(
    IFileIO fileIo,
    ILogger logger,
    IDelayer delayer,
    SpotifyClient spotifyClient,
    SpotifyClientUseCase spotifyClientUseCase,
    IWebDriverWrapper chromeDriver,
    IHttpClient http
)
{
    public async Task DoWork(string jsonPath)
    {
        string GetKworbUrl(string str) => "https://kworb.net/spotify/" + str;

        async Task GoToLinkTable(string link)
        {
            var response = await http.GetAsync(link);
            var responseContent = await response.Content.ReadAsStringAsync();
            var table = Regex.Match(responseContent, @"(<table class="".*sortable""(.|\n)*?</table>)").Groups[1].Value;
            fileIo.WriteAllText("temp.html", table);
            chromeDriver.GoToUrl(
                "file:///C:/Users/simme/source/repos/CoolProjects_v2/Spotify_Console/bin/Debug/net9.0/temp.html"
            );
        }

        for (int i = 0; i < 10; i++)
        {
            var listenersId = "listeners" + (i == 0 ? "" : i + 1);
            var store = jsonPath + "\\kworb" + listenersId + ".json";
            var results = new Dictionary<string, long>();
            logger.Log(listenersId);
            
            await GoToLinkTable(GetKworbUrl(listenersId  + ".html"));
            var artistLinks = (ReadOnlyCollection<object>) chromeDriver.ExecuteScript(
                """
                return [...document.querySelectorAll('a')].map(a => a.href.replace(/^.*artist\//i, ""));
                """
            );
            // var results = Newtonsoft.Json.JsonSerializer.Deserialize<Dictionary<string, long>>(store)!;
            // var results = JsonSerializer.Deserialize<Dictionary<string, long>>(fileIo.ReadAllText(store))!;
            // await spotifyClientUseCase.GetSongs(results.Keys.ToList(), jsonPath);

            for (int j = 0; j < artistLinks.Count; j++)
            {
                logger.Log($"Artist link {j}");
                object artistLink = artistLinks[j];
                await GoToLinkTable(GetKworbUrl("artist/" + (string) artistLink));
                var map = (Dictionary<string, object>) chromeDriver.ExecuteScript(
                    """
                    var result = {};
                    var rows = document.querySelectorAll('tbody tr');
                    for (var row of rows) {
                        var cells = row.cells;
                        var spotifyId = cells[0].querySelector('a').href.replace("https://open.spotify.com/track/", "");
                        var streams = parseInt(cells[1].textContent.replaceAll(",", ""));
                        result[spotifyId] = Math.max(result[spotifyId] || 0, streams);
                    }
                    return result;
                    """
                );

                foreach ((string key, object value) in map)
                {
                    var v = (long) value;
                    if (!results.TryAdd(key, v))
                    {
                        results[key] = Math.Max(v, results[key]);
                    }
                }
            }
            fileIo.WriteAllText(store, results.SerializeObject());
        }

        // var results = JsonSerializer.Deserialize<Dictionary<string, long>>(fileIo.ReadAllText(jsonPath + "\\kworb.json"))!;

        // var results = new Dictionary<string, long>();
        // foreach (var link in links)
        // {
        //     var response = await http.GetAsync(link);
        //     var responseContent = await response.Content.ReadAsStringAsync();
        //     fileIo.WriteAllText("temp.html", responseContent);
        //     chromeDriver.GoToUrl("file:///C:/Users/simme/source/repos/CoolProjects_v2/Spotify_Console/bin/Debug/net9.0/temp.html");
        //     // chromeDriver.GoToUrl(link);
        //     var map = (Dictionary<string, object>)chromeDriver.ExecuteScript(
        //         """
        //         var result = {};
        //         var rows = document.querySelectorAll('tbody tr');
        //         for (var row of rows) {
        //             var cells = row.cells;
        //             var artistSong = cells[0].textContent;
        //             var streams = parseInt(cells[1].textContent.replaceAll(",", ""));
        //             result[artistSong] = Math.max(result[artistSong] || 0, streams);
        //         }
        //         return result;
        //         """
        //     );
        //
        //     foreach ((string key, object value) in map)
        //     {
        //         var v = (long) value;
        //         if (!results.TryAdd(key, v))
        //         {
        //             results[key] = Math.Max(v, results[key]);
        //         }
        //     }
        // }
        // fileIo.WriteAllText(jsonPath + "/kwob.json", results.SerializeObject());
        // var x = 1;

        // var dumbedResults = new Dictionary<ArtistSong, long>(new ArtistSongEqualityComparer());
        // foreach (var pair in results)
        // {
        //     var artistSong = new ArtistSong(
        //         SpotifyHelper.CleanText(pair.Key.Split(" - ", StringSplitOptions.RemoveEmptyEntries)[0], true),
        //         SpotifyHelper.CleanText(pair.Key.Split(" - ", StringSplitOptions.RemoveEmptyEntries)[1], false)
        //     );
        //     if (dumbedResults.ContainsKey(artistSong))
        //     {
        //         dumbedResults[artistSong] = Math.Max(dumbedResults[artistSong], pair.Value);
        //     }
        //     else
        //     {
        //         dumbedResults.Add(artistSong, pair.Value);
        //     }
        // }
        //
        // var trackMap = (await spotifyClientUseCase.GetSongs(dumbedResults.Select(x => x.Key).ToList(), jsonPath, false))
        //     .Where(x => x.Value != null)
        //     .Select(x => new KeyValuePair<string,TrackObject>(x.Key, x.Value!)) 
        //     .ToList();
        //
        // var artists =
        //     await spotifyClientUseCase.GetArtists(
        //         trackMap.Select(x => x.Value.artists.First().id).ToList(), jsonPath
        //     );
        //
        // var usContents =
        //     // JsonSerializer.Deserialize<List<BillboardList>>(fileIo.ReadAllText(jsonPath + "\\billboard_all_mhollingshead.json"))!;
        //     JsonSerializer.Deserialize<List<BillboardList>>(
        //         await (await http.GetAsync(
        //             "https://raw.githubusercontent.com/mhollingshead/billboard-hot-100/main/all.json"
        //         )).Content.ReadAsStringAsync()
        //     )!;
        //
        // var dateSongMaps = SpotifyHelper.GetDateSongMaps(usContents);
        //
        // var yearMaps = new Dictionary<string, int>();
        // foreach (var map in dateSongMaps)
        // {
        //     foreach (var pair in map)
        //     {
        //         foreach (var song in pair.Value)
        //         {
        //             var simpleName = SpotifyHelper.CleanText(song.artist, true) + " - " +
        //                              SpotifyHelper.CleanText(song.song, false);
        //             yearMaps.TryAdd(simpleName, int.Parse(pair.Key[..4]));
        //         }
        //     }
        // }
        //
        // var dumbedResultsWithStrings = dumbedResults.ToDictionary(x => x.Key.GetArtistDashSong(), x => x.Value);
        // var spanishVowels = new List<char> {'á', 'é', 'í', 'ó', 'ú'};
        // var fullCol = trackMap.Select(x => new Master(
        //         x.Key,
        //         x.Value,
        //         artists[x.Value.artists.First().id],
        //         yearMaps.TryGetValue(x.Key, out int year) ? year : int.Parse(x.Value.album.release_date[..4]),
        //         dumbedResultsWithStrings[x.Key]
        //     )
        // );
        //
        // bool ContainsSpanishLetters(string str) => str.Any(x => spanishVowels.Contains(x)); 
        //
        // var songsToAdd = fullCol
        //     .Where(x => !ContainsSpanishLetters(x.spotifyArtist.name) && !ContainsSpanishLetters(x.spotifySong.name))
        //     .Where(x => !x.SongIsGenre(["rap", "country"]))
        //     .OrderByDescending(x => x.streams)
        //     // .ThenByDescending(x => x.spotifySong.popularity)
        //     .GroupBy(x => x.year / 10)
        //     .Where(x => x.Key is > 197 and < 202)
        //     .SelectMany(x => x.Take(250))
        //     .ToList();
        //
        // await spotifyClientUseCase.AddSongsToNewPlaylist($"Billboard-{DateTime.Now}", songsToAdd.Select(x => x.spotifySong.id));
    }

    [DebuggerDisplay("{GetSummary()}")]
    private record Master(string key, TrackObject spotifySong, FullArtistObject spotifyArtist, int year, long streams)
    {
        public string GetSummary() =>
            $"{key} ({spotifyArtist.name} - {spotifySong.name}) - {year} - {streams}";

        public bool SongIsGenre(string[] genres) => spotifyArtist.genres.Any(s => genres.Any(s.Contains));
    }
}