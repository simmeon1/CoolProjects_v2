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

    public async Task<Dictionary<string, TrackObject?>> GetSongs(List<ArtistSong> songs)
    {
        Dictionary<string, TrackObject?> tracks = new();
        for (int i = 0; i < songs.Count; i++)
        {
            ArtistSong song = songs[i];
            string search = song.artist + " " + song.song;
            var track = await client.GetFirstTrackResult(search);
            logger.Log(
                track == null
                    ? $"No tracks found ({search})"
                    : $"Searched {i} out of {songs.Count} song ids ({search})"
            );
            tracks.Add(song.ToString(), track);
        }

        return tracks;
    }
}