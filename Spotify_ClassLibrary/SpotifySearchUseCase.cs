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
        var cached = JsonSerializer.Deserialize<Dictionary<string, TrackObject?>>(fileIo.ReadAllText(cachePath))!;
        
        Dictionary<string, TrackObject?> tracks = new();
        for (int i = 0; i < songs.Count; i++)
        {
            ArtistSong song = songs[i];
            TrackObject? track;
            string artistDashSong = song.GetArtistDashSong();
            if (cached.TryGetValue(artistDashSong, out TrackObject? value))
            {
                track = value;
            }
            else
            {
                // return tracks;
                try
                {
                    track = await client.GetFirstTrackResult(song.GetArtistSpaceSong());
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
                    ? $"No tracks found ({artistDashSong})"
                    : $"Searched {i} out of {songs.Count} song ids ({artistDashSong}) (Popularity: {track.popularity})"
            );
            tracks.Add(artistDashSong, track);
        }

        fileIo.WriteAllText(cachePath, tracks.SerializeObject());
        return tracks;
    }
}