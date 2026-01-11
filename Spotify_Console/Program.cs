// See https://aka.ms/new-console-template for more information

using Common_ClassLibrary;
using OpenQA.Selenium.Chrome;
using Spotify_ClassLibrary;
using Spotify_Console;
using ChromeDriverService = Common_ClassLibrary.ChromeDriverService;

IFileIO fileIo = new RealFileIO();
IDelayer delayer = new RealDelayer();
IHttpClient http = new RealHttpClient();
ILogger logger = new Logger_Console();
try
{
    var dict = GetCommandAndValuesDictionary(args);
    var credentialFilePath = dict["--credentials-file"];
    var client = GetClientFromCredentialFiles(credentialFilePath);
    var clientUseCase = new SpotifyClientUseCase(client: client, logger: logger, fileIo: fileIo, http: http);
    await client.Initialise();

    string GetJsonPath()
    {
        return dict["--jsonPath"];
    }

    switch (dict["command"])
    {
        case "uk-radio-live-add-radio":
        {
            UkRadioLiveAddRadioUseCase useCase = new(
                fileIo: fileIo,
                delayer: delayer,
                driver: await GetChromeDriver(GetJsonPath()),
                spotifyClientUseCase: clientUseCase
            );
            await useCase.AddRadio(
                scriptFilePath: dict["--script-file"],
                radioName: dict["--radio-name"],
                maxSongs: dict["--max-songs"],
                jsonPath: GetJsonPath()
            );
            break;
        }
        case "online-radio-box-use-case":
        {
            OnlineRadioBoxUseCase useCase = new(spotifyClientUseCase: clientUseCase, http: http);
            await useCase.AddRadio(radioName: dict["--radio-name"], jsonPath: GetJsonPath());
            break;
        }
        case "spotify-merge-playlists":
        {
            var playlists = dict["--playlists"].Split(separator: ",", options: StringSplitOptions.RemoveEmptyEntries)
                .Distinct().ToArray();
            var finalPlaylist = dict["--final-playlist"];
            await clientUseCase.MergePlaylists(playlists: playlists, finalPlaylist: finalPlaylist);
            break;
        }
        case "billboard":
        {
            BillboardUseCase useCase = new(spotifyClientUseCase: clientUseCase, http: http);
            await useCase.DoWork(GetJsonPath());
            break;
        }
        case "kworbNet":
        {
            KworbNetUseCase useCase = new(
                fileIo: fileIo,
                logger: logger,
                delayer: delayer,
                spotifyClient: client,
                spotifyClientUseCase: clientUseCase,
                chromeDriver: await GetChromeDriver(GetJsonPath()),
                http: http
            );
            await useCase.DoWork(GetJsonPath());
            break;
        }
        case "ukSingles":
        {
            UkSinglesScrapperUseCase useCase = new(
                fileIo: fileIo,
                logger: logger,
                delayer: delayer,
                driver: await GetChromeDriver(GetJsonPath()),
                http: http
            );
            await useCase.Scrap(GetJsonPath());
            break;
        }
    }

    async Task<IWebDriverWrapper> GetChromeDriver(string savePath)
    {
        await ChromeDriverService.GetLatestChromeDriver(
            logger: logger,
            httpClient: http,
            savePath: savePath,
            fileIo: fileIo
        );
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
        ChromeDriver chromeDriver = new(
            chromeDriverDirectory: $"{savePath}\\chromeDriverFolder\\chromedriver-win64",
            options: options,
            commandTimeout: TimeSpan.FromMinutes(3)
        );
        chromeDriver.Manage().Timeouts().AsynchronousJavaScript = new TimeSpan(hours: 0, minutes: 5, seconds: 0);
        IWebDriverWrapper webDriverWrapper = new ChromeDriverWrapper(chromeDriver);
        return webDriverWrapper;
    }
}
catch (Exception ex)
{
    logger.Log(ex.ToString());
}
Console.WriteLine("Press any key to finish...");
Console.ReadKey();

SpotifyClient GetClientFromCredentialFiles(string filePath)
{
    //Found in dashboard
    var clientId = "";
    var clientSecret = "";
    var callback = "";

    //Retrieved with authorize code
    //If you need a new token, go to the url to authorize app scope access. Get code from response url
    //and use the client function with the code to get new token
    //string authorizeUrl = SpotifyAuthorizationHelper.GetEncodedAuthorizeUrl(clientId, callback);
    //string authorizeCode = "AQDwCqIGMuhtDkiHRvANtbJ1oHeNQRg...";
    //await client.SetAccessTokenFromAuthorizeCode(authorizeCode);

    var accessToken = "";
    var tokenType = "";
    var expiresIn = 0;
    var refreshToken = "";
    var scope = "";

    var credentialLines = fileIo.ReadLines(filePath);
    foreach (var line in credentialLines)
    {
        var lineSplit = line.Split(separator: "=", options: StringSplitOptions.RemoveEmptyEntries);
        var key = lineSplit[0];
        var value = lineSplit[1];
        if (key == "clientId")
        {
            clientId = value;
        }
        else if (key == "clientSecret")
        {
            clientSecret = value;
        }
        else if (key == "callback")
        {
            callback = value;
        }
        else if (key == "accessToken")
        {
            accessToken = value;
        }
        else if (key == "tokenType")
        {
            tokenType = value;
        }
        else if (key == "expiresIn")
        {
            expiresIn = int.Parse(value);
        }
        else if (key == "refreshToken")
        {
            refreshToken = value;
        }
        else if (key == "scope")
        {
            scope = value;
        }
    }

    SpotifyCredentials credentials = new(
        accessToken: accessToken,
        tokenType: tokenType,
        expiresIn: expiresIn,
        refreshToken: refreshToken,
        scope: scope
    );

    SpotifyClient spotifyClient = new(
        http: http,
        delayer: delayer,
        clientId: clientId,
        clientSecret: clientSecret,
        callback: callback,
        credentials: credentials
    );
    return spotifyClient;
}

Dictionary<string, string> GetCommandAndValuesDictionary(string[] strings)
{
    Dictionary<string, string> commandsAndValues = new();
    for (var i = 0; i < strings.Length; i++)
    {
        var arg = strings[i];
        if (i == 0)
        {
            commandsAndValues.Add(key: "command", value: arg);
        }
        else if (arg.StartsWith("--"))
        {
            commandsAndValues.Add(key: arg, value: strings[i + 1]);
        }
    }

    return commandsAndValues;
}