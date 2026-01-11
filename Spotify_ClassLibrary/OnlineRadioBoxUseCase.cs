using System.Text.Json;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class OnlineRadioBoxUseCase(SpotifyClientUseCase spotifyClientUseCase, IHttpClient http)
{
    public async Task AddRadio(string radioName, string jsonPath)
    {
        var tracks = new HashSet<string>();
        var i = 0;
        while (true)
        {
            var rp = await http.GetAsync($"https://onlineradiobox.com/json/{radioName}/playlist/{i}");
            var content = await rp.Content.ReadAsStringAsync();
            var resp = JsonSerializer.Deserialize<Response>(content)!;
            if (resp.playlist.Length == 0)
            {
                break;
            }
            foreach (var p in resp.playlist)
            {
                tracks.Add(p.name);
            }
            i++;
        }

        var artistSongs = tracks
            .Select(x =>
                {
                    var artistAndSong = x.Split(separator: " - ", options: StringSplitOptions.RemoveEmptyEntries);
                    if (artistAndSong.Length == 2)
                    {
                        return new ArtistSong(artist: SpotifyHelper.CleanText(artistAndSong[0], true), song: SpotifyHelper.CleanText(artistAndSong[1], false));
                    } else
                    {
                        return new ArtistSong(artist: artistAndSong[0], song: artistAndSong[0]);
                    }

                }
            );


        var artistSongTrackMaps = (await spotifyClientUseCase.GetSongs(
                songs: artistSongs.ToList(),
                storeFolder: jsonPath,
                updateStore: true
            ))!
            .Where(x => x.Value != null)
            .Select(x => new KeyValuePair<string, TrackObject>(key: x.Key, value: x.Value!));

        await spotifyClientUseCase.AddSongsToNewPlaylist(
            playlistName: radioName + "-" + DateTime.Now,
            songIds: artistSongTrackMaps.Select(x => x.Value.id)
        );
    }

    private record Response(Track[] playlist);

    private record Track(string? id, string name);
}