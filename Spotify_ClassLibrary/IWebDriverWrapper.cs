using System.Collections.ObjectModel;

namespace Spotify_ClassLibrary;
public interface IWebDriverWrapper
{
    void GoToUrl(string url);
    object ExecuteAsyncScript(string script, params object[] args);
}