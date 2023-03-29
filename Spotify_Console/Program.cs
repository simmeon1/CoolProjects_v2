// See https://aka.ms/new-console-template for more information

using Common_ClassLibrary;
using OpenQA.Selenium.Chrome;
using Spotify_ClassLibrary;
using Spotify_Console;

IFileIO fileIo = new RealFileIO();
IDelayer delayer = new RealDelayer();
IHttpClient http = new RealHttpClient();
ILogger logger = new Logger_Console();

ChromeOptions options = new();
//Tried debugging but no luck
//options.AddArgument("--no-sandbox");
//options.AddArgument("--remote-debugging-port=63342");
//options.AddArgument("--user-data-dir=C:\\D\\ChromeDebugProfile");
//options.AddAdditionalCapability("debuggerAddress", "127.0.0.1:63342");
// options.DebuggerAddress = "127.0.0.1:63341";

//https://seleniumjava.com/2019/09/02/solved-the-http-request-to-the-remote-webdriver-server-for-url-http-localhost52847-session-value-timed-out-after-60-seconds/
//Adding the service/timespan parameters fixed the issue for "the http request to the remote webdriver server for url timed out after 60 seconds" issue
//See RemoteWebDriver.DefaultCommandTimeout property (set by timespan param)
ChromeDriver chromeDriver = new(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
chromeDriver.Manage().Timeouts().AsynchronousJavaScript = new TimeSpan(0,5,0);
IWebDriverWrapper driver = new ChromeDriverWrapper(chromeDriver);

Dictionary<string, string> dict = GetCommandAndValuesDictionary(args);
string credentialFilePath = dict["--credentials-file"];
SpotifyClient client = GetClientFromCredentialFiles(credentialFilePath);

if (dict["command"] == "add-radio")
{
    string path = dict["--script-file"];
    string radio = dict["--radio-name"];
    string maxSongs = dict["--max-songs"];
    AddRadioUseCase useCase = new(fileIo, logger, delayer, client, driver);
    await useCase.AddRadio(path, radio, maxSongs);
    chromeDriver.Quit();
}
Console.WriteLine("Press any key to finish...");
Console.ReadKey();

SpotifyClient GetClientFromCredentialFiles(string filePath)
{
    //Found in dashboard
    string clientId = "";
    string clientSecret = "";
    string callback = "";

    //Retrieved with authorize code
    //If you need a new token, go to the url to authorize app scope access. Get code from response url
    //and use the client function with the code to get new token
    //string authorizeUrl = SpotifyAuthorizationHelper.GetEncodedAuthorizeUrl(clientId, callback);
    //string authorizeCode = "AQDwCqIGMuhtDkiHRvANtbJ1oHeNQRg...";
    //await client.SetAccessTokenFromAuthorizeCode(authorizeCode);

    string accessToken = "";
    string tokenType = "";
    int expiresIn = 0;
    string refreshToken = "";
    string scope = "";

    IEnumerable<string> credentialLines = fileIo.ReadLines(filePath);
    foreach (string line in credentialLines)
    {
        string[] lineSplit = line.Split("=", StringSplitOptions.RemoveEmptyEntries);
        string key = lineSplit[0];
        string value = lineSplit[1];
        if (key == "clientId") clientId = value;
        else if (key == "clientSecret") clientSecret = value;
        else if (key == "callback") callback = value;
        else if (key == "accessToken") accessToken = value;
        else if (key == "tokenType") tokenType = value;
        else if (key == "expiresIn") expiresIn = int.Parse(value);
        else if (key == "refreshToken") refreshToken = value;
        else if (key == "scope") scope = value;
    }
    
    SpotifyCredentials credentials = new(
        accessToken,
        tokenType,
        expiresIn,
        refreshToken,
        scope
    );

    SpotifyClient spotifyClient = new(
        http,
        delayer,
        clientId,
        clientSecret,
        callback,
        credentials
    );
    return spotifyClient;
}

Dictionary<string, string> GetCommandAndValuesDictionary(string[] strings)
{
    Dictionary<string, string> commandsAndValues = new();
    for (int i = 0; i < strings.Length; i++)
    {
        string arg = strings[i];
        if (i == 0)
        {
            commandsAndValues.Add("command", arg);
        } 
        else if (arg.StartsWith("--"))
        {
            commandsAndValues.Add(arg, strings[i + 1]);
        }
    }

    return commandsAndValues;
}