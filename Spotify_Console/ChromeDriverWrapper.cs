using OpenQA.Selenium.Chrome;
using Spotify_ClassLibrary;

namespace Spotify_Console;

public class ChromeDriverWrapper(ChromeDriver driver) : IWebDriverWrapper
{
    public void GoToUrl(string url) => driver.Navigate().GoToUrl(url);
    public void Quit() => driver.Quit();
    public object ExecuteScript(string script, params object[] args) => driver.ExecuteScript(script, args);
    public object ExecuteAsyncScript(string script, params object[] args) => driver.ExecuteAsyncScript(script, args);
}