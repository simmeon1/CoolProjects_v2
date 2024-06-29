using OpenQA.Selenium.Chrome;
using Spotify_ClassLibrary;

namespace Spotify_Console;

public class ChromeDriverWrapper : IWebDriverWrapper
{
    private readonly ChromeDriver driver;
    public ChromeDriverWrapper(ChromeDriver driver)
    {
        this.driver = driver;
    }

    public void GoToUrl(string url)
    {
        driver.Navigate().GoToUrl(url);
    }

    public void Quit()
    {
        driver.Quit();
    }
    
    public object ExecuteScript(string script, params object[] args)
    {
        return driver.ExecuteScript(script, args);
    }

    public object ExecuteAsyncScript(string script, params object[] args)
    {
        return driver.ExecuteAsyncScript(script, args);
    }
}