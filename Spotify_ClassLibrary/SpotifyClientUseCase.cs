using System.Text;
using System.Text.Json;
using System.Web;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class SpotifyClientUseCase(SpotifyClient client, ILogger logger, IFileIO fileIo, IHttpClient http)
{
    public async Task MergePlaylists(IEnumerable<string> playlists, string finalPlaylist)
    {
        List<string> playlistTracks = new();
        foreach (string playlist in playlists)
        {
            playlistTracks.AddRange(await client.GetPlaylistTracks(playlist));
        }

        playlistTracks = playlistTracks.Distinct().ToList();
        string userId = await client.GetUserId();
        string playlistId = await client.CreatePlaylist(finalPlaylist + "-" + DateTime.Now, userId);
        await client.AddSongsToPlaylist(playlistId, playlistTracks);
    }

    public async Task AddSongsToNewPlaylist(string playlistName, IEnumerable<string> songIds)
    {
        string userId = await client.GetUserId();
        string playlistId = await client.CreatePlaylist(playlistName, userId);
        await client.AddSongsToPlaylist(playlistId, songIds);
        logger.Log("Playlist added.");
    }

    public async Task<Dictionary<string, TrackObject?>> GetSongs(
        List<ArtistSong> songs,
        string storeFolder,
        bool updateStore
    ) {
        var storePath = storeFolder + "/spotifyTrackCache.json";
        var store = GetStore<TrackObject?>(storePath);
        Dictionary<string, TrackObject?> tracks = new();
        try
        {
            for (int i = 0; i < songs.Count; i++)
            {
                ArtistSong song = songs[i];
                string artistDashSong = song.GetArtistDashSong();
                TrackObject? track;
                bool wasCached = false;
                if (store.TryGetValue(artistDashSong, out TrackObject? value))
                {
                    track = value;
                    wasCached = true;
                }
                else
                {
                    while (true)
                    {
                        try
                        {
                            track = await client.GetFirstTrackResult(song.GetArtistSpaceSong());
                            break;
                        }
                        catch (HttpRequestException ex)
                        {
                            // Try again
                        }
                    }
                }

                var extra = wasCached ? " (already cached)" : "";
                logger.Log(
                    track == null
                        ? $"No tracks found ({artistDashSong})"
                        : $"Searched {i} out of {songs.Count} song ids ({artistDashSong}) (Popularity: {track.popularity}){extra}"
                );
                store.TryAdd(artistDashSong, track);
                tracks.TryAdd(artistDashSong, track);

                if (i % 250 == 0)
                {
                    FinalizeStoreUsage(storePath, store, updateStore);
                }
            }
            return tracks;
        }
        finally
        {
            FinalizeStoreUsage(storePath, store, updateStore);
        }
    }

    public async Task<Dictionary<string, FullArtistObject>> GetArtists(
        IEnumerable<string> artistIds,
        string storeFolder,
        bool getNewEntries,
        bool updateStore
    )
    {
        return await GetSeveral(
            artistIds,
            storeFolder,
            "spotifyArtistCache.json",
            client.GetArtists,
            x => x,
            getNewEntries,
            updateStore,
            50
        );
    }

    public async Task<Dictionary<string, SimpleTrackObject>> GetSongs(
        IEnumerable<string> songIds,
        string storeFolder,
        bool getNewEntries,
        bool updateStore
    ) {
        return await GetSeveral(
            songIds,
            storeFolder,
            "spotifyTracksByIdCache.json",
            async x => (await client.GetSongs(x)).ToDictionary(
                y => y.Key,
                y =>
                {
                    var track = y.Value;
                    var firstArtist = track.artists.First();
                    return new SimpleTrackObject
                    {
                        id = track.id,
                        name = track.name,
                        artist_id = firstArtist.id,
                        artist_name = firstArtist.name,
                        popularity = track.popularity,
                        release_date = track.album.release_date
                    };
                }
            ), x => x, getNewEntries, updateStore, 50
        );
    }

    public async Task<Dictionary<string, YoutubeTrack>> GetYoutubeSongs(
        IEnumerable<SimpleTrackObject> songs,
        string storeFolder,
        bool getNewEntries,
        bool updateStore
    ) {
        return await GetSeveral(
            songs,
            storeFolder,
            "youtubeTracksBySpotifyIdCache.json",
            async x =>
            {
                var song = x.FirstOrDefault();
                if (song == null)
                {
                    return new Dictionary<string, YoutubeTrack>();
                }
                
                var response = await http.GetAsync("https://www.youtube.com/results?search_query=" + 
                    HttpUtility.UrlEncode(
                        SpotifyHelper.CleanText(song.artist_name, true) + " " + SpotifyHelper.CleanText(song.name, false)
                    )
                );
                var responseContent = await response.Content.ReadAsStringAsync();
                var obj = GetVideoRenderer(responseContent);
                var id = obj.GetProperty("videoId").GetString();
                var name = obj.GetProperty("title").GetProperty("runs")[0].GetProperty("text").GetString();
                var views = long.Parse(obj.GetProperty("viewCountText").GetProperty("simpleText").GetString().Replace(",", "").Replace(" views", ""));
                return new Dictionary<string, YoutubeTrack> {{song.id, new YoutubeTrack(id, name, views)}};
            }, x => x.id, getNewEntries, updateStore, 1
        );
    }
    
    private static JsonElement GetVideoRenderer(string str)
    {
        var lookFor = "\"videoRenderer\":";
        var index = str.IndexOf(lookFor) + lookFor.Length;
        var sb = new StringBuilder();
        var counter = 0;
        while (true)
        {
            var ch = str[index];
            sb.Append(ch);
            index++;
            if (ch == '{')
            {
                counter++;
            } else if (ch == '}')
            {
                counter--;
            }

            if (counter == 0)
            {
                return JsonSerializer.Deserialize<JsonElement>(sb.ToString());
            }            
        }
    }

    
    private async Task<Dictionary<string, T>> GetSeveral<T, K>(
        IEnumerable<K> inputList,
        string storeFolder,
        string storeName,
        Func<IEnumerable<K>, Task<Dictionary<string, T>>> entriesGetter,
        Func<K, string> idGetter,
        bool getNewEntries,
        bool updateStore,
        int chunkSize
    ) {
        // Remove duplicates
        inputList = inputList.DistinctBy(idGetter).ToList();
        var storePath = $"{storeFolder}\\{storeName}";
        var store = GetStore<T>(storePath);
        var initialStoreCount = store.Count;
        void Finish() => FinalizeStoreUsage(storePath, store, updateStore);
        logger.Log($"{initialStoreCount} initial store entry count.");
        
        client.TooManyRequestsAction = Finish;
        try
        {
            if (getNewEntries)
            {
                var chunks = inputList.ExceptBy(store.Keys, idGetter).Distinct().Chunk(chunkSize);
                foreach (var idSets in chunks)
                {
                    var entries = await entriesGetter(idSets);
                    foreach (var entry in entries)
                    {
                        store.Add(entry.Key, entry.Value);
                    }
                    logger.Log($"{store.Count - initialStoreCount} store entries added.");
                }
            }
            else
            {
                logger.Log($"Skipping collection of new entries.");
            }
            return inputList.Where(x => store.ContainsKey(idGetter(x))).ToDictionary(idGetter, x => store[idGetter(x)]);
        }
        finally
        {
            Finish();
        }
    }

    private Dictionary<string, T> GetStore<T>(string storePath)
    {
        return fileIo.FileExists(storePath)
            ? JsonSerializer.Deserialize<Dictionary<string, T>>(fileIo.ReadAllText(storePath))!
            : new Dictionary<string, T>();
    }

    private void FinalizeStoreUsage(string storePath, object store, bool updateStore)
    {
        if (updateStore)
        {
            logger.Log("Saving store.");
            fileIo.WriteAllText(storePath, store.SerializeObject());
        }
    }
}