using System.Runtime.InteropServices;
using System.Text.Json;
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
        var contents = JsonSerializer.Deserialize<List<BillboardList>>(fileIo.ReadAllText(jsonPath));
        var dateSongMaps = contents.ToDictionary((x) => x.date, y => y.data);

        var songScoreMap = new Dictionary<string, int>();
        var songYearMap = new Dictionary<string, int>();
        var songFullSongMap = new Dictionary<string, BillboardSong2>();
        
        foreach (var dateSongMap in dateSongMaps)
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

                var song2 = new BillboardSong2()
                {
                    artist = song.artist,
                    song = song.song,
                };
                songFullSongMap.TryAdd(songName, song2);
                songFullSongMap[songName].score = songScoreMap[songName];
                songFullSongMap[songName].year = songYearMap[songName];
            }
        }

        var bestSongs = songFullSongMap
            .OrderByDescending(x => x.Value.score)
            .Select(x => x.Value)
            .ToList();


        var groups = new Dictionary<string, List<BillboardSong2>>{ { "all", bestSongs } };

        foreach (var song in bestSongs)
        {
            string key = song.year.ToString()[..3];
            groups.TryAdd(key, new List<BillboardSong2>());
            groups[key].Add(song);
        }

        int count = 150;
        var artistSongs = new List<ArtistSong>();
        foreach (var decade in new List<string> { "197", "198", "199", "200", "201", "202" })
        {
            artistSongs.AddRange(groups[decade].Take(count)
                .Select(x => new ArtistSong {artist = x.artist, song = x.song}));
        }

        var songIds = await spotifySearchUseCase.GetSongIds(artistSongs);
        string userId = await client.GetUserId();
        string playlistId = await client.CreatePlaylist($"Billboard-{count}-{DateTime.Now}", userId);
        await client.AddSongsToPlaylist(playlistId, songIds);
        logger.Log("Billboard playlist added.");
        
        
        // var bestSongs = songScoreMap.OrderByDescending(x => x.Value).ToList();
        var bestSongs2 = bestSongs.Select(x => x.ToString()).ToList();
        var x = 1;
    }
}