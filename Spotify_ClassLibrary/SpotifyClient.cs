using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using Common_ClassLibrary;
using Newtonsoft.Json.Linq;

namespace Spotify_ClassLibrary;

public class SpotifyClient
{
    private const string Root = "https://api.spotify.com/v1/";
    private readonly IHttpClient http;
    private readonly IDelayer delayer;
    private readonly string clientId;
    private readonly string clientSecret;
    private readonly string callback;
    private SpotifyCredentials credentials;

    public SpotifyClient(IHttpClient http, IDelayer delayer, string clientId, string clientSecret, string callback, SpotifyCredentials credentials = null)
    {
        this.http = http;
        this.delayer = delayer;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
        this.callback = callback;
        this.credentials = credentials;
    }
    
    public async Task Initialise()
    {
        await SetAccessTokenFromRefreshToken();
    }

    public async Task<string> GetIdOfFirstResultOfSearch(string query)
    {
        JObject responseJson = await GetJObjectFromRequestResponse(
            HttpMethod.Get,
            $"{Root}search?q={HttpUtility.UrlEncode(query)}&type=track&limit=1"
        );
        JToken items = responseJson["tracks"]["items"];
        return !items.Any() ? "" : items[0]["id"].ToString();
    }

    public async Task<string> GetUserId()
    {
        JObject responseJson = await GetJObjectFromRequestResponse(HttpMethod.Get, $"{Root}me");
        return responseJson["id"].ToString();
    }

    public async Task<string> CreatePlaylist(string name, string userId)
    {
        Dictionary<string, object> options = new()
        {
            {"name", name},
            {"public", false}
        };
        JObject responseJson = await GetJObjectFromRequestResponse(
            HttpMethod.Post,
            $"{Root}users/{userId}/playlists",
            options
        );
        return responseJson["id"].ToString();
    }

    public async Task AddSongsToPlaylist(string playlistId, List<string> songIds)
    {
        List<string> partOfSongIds = new();
        while (songIds.Any())
        {
            partOfSongIds = songIds.Take(100).ToList();
            songIds.RemoveRange(0, partOfSongIds.Count);

            for (int i = 0; i < partOfSongIds.Count; i++) partOfSongIds[i] = "spotify:track:" + partOfSongIds[i];

            await GetJObjectFromRequestResponse(
                HttpMethod.Post,
                $"{Root}playlists/{playlistId}/tracks",
                new Dictionary<string, object> {{"uris", partOfSongIds}}
            );
        }
    }
    
    public async Task<List<string>> GetPlaylistTracks(string playlist)
    {
        List<string> result = new();
        int offset = 0;
        int limit = 100;

        while (true)
        {
            JObject responseJson = await GetJObjectFromRequestResponse(
                HttpMethod.Get,
                $"{Root}playlists/{playlist}/tracks?fields=items(track(id))&limit={limit}&offset={offset}"
            );
            JToken items = responseJson["items"];
            if (!items.Any()) return result;

            foreach (JToken item in items)
            {
                result.Add(item["track"]["id"].ToString());
            }
            offset += limit;
        }
    }

    public async Task SetAccessTokenFromAuthorizeCode(string authorizeCode)
    {
        HttpRequestMessage request = new(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {SpotifyAuthorizationHelper.GetBase64EncodedString(clientId, clientSecret)}");
        List<string> contentList = new()
        {
            "grant_type=authorization_code",
            $"code={authorizeCode}",
            $"redirect_uri={callback}"
        };
        request.Content = new StringContent(string.Join("&", contentList));
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded"); 
        
        string responseText = await SendRequest(request);
        JObject tokenResponse = JObject.Parse(responseText);
        credentials = new SpotifyCredentials(
            tokenResponse["access_token"].ToString(),
            tokenResponse["token_type"].ToString(),
            int.Parse(tokenResponse["expires_in"].ToString()),
            tokenResponse["refresh_token"].ToString(),
            tokenResponse["scope"].ToString()
        );
    }

    private async Task SetAccessTokenFromRefreshToken()
    {
        HttpRequestMessage request = new(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {SpotifyAuthorizationHelper.GetBase64EncodedString(clientId, clientSecret)}");

        List<string> contentList = new()
        {
            "grant_type=refresh_token",
            $"refresh_token={credentials.RefreshToken}"
        };
        request.Content = new StringContent(string.Join("&", contentList));
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded"); 
        string responseText = await SendRequest(request);
        JObject tokenResponse = JObject.Parse(responseText);
        credentials = new SpotifyCredentials(
            tokenResponse["access_token"].ToString(),
            tokenResponse["token_type"].ToString(),
            int.Parse(tokenResponse["expires_in"].ToString()),
            credentials.RefreshToken,
            tokenResponse["scope"].ToString()
        );
    }
    
    private async Task<JObject> GetJObjectFromRequestResponse(
        HttpMethod httpMethod,
        string requestUri,
        Dictionary<string, object>? content = null
    )
    {
        HttpRequestMessage requestMessage = CreateRequestMessage(httpMethod, requestUri, content);
        string response = await SendRequest(requestMessage);
        return JObject.Parse(response);
    }

    private async Task<string> SendRequest(HttpRequestMessage request)
    {
        HttpResponseMessage response = await http.SendRequest(request);
        HttpStatusCode responseStatusCode = response.StatusCode;
        
        if (responseStatusCode == HttpStatusCode.Unauthorized)
        {
            await SetAccessTokenFromRefreshToken();
            response = await ResendRequest(request);
        }
        
        if (responseStatusCode == HttpStatusCode.TooManyRequests)
        {
            await delayer.Delay(response.Headers.RetryAfter.Delta.Value);
            response = await ResendRequest(request);
        }

        string content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        return content;
    }

    private async Task<HttpResponseMessage> ResendRequest(HttpRequestMessage originalRequest)
    {
        string contentStr = await originalRequest.Content.ReadAsStringAsync();
        Dictionary<string, object>? content = contentStr.IsNullOrEmpty() 
            ? null 
            : JsonSerializer.Deserialize<Dictionary<string, object>>(contentStr);
        
        HttpRequestMessage clonedRequest = CreateRequestMessage(
            originalRequest.Method,
            originalRequest.RequestUri.ToString(),
            content
        );
        return await http.SendRequest(clonedRequest);
    }

    private HttpRequestMessage CreateRequestMessage(
        HttpMethod method,
        string requestUri,
        Dictionary<string, object>? content
    )
    {
        HttpRequestMessage requestMessage = new(method, requestUri);
        string contentStr = content == null ? "" : JsonSerializer.Serialize(content);
        requestMessage.Content = new StringContent(contentStr);
        requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        requestMessage.Headers.TryAddWithoutValidation(
        "Authorization",
        $"{credentials.TokenType} {credentials.AccessToken}"
        );
        return requestMessage;
    }
}