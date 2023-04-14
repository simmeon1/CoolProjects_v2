namespace Spotify_ClassLibrary;

public class SpotifyMergePlaylistsUseCase
{
    private SpotifyClient client;

    public SpotifyMergePlaylistsUseCase(SpotifyClient client)
    {
        this.client = client;
    }

    public async Task MergePlaylists(string[] playlists, string finalPlaylist)
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
}