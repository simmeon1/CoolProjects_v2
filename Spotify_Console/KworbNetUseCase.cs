using Common_ClassLibrary;
using Spotify_ClassLibrary;

public class KworbNetUseCase(
    IFileIO FileIo,
    ILogger Logger,
    IDelayer Delayer,
    SpotifyClient SpotifyClient,
    SpotifyClientUseCase spotifyClientUseCase,
    IWebDriverWrapper ChromeDriver
) {
    public async Task DoWork(string jsonPath)
    {
        
    }
}