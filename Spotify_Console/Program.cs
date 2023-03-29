// See https://aka.ms/new-console-template for more information

using Common_ClassLibrary;
using Spotify_ClassLibrary;

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

IEnumerable<string> credentialLines = File.ReadLines(args[1]);
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

IHttpClient http = new RealHttpClient();
IDelayer delayer = new RealDelayer();
SpotifyCredentials credentials = new (
    accessToken,
    tokenType,
    expiresIn,
    refreshToken,
    scope
);

SpotifyClient client = new(
    http,
    delayer,
    clientId,
    clientSecret,
    callback,
    credentials
);

await client.SetAccessTokenFromRefreshToken();
string userId = await client.GetUserId();
var x = 1;