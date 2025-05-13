using System.Collections.ObjectModel;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class UkRadioLiveAddRadioUseCase(
    IFileIO fileIo,
    IDelayer delayer,
    IWebDriverWrapper driver,
    SpotifyClientUseCase spotifyClientUseCase
)
{
    public async Task AddRadio(
        string scriptFilePath,
        string radioName,
        string maxSongs,
        string jsonPath
    ) {
        string script = fileIo.ReadAllText(scriptFilePath);
        driver.GoToUrl("https://ukradiolive.com/playlists");
        await delayer.Delay(2000);
        ReadOnlyCollection<object> songs =
            (ReadOnlyCollection<object>) driver.ExecuteAsyncScript(script, radioName, maxSongs);
        driver.Quit();

        var artistSongs = songs
            .Select(x => (Dictionary<string, object>) x)
            .Select(x => new ArtistSong(x["artist"].ToString(), x["track"].ToString()));
        
        var artistSongTrackMaps = (await spotifyClientUseCase.GetSongs(artistSongs.ToList(), jsonPath, true))!
            .Where(x => x.Value != null)
            .Select(x => new KeyValuePair<string,TrackObject>(x.Key, x.Value!));

        await spotifyClientUseCase.AddSongsToNewPlaylist(radioName + "-" + DateTime.Now, artistSongTrackMaps.Select(x => x.Value.id));
    }
}