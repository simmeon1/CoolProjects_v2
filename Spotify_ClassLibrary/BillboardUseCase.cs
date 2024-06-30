using System.Runtime.InteropServices;
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

        var bestSongs = songFullSongMap
            .OrderByDescending(x => x.Value.score)
            .Select(x => x.Value)
            .ToList();


        var groups = new Dictionary<string, List<BillboardSong2>> {{"all", bestSongs}};

        foreach (var song in bestSongs)
        {
            string key = song.year.ToString()[..3];
            groups.TryAdd(key, new List<BillboardSong2>());
            groups[key].Add(song);
        }

        // var toTake = new Dictionary<string, int>()
        // {
        //     { "198", 280 },
        //     { "199", 350 },
        //     { "200", 350 },
        //     { "201", 400 },
        //     { "202", 75 },
        // };
        //
        int defaultCount = 150;
        // var artistSongs = new List<ArtistSong>();
        // foreach (var decade in new List<string> {"198", "199", "200", "201", "202"})
        // {
        //     var count = toTake.TryGetValue(decade, out int val) ? val : defaultCount;
        //     artistSongs.AddRange(
        //         groups[decade].Take(count)
        //             .Select(x => new ArtistSong {artist = x.artist, song = x.song})
        //     );
        // }

        var artistSongs = bestSongs.Select(x => new ArtistSong {artist = x.artist, song = x.song}).ToList();
        var songIds = await spotifySearchUseCase.GetSongIds(artistSongs, 1500, new[] {"rap"});
        string userId = await client.GetUserId();
        string playlistId = await client.CreatePlaylist($"Billboard-{defaultCount}-{DateTime.Now}", userId);
        await client.AddSongsToPlaylist(playlistId, songIds);
        logger.Log("Billboard playlist added.");
    }

    private void MakeSongsMoreGeneric(List<Dictionary<string, List<BillboardSong>>> dateSongMaps)
    {
        foreach (var songMap in dateSongMaps)
        {
            foreach (var songs in songMap.Values)
            {
                foreach (var song in songs)
                {
                    var songStr = song.song;
                    songStr = CleanText(songStr, false);
                    song.song = songStr;
                    
                    var artist = song.artist;
                    artist = CleanText(artist, true);
                    song.artist = artist;
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