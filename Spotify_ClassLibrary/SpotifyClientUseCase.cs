using System.Text.Json;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class SpotifyClientUseCase(SpotifyClient client, ILogger logger)
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
        IFileIO fileIo,
        string storePath,
        bool saveToFile
    ) {
        var store = GetStore<TrackObject?>(fileIo, storePath);
        Dictionary<string, TrackObject?> tracks = new();
        try
        {
            for (int i = 0; i < songs.Count; i++)
            {
                ArtistSong song = songs[i];
                string artistDashSong = song.GetArtistDashSong();
                TrackObject? track = store.TryGetValue(artistDashSong, out TrackObject? value)
                    ? value
                    : await client.GetFirstTrackResult(song.GetArtistSpaceSong());
                logger.Log(
                    track == null
                        ? $"No tracks found ({artistDashSong})"
                        : $"Searched {i} out of {songs.Count} song ids ({artistDashSong}) (Popularity: {track.popularity})"
                );
                store.TryAdd(artistDashSong, track);
                tracks.Add(artistDashSong, track);
            }
            return tracks;  
        }
        finally
        {
            SaveStoreToFile(fileIo, storePath, saveToFile, store);
        }
    }

    private static Dictionary<string, T> GetStore<T>(IFileIO fileIo, string storePath)
    {
        return fileIo.FileExists(storePath) ?
            JsonSerializer.Deserialize<Dictionary<string, T>>(fileIo.ReadAllText(storePath))! :
            new Dictionary<string, T>();
    }

    public async Task<Dictionary<string, FullArtistObject>> GetArtists(
        List<string> artistIds,
        IFileIO fileIo,
        string storePath,
        bool saveToFile
    ) {
        var store = GetStore<FullArtistObject>(fileIo, storePath);
        try
        {
            var artists = await client.GetArtists(artistIds.Except(store.Keys));
            foreach (var artist in artists)
            {
                store.Add(artist.Key, artist.Value);
            }
            return artistIds.ToDictionary(x => x, x => store[x]);
        }
        finally
        {
            SaveStoreToFile(fileIo, storePath, saveToFile, store);
        }
    }

    private static void SaveStoreToFile(IFileIO fileIo, string storePath, bool saveToFile, object store)
    {
        if (saveToFile)
        {
            fileIo.WriteAllText(storePath, store.SerializeObject());
        }
    }
}