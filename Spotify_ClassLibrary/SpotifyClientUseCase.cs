using System.Text.Json;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class SpotifyClientUseCase(SpotifyClient client, ILogger logger, IFileIO fileIo)
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

    public async Task<Dictionary<string, FullArtistObject>> GetArtists(IEnumerable<string> artistIds, string storeFolder)
    {
        return await GetSeveral(artistIds, storeFolder, "spotifyArtistCache.json", client.GetArtists);
    }

    public async Task<Dictionary<string, SimpleTrackObject>> GetSongs(IEnumerable<string> artistIds, string storeFolder)
    {
        return await GetSeveral(
            artistIds,
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
            )
        );
    }
    
    private async Task<Dictionary<string, T>> GetSeveral<T>(
        IEnumerable<string> ids,
        string storeFolder,
        string storeName,
        Func<IEnumerable<string>, Task<Dictionary<string, T>>> entriesGetter
    )
    {
        var idsList = ids.ToList();
        var storePath = $"{storeFolder}\\{storeName}";
        var store = GetStore<T>(storePath);
        var initialStoreCount = store.Count;
        logger.Log($"{initialStoreCount} initial store entry count.");
        try
        {
            var chunks = idsList.Except(store.Keys).Distinct().Chunk(50);
            foreach (var idSets in chunks)
            {
                var entries = await entriesGetter(idSets);
                foreach (var entry in entries)
                {
                    store.Add(entry.Key, entry.Value);
                }
                logger.Log($"{store.Count - initialStoreCount} store entries added.");
            }
            return idsList.ToDictionary(x => x, x => store[x]);
        }
        finally
        {
            FinalizeStoreUsage(storePath, store, true);
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
            fileIo.WriteAllText(storePath, store.SerializeObject());
        }
    }
}