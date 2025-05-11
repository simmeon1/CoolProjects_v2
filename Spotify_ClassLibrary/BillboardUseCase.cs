using System.Diagnostics;
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
            )!;
        var ukContents =
            JsonSerializer.Deserialize<List<BillboardList>>(fileIo.ReadAllText(jsonPath + "\\ukSingles.json"))!;

        Dictionary<string, List<BillboardSong>> GetDictFromBillboardList(List<BillboardList> billboardLists) =>
            billboardLists.ToDictionary(x => x.date, y => y.data);

        var dateSongMaps = new List<Dictionary<string, List<BillboardSong>>>
        {
            GetDictFromBillboardList(usContents), GetDictFromBillboardList(ukContents)
        };
        
        MakeSongsMoreGeneric(dateSongMaps);
        var songsMap = GetSongsMap(dateSongMaps);
        // return songFullSongMap.Select(x => x.Value).OrderByDescending(x => x.score).ToList();

        var trackMap = new Dictionary<string, TrackObject>();
        // var filteredArtistSongsMap = new Dictionary<string, ArtistSong>();
        foreach ((string key, TrackObject? track) in JsonSerializer.Deserialize<Dictionary<string, TrackObject?>>(fileIo.ReadAllText(jsonPath + "\\cachedSearchesAndSong.json"))!)
        {
            if (track != null)
            {
                trackMap.Add(key, track);
                // filteredArtistSongsMap.Add(key, artistSongsMap[key]);
            }
        }
        
        var artists =
            // await client.GetArtists(mainArtistMap.Select(x => x.Value).ToList());
            JsonSerializer.Deserialize<Dictionary<string, FullArtistObject>>(fileIo.ReadAllText(jsonPath + "\\cachedTrackArtistsMap.json"))!;

        var fullCol = trackMap.Select(x => new Master(
                songsMap[x.Key],
                x.Value,
                artists[x.Value.artists.First().id]
            )
        );

        var songsToAdd = fullCol
            .Where(x => !x.SongIsGenre(new[]{ "rap", "country" }))
            .OrderByDescending(x => x.spotifySong.popularity)
            .ThenByDescending(x => x.billboardSong.score)
            .GroupBy(x => x.billboardSong.year / 10)
            .Where(x => x.Key == 198)
            .SelectMany(x => x.Take(100))
            .ToList();
        
        string userId = await client.GetUserId();
        string playlistId = await client.CreatePlaylist($"Billboard-{DateTime.Now}", userId);
        await client.AddSongsToPlaylist(playlistId, songsToAdd.Select(x => x.spotifySong.id));
        logger.Log("Billboard playlist added.");
    }

    private static Dictionary<string, BillboardSong2> GetSongsMap(List<Dictionary<string, List<BillboardSong>>> dateSongMaps)
    {
        var songScoreMap = new Dictionary<string, int>();
        var songYearMap = new Dictionary<string, int>();
        var songFullSongMap = new Dictionary<string, BillboardSong2>();

        foreach (var regionDateSongMap in dateSongMaps)
        {
            foreach (var dateSongMap in regionDateSongMap)
            {
                var year = int.Parse(dateSongMap.Key[..4]);
                var songs = dateSongMap.Value;
                foreach (var song in songs)
                {
                    var songName = new ArtistSong(song.artist, song.song).GetArtistDashSong();

                    songScoreMap.TryAdd(songName, 0);
                    songScoreMap[songName] += 101 - song.this_week;

                    songYearMap.TryAdd(songName, year);
                    songYearMap[songName] = Math.Min(year, songYearMap[songName]);

                    var song2 = new BillboardSong2 { artistSong = new ArtistSong(song.artist, song.song) };
                    songFullSongMap.TryAdd(songName, song2);
                    songFullSongMap[songName].score = songScoreMap[songName];
                    songFullSongMap[songName].year = songYearMap[songName];
                }
            }
        }

        return songFullSongMap;
    }
    
    private void MakeSongsMoreGeneric(List<Dictionary<string, List<BillboardSong>>> dateSongMaps)
    {
        foreach (var songMap in dateSongMaps)
        {
            foreach (var songs in songMap.Values)
            {
                foreach (var song in songs)
                {
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

    [DebuggerDisplay("{GetSummary()}")]
    private class Master
    {
        public BillboardSong2 billboardSong { get; init; }
        public TrackObject spotifySong { get; init; }
        public FullArtistObject spotifyArtist { get; init; }

        public Master(BillboardSong2 billboardSong, TrackObject spotifySong, FullArtistObject spotifyArtist)
        {
            this.billboardSong = billboardSong;
            this.spotifySong = spotifySong;
            this.spotifyArtist = spotifyArtist;
        }

        public string GetSummary() =>
            $"{billboardSong.GetArtistDashSong()} ({spotifyArtist.name} - {spotifySong.name}) - {billboardSong.score} - {spotifySong.popularity}";
        public bool SongIsGenre(string[] genres) => spotifyArtist.genres.Any(genres.Contains);
    }
}