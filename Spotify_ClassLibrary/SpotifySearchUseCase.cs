using System.Runtime.InteropServices;
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

    public async Task<List<string>> GetSongIds(List<ArtistSong> songs)
    {
        List<string> songIds = new();
        for (int i = 0; i < songs.Count; i++)
        {
            var song = songs[i];
            string search = song.artist + " " + song.song;
            songIds.Add(await client.GetIdOfFirstResultOfSearch(search));
            logger.Log($"Retrieved {i + 1} out of {songs.Count} song ids ({search})");
        }
        return songIds;
    }

}