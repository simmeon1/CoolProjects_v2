using System.Text.Json;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class SpotifySearchUseCase
{
    private readonly ILogger logger;
    private readonly SpotifyClient client;

    public SpotifySearchUseCase(ILogger logger, SpotifyClient client)
    {
        this.logger = logger;
        this.client = client;
    }

    public async Task<Dictionary<string, TrackObject?>> GetSongs(
        List<ArtistSong> songs,
        IFileIO fileIo,
        string jsonPath
    )
    {
        var cachePath = jsonPath + "\\cachedSearchesAndSong.json";
        var cached =
            JsonSerializer.Deserialize<Dictionary<string, TrackObject?>>(fileIo.ReadAllText(cachePath))!;
        
        Dictionary<string, TrackObject?> tracks = new();
        for (int i = 0; i < songs.Count; i++)
        {
            ArtistSong song = songs[i];
            TrackObject? track;
            string songToString = song.ToString();
            if (cached.ContainsKey(songToString))
            {
                track = cached[songToString];
            }
            else
            {
                // return tracks;
                string search = song.artist + " " + song.song;

                try
                {
                    track = await client.GetFirstTrackResult(search);
                }
                catch (Exception e)
                {
                    fileIo.WriteAllText(cachePath, tracks.SerializeObject());
                    throw;
                }
                // For getting thousands of songs
                await Task.Delay(500);
            }
            logger.Log(
                track == null
                    ? $"No tracks found ({songToString})"
                    : $"Searched {i} out of {songs.Count} song ids ({songToString}) (Popularity: {track.popularity})"
            );
            tracks.Add(songToString, track);
        }

        fileIo.WriteAllText(cachePath, tracks.SerializeObject());
        return tracks;
    }
}