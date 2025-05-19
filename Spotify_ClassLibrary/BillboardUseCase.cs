using System.Diagnostics;
using System.Text.Json;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class BillboardUseCase(SpotifyClientUseCase spotifyClientUseCase, IHttpClient http) {
    public async Task DoWork(string jsonPath)
    {
        var usContents =
            // JsonSerializer.Deserialize<List<BillboardList>>(fileIo.ReadAllText(jsonPath + "\\billboard_all_mhollingshead.json"))!;
            JsonSerializer.Deserialize<List<BillboardList>>(
                await (await http.GetAsync(
                    "https://raw.githubusercontent.com/mhollingshead/billboard-hot-100/main/all.json"
                )).Content.ReadAsStringAsync()
            )!;
        var ukContents =
            // JsonSerializer.Deserialize<List<BillboardList>>(fileIo.ReadAllText(jsonPath + "\\ukSingles.json"))!;
            new List<BillboardList>();

        var dateSongMaps = SpotifyHelper.GetDateSongMaps(usContents, ukContents);
        var songsMap = GetSongsMap(dateSongMaps);
        var artistSongStringMap = songsMap.ToDictionary(x => x.Key.GetArtistDashSong(), x => x.Key);

        var trackMap =
            // JsonSerializer.Deserialize<Dictionary<string, TrackObject?>>(fileIo.ReadAllText(jsonPath + "\\cachedSearchesAndSong.json"))!
            (await spotifyClientUseCase.GetSongs(
                songsMap.Keys.OrderBy(x => x.GetArtistDashSong()).ToList(), jsonPath,
                false
                )
            )!.Where(x => x.Value != null)
                .Select(x => new KeyValuePair<string,TrackObject>(x.Key, x.Value!))
                .ToList();
        
        // JsonSerializer.Deserialize<Dictionary<string, FullArtistObject>>(fileIo.ReadAllText(jsonPath + "\\cachedTrackArtistsMap.json"))!;
        var artists =
            await spotifyClientUseCase.GetArtists(
                trackMap.Select(x => x.Value.artists.First().id).ToList(), jsonPath,
                true,
                true
            );

        var fullCol = trackMap.Select(x => new Master(
                songsMap[artistSongStringMap[x.Key]],
                x.Value,
                artists[x.Value.artists.First().id]
            )
        );

        var songsToAdd = fullCol
            .Where(x => !x.SongIsGenre(["rap", "country"]))
            .OrderByDescending(x => x.billboardSong.score)
            // .ThenByDescending(x => x.spotifySong.popularity)
            .GroupBy(x => x.billboardSong.year / 10)
            .Where(x => x.Key is > 197 and < 202)
            .SelectMany(x => x.Take(250))
            .ToList();

        await spotifyClientUseCase.AddSongsToNewPlaylist($"Billboard-{DateTime.Now}", songsToAdd.Select(x => x.spotifySong.id));
    }
    
    private static Dictionary<ArtistSong, BillboardSong2> GetSongsMap(List<Dictionary<string, List<BillboardSong>>> dateSongMaps)
    {
        var comparer = new ArtistSongEqualityComparer();
        var songScoreMap = new Dictionary<ArtistSong, int>(comparer);
        var songYearMap = new Dictionary<ArtistSong, int>(comparer);
        var songFullSongMap = new Dictionary<ArtistSong, BillboardSong2>(comparer);

        foreach (var regionDateSongMap in dateSongMaps)
        {
            foreach ((string fullDate, List<BillboardSong> songs) in regionDateSongMap)
            {
                var year = int.Parse(fullDate[..4]);
                foreach (var song in songs)
                {
                    var artistSong = new ArtistSong(song.artist, song.song);

                    songScoreMap.TryAdd(artistSong, 0);
                    songScoreMap[artistSong] += 101 - song.this_week;

                    songYearMap.TryAdd(artistSong, year);
                    songYearMap[artistSong] = Math.Min(year, songYearMap[artistSong]);

                    var song2 = new BillboardSong2 { artistSong = new ArtistSong(song.artist, song.song) };
                    songFullSongMap.TryAdd(artistSong, song2);
                    songFullSongMap[artistSong].score = songScoreMap[artistSong];
                    songFullSongMap[artistSong].year = songYearMap[artistSong];
                }
            }
        }

        return songFullSongMap;
    }
    
    [DebuggerDisplay("{GetSummary()}")]
    private record Master(BillboardSong2 billboardSong, TrackObject spotifySong, FullArtistObject spotifyArtist)
    {
        public string GetSummary() =>
            $"{billboardSong.GetArtistDashSong()} ({spotifyArtist.name} - {spotifySong.name}) - {billboardSong.score} - {spotifySong.popularity}";
        public bool SongIsGenre(string[] genres) => spotifyArtist.genres.Any(s => genres.Any(s.Contains));
    }
}