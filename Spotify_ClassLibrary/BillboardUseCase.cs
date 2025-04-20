using System.Text.Json;
using System.Text.RegularExpressions;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class BillboardUseCase
{
    private readonly IFileIO fileIo;
    private readonly ILogger logger;
    private readonly IDelayer delayer;
    private readonly SpotifyClient client;
    private readonly SpotifySearchUseCase spotifySearchUseCase;

    public BillboardUseCase(
        IFileIO fileIo,
        ILogger logger,
        IDelayer delayer,
        SpotifyClient client,
        SpotifySearchUseCase spotifySearchUseCase
    )
    {
        this.fileIo = fileIo;
        this.logger = logger;
        this.delayer = delayer;
        this.client = client;
        this.spotifySearchUseCase = spotifySearchUseCase;
    }

    public async Task DoWork(string jsonPath)
    {
        var usContents =
            JsonSerializer.Deserialize<List<BillboardList>>(
                fileIo.ReadAllText(jsonPath + "\\billboard_all_mhollingshead.json")
            );
        var ukContents =
            JsonSerializer.Deserialize<List<BillboardList>>(fileIo.ReadAllText(jsonPath + "\\ukSingles.json"));
        var usDateSongMaps = usContents.ToDictionary(x => x.date, y => y.data);
        var ukDateSongMaps = ukContents.ToDictionary(x => x.date, y => y.data);
        var dateSongMaps = new List<Dictionary<string, List<BillboardSong>>>() {usDateSongMaps, ukDateSongMaps};

        MakeSongsMoreGeneric(dateSongMaps);

        var songScoreMap = new Dictionary<string, int>();
        var songYearMap = new Dictionary<string, int>();
        var songFullSongMap = new Dictionary<string, BillboardSong2>();

        foreach (var regionDateSongMap in dateSongMaps)
        {
            foreach (var dateSongMap in regionDateSongMap)
            {
                var year = int.Parse(dateSongMap.Key.Substring(0, 4));
                var songs = dateSongMap.Value;
                foreach (var song in songs)
                {
                    var songName = $"{song.artist} - {song.song}";

                    songScoreMap.TryAdd(songName, 0);
                    songScoreMap[songName] += 101 - song.this_week;

                    songYearMap.TryAdd(songName, year);
                    songYearMap[songName] = Math.Min(year, songYearMap[songName]);

                    var song2 = new BillboardSong2
                    {
                        artist = song.artist,
                        song = song.song,
                    };
                    songFullSongMap.TryAdd(songName, song2);
                    songFullSongMap[songName].score = songScoreMap[songName];
                    songFullSongMap[songName].year = songYearMap[songName];
                }
            }
        }

        var billboardSong2Col = songFullSongMap
            .Select(x => x.Value).ToList();
        
        // Average out
        // int GetGroup(int year) => year / 10;
        // double GetAverage(ICollection<BillboardSong2> col) => col.Average(x => x.score);
        // var average = GetAverage(billboardSong2Col);
        // var groups = billboardSong2Col.Select(x => GetGroup(x.year)).Distinct().ToList();
        // foreach (var g in groups)
        // {
        //     var groupSongs = billboardSong2Col.Where(x => GetGroup(x.year) == g).ToList();
        //     var groupAverage = GetAverage(groupSongs.ToList());
        //     var multiplier = average / groupAverage;
        //     foreach (var s in groupSongs)
        //     {
        //         s.score = (int) Math.Ceiling(s.score * multiplier);
        //     }
        // }
        // End of averaging
        
        var bestSongs = billboardSong2Col
            .OrderByDescending(x => x.score)
            .ToList();


        // Take from multiple groups
        // var groups = new Dictionary<string, List<BillboardSong2>> {{"all", bestSongs}};

        // foreach (var song in bestSongs)
        // {
        //     string key = song.year.ToString()[..3];
        //     groups.TryAdd(key, new List<BillboardSong2>());
        //     groups[key].Add(song);
        // }

        // var toTake = new Dictionary<string, int>()
        // {
        //     { "198", 280 },
        //     { "199", 350 },
        //     { "200", 350 },
        //     { "201", 400 },
        //     { "202", 75 },
        // };
        //
        // var artistSongs = new List<ArtistSong>();
        // foreach (var decade in new List<string> {"198", "199", "200", "201", "202"})
        // {
        //     var count = toTake.TryGetValue(decade, out int val) ? val : defaultCount;
        //     artistSongs.AddRange(
        //         groups[decade].Take(count)
        //             .Select(x => new ArtistSong {artist = x.artist, song = x.song})
        //     );
        // }

        var artistSongs = new List<ArtistSong>();
        var bestSongsMap = new Dictionary<string, BillboardSong2>();
        var artistSongsMap = new Dictionary<string, ArtistSong>();
        foreach (var bestSong in bestSongs)
        {
            var artistSong = new ArtistSong {artist = bestSong.artist, song = bestSong.song};
            artistSongs.Add(artistSong);
            bestSongsMap.Add(artistSong.ToString(), bestSong);
            artistSongsMap.Add(artistSong.ToString(), artistSong);
        }
        
        // var artistSongTrackMaps = await spotifySearchUseCase.GetSongs(artistSongs, fileIo, jsonPath);
        var artistSongTrackMaps = 
            JsonSerializer.Deserialize<Dictionary<string, TrackObject?>>(fileIo.ReadAllText(jsonPath + "\\cachedSearchesAndSong.json"));
        
        var artists =
            // await client.GetArtists(mainArtistMap.Select(x => x.Value).ToList());
        JsonSerializer.Deserialize<Dictionary<string, FullArtistObject>>(fileIo.ReadAllText(jsonPath + "\\cachedTrackArtistsMap.json"));

        var filteredTracks = new Dictionary<string, TrackObject>();
        var filteredArtistSongsMap = new Dictionary<string, ArtistSong>();
        foreach (var pair in artistSongTrackMaps)
        {
            var key = pair.Key;
            var track = pair.Value;
            if (track != null)
            {
                filteredTracks.Add(key, track);
                filteredArtistSongsMap.Add(key, artistSongsMap[key]);
            }
        }

        // var mainArtistMap = new Dictionary<string, string>();
        var orderedArtistSongsKPs = filteredArtistSongsMap
            .Where(x =>
                {
                    var genres = artists[filteredTracks[x.Key].artists.First().id].genres;
                    bool any = !genres.Any(g => g.ToLower().Contains("rap") || g.ToLower().Contains("country"));
                               // && genres.Any(g => g.ToLower().Contains("rock") || g.ToLower().Contains("metal"));
                    return any;

                }
            )
            // .Where(x => bestSongsMap[x.Key].year >= 1990 && bestSongsMap[x.Key].year <= 1999)
            // .OrderByDescending(x => bestSongsMap[x.Key].score)
            .OrderByDescending(x => filteredTracks[x.Key].popularity)
            // .OrderByDescending(x => bestSongsMap[x.Key].score * (filteredTracks[x.Key].popularity / (double) 100))
            // .ThenByDescending(x => bestSongsMap[x.Key].score)
            // .Take(500)
            .GroupBy(x => bestSongsMap[x.Key].year)
                .Where(x => x.Key is > 1969 and < 2024)
                .SelectMany(x => x.Take(10).ToList())
            .ToList();

        var x = orderedArtistSongsKPs.Select(x => bestSongsMap[x.Key]).ToList();
        
        // foreach (var pair in orderedArtistSongsKPs)
        // {
        //     mainArtistMap.Add(pair.Key, filteredTracks[pair.Key].artists.First().id);
        // }
        
        // var toAdd = new List<string>();
        // var artists =
        //     await client.GetArtists(mainArtistMap.Select(x => x.Value).ToList());
        // fileIo.WriteAllText(jsonPath + "\\cachedTrackArtistsMap.json", artists.SerializeObject());
        
        // foreach (var pair in orderedArtistSongsKPs)
        // {
        //     var artistSong = pair.Value;
        //     var toString = artistSong.ToString();
        //     var track = filteredTracks[toString];
        //     var genres = artists[mainArtistMap[toString]].genres;
        // if (!genres.Any(g => g.ToLower().Contains("rap") || g.ToLower().Contains("country"))) toAdd.Add(track.id);
        // }
        
        string userId = await client.GetUserId();
        string playlistId = await client.CreatePlaylist($"Billboard-{DateTime.Now}", userId);
        await client.AddSongsToPlaylist(playlistId, orderedArtistSongsKPs.Select(
            x => filteredTracks[x.Key].id).ToList()
        );
        logger.Log("Billboard playlist added.");
    }

    private void MakeSongsMoreGeneric(List<Dictionary<string, List<BillboardSong>>> dateSongMaps)
    {
        var songss = new List<string>();
        var artists = new List<string>();
        foreach (var songMap in dateSongMaps)
        {
            foreach (var songs in songMap.Values)
            {
                foreach (var song in songs)
                {
                    songss.Add(song.song);
                    artists.Add(song.artist);
                    song.song = CleanText(song.song, false);
                    song.artist = CleanText(song.artist, true);
                }
            }
        }
    }

    private static string CleanText(string text, bool isArtist)
    {
        //\bAND\b|& | with
        text = text.ToUpper();
        text = ReplaceTextPattern(text, @"\(.*?\)");
        text = ReplaceTextPattern(text, @"(\bFEAT\b|\bFT\b|\bFEATURING\b|DUET WITH|A DUET WITH|\\|\/).*");

        if (isArtist)
        {
            text = ReplaceTextPattern(text, @"(\bAND\b|\bWITH\b|& ).*");
        }
        
        text = ReplaceTextPattern(text, @"('|^THE )");
        text = ReplaceTextPattern(text, @"[^a-zA-Z0-9 ]", " ");
        text = ReplaceTextPattern(text, @"\s+", " ");
        text = text.Trim();
        return text;
    }

    private static string ReplaceTextPattern(string str, string pattern, string replacement = "")
    {
        return Regex.Replace(str, pattern, replacement);
    }
}