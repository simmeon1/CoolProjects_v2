namespace Spotify_ClassLibrary;

public interface IWebDriverWrapper
{
    void GoToUrl(string url);
    void Quit();
    object ExecuteScript(string script, params object[] args);
    object ExecuteAsyncScript(string script, params object[] args);
}