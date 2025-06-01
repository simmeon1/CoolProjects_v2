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
        // await GetTopListStores(jsonPath);
        // await GetListenerStores(jsonPath);
        chromeDriver.Quit();
        
        var totalCache = new Dictionary<string, long>();
        var trackMap = new Dictionary<string, SimpleTrackObject>();
        var getNewEntries = true;
        var updateStore = true;
        var collections = new[]
        {
            "kworblisteners",
            "kworblisteners2",
            "kworblisteners3",
            "kworblisteners4",
            "kworblisteners5",
            "kworblisteners6",
            "kworblisteners7",
            "kworblisteners8",
            "kworblisteners9"
        };

        foreach (var col in collections)
        {
            foreach (var pair in JsonSerializer.Deserialize<Dictionary<string, long>>(
                         fileIo.ReadAllText($"{jsonPath}\\{col}.json")
            )!) {
                totalCache.TryAdd(pair.Key, pair.Value);
            }
        }
        
        var cache = totalCache.Where(x => x.Value >= 5000000).ToList();
        // logger.Log($"Processing {col}.");
        logger.Log($"{cache.Count} songs to collect.");
        var songs = await spotifyClientUseCase.GetSongs(cache.Select(x => x.Key), jsonPath, getNewEntries, updateStore);
        foreach (var song in songs)
        {
            trackMap.TryAdd(song.Key, song.Value);
        }
        
        var artists = await spotifyClientUseCase.GetArtists(
            trackMap.Values.Select(x => x.artist_id),
            jsonPath,
            getNewEntries, 
            updateStore
        );
        
        var usContents =
            // JsonSerializer.Deserialize<List<BillboardList>>(fileIo.ReadAllText(jsonPath + "\\billboard_all_mhollingshead.json"))!;
            JsonSerializer.Deserialize<List<BillboardList>>(
                await (await http.GetAsync(
                    "https://raw.githubusercontent.com/mhollingshead/billboard-hot-100/main/all.json"
                )).Content.ReadAsStringAsync()
            )!;
        
        var dateSongMaps = SpotifyHelper.GetDateSongMaps(usContents);
        
        var yearMaps = new Dictionary<string, int>();
        foreach (var map in dateSongMaps)
        {
            foreach (var pair in map)
            {
                foreach (var song in pair.Value)
                {
                    var simpleName = SpotifyHelper.CleanText(song.artist, true) + " - " +
                                     SpotifyHelper.CleanText(song.song, false);
                    yearMaps.TryAdd(simpleName, int.Parse(pair.Key[..4]));
                }
            }
        }

        // Remove duplicates
        trackMap = trackMap
            .GroupBy(
                x => totalCache[x.Key],
                x => x.Value
                , (x, y) => y.OrderByDescending(z => z.popularity).First()
            ).ToDictionary(x => x.id, x => x);
        
        // Anything
        var songsToAdd = trackMap.Values
            .Where(x =>
                !ContainsSpanish(x.name) &&
                !ContainsSpanish(x.artist_name) &&
                artists.TryGetValue(x.artist_id, out var artist) &&
                (new[] { "Linkin Park", "Eminem" }.Contains(artist.name) || !artist.genres.Any(g => new[] {"rap", "country"}.Any(g.Contains)))
            ).OrderByDescending(x => totalCache[x.id])
            .GroupBy(
                x => Math.Min(
                    yearMaps.GetValueOrDefault(
                        SpotifyHelper.CleanText(x.artist_name, true) + " - " +
                        SpotifyHelper.CleanText(x.name, false),
                        9999
                    ),
                    int.Parse(x.release_date[..4])
                ) / 10,
                (x, y) => y.Take(x < 197 ? 0 : x > 201 ? 100 : 500)
            )
            // .Where(x => x.Key is > 197 and < 202)
            .SelectMany(x => x)
            .ToList();

        // Rock/metal
        // var songsToAdd = trackMap.Values
        //     .Where(x =>
        //         !ContainsSpanish(x.name) &&
        //         !ContainsSpanish(x.artist_name) &&
        //         artists.TryGetValue(x.artist_id, out var artist) &&
        //         artist.genres.Any(g => new[] {"rock", "metal"}.Any(g.Contains))
        //     ).OrderByDescending(x => totalCache[x.id])
        //     // .GroupBy(
        //     //     x => Math.Min(
        //     //         yearMaps.GetValueOrDefault(
        //     //             SpotifyHelper.CleanText(x.artist_name, true) + " - " +
        //     //             SpotifyHelper.CleanText(x.name, false),
        //     //             9999
        //     //         ),
        //     //         int.Parse(x.release_date[..4])
        //     //     ) / 10,
        //     //     (x, y) => y.Take(x < 197 ? 0 : x > 201 ? 100 : 500)
        //     // )
        //     .Take(1000)
        //     .ToList();
        
        await spotifyClientUseCase.AddSongsToNewPlaylist($"kworb-topLists-{DateTime.Now}", songsToAdd.Select(x => x.id));
    }

    private async Task GetListenerStores(string jsonPath)
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
    }
    
    private async Task GetTopListStores(string jsonPath)
    {
        var links = new[]
        {
            //  "https://kworb.net/spotify/songs.html",
            "https://kworb.net/spotify/songs_2024.html",
            "https://kworb.net/spotify/songs_2023.html",
            "https://kworb.net/spotify/songs_2022.html",
            "https://kworb.net/spotify/songs_2021.html",
            "https://kworb.net/spotify/songs_2020.html",
            "https://kworb.net/spotify/songs_2019.html",
            "https://kworb.net/spotify/songs_2018.html",
            "https://kworb.net/spotify/songs_2017.html",
            "https://kworb.net/spotify/songs_2016.html",
            "https://kworb.net/spotify/songs_2015.html",
            "https://kworb.net/spotify/songs_2014.html",
            "https://kworb.net/spotify/songs_2013.html",
            "https://kworb.net/spotify/songs_2012.html",
            "https://kworb.net/spotify/songs_2011.html",
            "https://kworb.net/spotify/songs_2010.html",
            "https://kworb.net/spotify/songs_2005.html",
            "https://kworb.net/spotify/songs_2000.html",
            "https://kworb.net/spotify/songs_1990.html",
            "https://kworb.net/spotify/songs_1980.html",
            "https://kworb.net/spotify/songs_1970.html",
            "https://kworb.net/spotify/songs_1960.html",
            "https://kworb.net/spotify/songs_1950.html"
        };
        
        // var results = JsonSerializer.Deserialize<Dictionary<string, long>>(fileIo.ReadAllText(jsonPath + "\\kworb.json"))!;

        var results = new Dictionary<string, long>();
        foreach (var link in links)
        {
            var response = await http.GetAsync(link);
            var responseContent = await response.Content.ReadAsStringAsync();
            var table = Regex.Match(responseContent, @"(<table (.|\n)*?</table>)").Groups[1].Value;
            fileIo.WriteAllText("temp.html", table);
            chromeDriver.GoToUrl("file:///C:/Users/simme/source/repos/CoolProjects_v2/Spotify_Console/bin/Debug/net9.0/temp.html");
            // chromeDriver.GoToUrl(link);
            var map = (Dictionary<string, object>)chromeDriver.ExecuteScript(
                """
                var result = {};
                var rows = document.querySelectorAll('tbody tr');
                for (var row of rows) {
                    var cells = row.cells;
                    var artistSong = cells[0].textContent;
                    var streams = parseInt(cells[1].textContent.replaceAll(",", ""));
                    result[artistSong] = Math.max(result[artistSong] || 0, streams);
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
        fileIo.WriteAllText(jsonPath + "/kwobTopListens.json", results.SerializeObject());
    }

    private bool ContainsSpanish(string str) => Regex.IsMatch(
        str.ToLower(),
        "(á|é|í|ó|ú|\bla\b|\blo\b|\bque\b|\bcomo\b|\bel\b)"
    );

    [DebuggerDisplay("{GetSummary()}")]
    private record Master(string key, TrackObject spotifySong, FullArtistObject spotifyArtist, int year, long streams)
    {
        public string GetSummary() =>
            $"{key} ({spotifyArtist.name} - {spotifySong.name}) - {year} - {streams}";

        public bool SongIsGenre(string[] genres) => spotifyArtist.genres.Any(s => genres.Any(s.Contains));
    }
}