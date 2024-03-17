using System.Collections.ObjectModel;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class UkRadioLiveAddRadioUseCase
{
    private readonly IFileIO fileIo;
    private readonly ILogger logger;
    private readonly IDelayer delayer;
    private readonly SpotifyClient client;
    private readonly IWebDriverWrapper driver;
    private readonly SpotifySearchUseCase searchUseCase;

    public UkRadioLiveAddRadioUseCase(IFileIO fileIo, ILogger logger, IDelayer delayer, SpotifyClient client, IWebDriverWrapper driver, SpotifySearchUseCase searchUseCase)
    {
        this.fileIo = fileIo;
        this.logger = logger;
        this.delayer = delayer;
        this.client = client;
        this.driver = driver;
        this.searchUseCase = searchUseCase;
    }

    public async Task AddRadio(
        string scriptFilePath,
        string radioName,
        string maxSongs
    )
    {
        string script = fileIo.ReadAllText(scriptFilePath);
        driver.GoToUrl("https://ukradiolive.com/playlists");
        await delayer.Delay(2000);
        ReadOnlyCollection<object> songs =
            (ReadOnlyCollection<object>) driver.ExecuteAsyncScript(script, radioName, maxSongs);
        driver.Quit();

        var artistSongs = songs
            .Select(x => (Dictionary<string, object>) x)
            .Select(x => new ArtistSong() { artist = x["artist"].ToString(), song = x["track"].ToString() } );
        List<string> songIds = await searchUseCase.GetSongIds(artistSongs.ToList());

        string userId = await client.GetUserId();
        string playlistId = await client.CreatePlaylist(radioName + "-" + DateTime.Now, userId);
        await client.AddSongsToPlaylist(playlistId, songIds);
        logger.Log("Radio playlist added.");
    }
}