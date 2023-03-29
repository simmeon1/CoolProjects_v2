﻿using System.Collections.ObjectModel;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class AddRadioUseCase
{
    private readonly IFileIO fileIo;
    private readonly ILogger logger;
    private readonly IDelayer delayer;
    private readonly SpotifyClient client;
    private readonly IWebDriverWrapper webDriverWrapper;

    public AddRadioUseCase(IFileIO fileIo, ILogger logger, IDelayer delayer, SpotifyClient client, IWebDriverWrapper webDriverWrapper)
    {
        this.fileIo = fileIo;
        this.logger = logger;
        this.delayer = delayer;
        this.client = client;
        this.webDriverWrapper = webDriverWrapper;
    }

    public async Task AddRadio(
        string scriptFilePath,
        string radioName,
        string maxSongs
    )
    {
        string script = fileIo.ReadAllText(scriptFilePath);
        webDriverWrapper.GoToUrl("https://ukradiolive.com/playlists");
        await delayer.Delay(2000);
        ReadOnlyCollection<object> songs =
            (ReadOnlyCollection<object>) webDriverWrapper.ExecuteAsyncScript(script, radioName, maxSongs);

        await client.Initialise();
        List<string> songIds = new();
        for (int i = 0; i < songs.Count; i++)
        {
            object songObj = songs[i];
            Dictionary<string, object> song = (Dictionary<string, object>) songObj;
            string search = song["artist"] + " " + song["track"];
            songIds.Add(await client.GetIdOfFirstResultOfSearch(search));
            logger.Log($"Retrieved {i + 1} out of {songs.Count} song ids ({search})");
        }

        string userId = await client.GetUserId();
        string playlistId = await client.CreatePlaylist(radioName + "-" + DateTime.Now, userId);
        await client.AddSongsToPlaylist(playlistId, songIds);
        logger.Log("Radio playlist added.");
    }
}