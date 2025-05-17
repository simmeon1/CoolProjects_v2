using System.Net;
using System.Net.Http.Headers;
using System.Web;
using Common_ClassLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

    public SpotifyClient(
        IHttpClient http,
        IDelayer delayer,
        string clientId,
        string clientSecret,
        string callback,
        SpotifyCredentials credentials = null
    )
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

    public async Task<TrackObject?> GetFirstTrackResult(string query)
    {
        var result = await GetDeserializedObjectFromRequestResponse<SearchResult>(
            HttpMethod.Get,
            $"{Root}search?q={HttpUtility.UrlEncode(query)}&type=track&limit=1"
        );

        var items = result.tracks.items;
        return items.FirstOrDefault();
    }

    public async Task<Dictionary<string, FullArtistObject>> GetArtists(IEnumerable<string> artistIds)
    {
        return await GetSeveral<FullArtistObject, GetSeveralArtistsResult>(artistIds, x => x.id, x => x.artists);
    }

    public async Task<Dictionary<string, TrackObject>> GetSongs(IEnumerable<string> songIds)
    {
        return await GetSeveral<TrackObject, GetSeveralTracksResult>(songIds, x => x.id, x => x.tracks);
    }

    private async Task<Dictionary<string, TResult>> GetSeveral<TResult, TSeveral>(
        IEnumerable<string> ids,
        Func<TResult, string> getIdFunc,
        Func<TSeveral, TResult[]> getResults
    ) {
        var list = new List<TResult>();
        foreach (var idSets in ids.Distinct().Chunk(50))
        {
            if (idSets.Length == 0)
            {
                continue;
            }
            var response = await GetDeserializedObjectFromRequestResponse<TSeveral>(
                HttpMethod.Get,
                $"{Root}tracks?ids={idSets.ConcatenateListOfStringsToCommaString()}"
            );
            list.AddRange(getResults(response));
        }
        return list.ToDictionary(getIdFunc, x => x);
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

    public async Task AddSongsToPlaylist(string playlistId, IEnumerable<string> songIds)
    {
        foreach (var ids in songIds.Where(s => !s.IsNullOrEmpty()).Distinct().Select(x => "spotify:track:" + x).Chunk(100))
        {
            await GetJObjectFromRequestResponse(
                HttpMethod.Post,
                $"{Root}playlists/{playlistId}/tracks",
                new Dictionary<string, object> {{"uris", ids}}
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
        request.Headers.TryAddWithoutValidation(
            "Authorization",
            $"Basic {SpotifyAuthorizationHelper.GetBase64EncodedString(clientId, clientSecret)}"
        );
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
        request.Headers.TryAddWithoutValidation(
            "Authorization",
            $"Basic {SpotifyAuthorizationHelper.GetBase64EncodedString(clientId, clientSecret)}"
        );

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
        string response = await SendRequest(httpMethod, requestUri, content);
        return JObject.Parse(response);
    }

    private async Task<T> GetDeserializedObjectFromRequestResponse<T>(
        HttpMethod httpMethod,
        string requestUri,
        Dictionary<string, object>? content = null
    )
    {
        string response = await SendRequest(httpMethod, requestUri, content);
        return JsonConvert.DeserializeObject<T>(response)!;
    }

    private async Task<string> SendRequest(
        HttpMethod httpMethod,
        string requestUri,
        Dictionary<string, object>? content
    )
    {
        HttpRequestMessage requestMessage = CreateRequestMessage(httpMethod, requestUri, content);
        return await SendRequest(requestMessage);
    }

    private async Task<string> SendRequest(HttpRequestMessage request)
    {
        HttpResponseMessage response = await http.SendRequest(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await SetAccessTokenFromRefreshToken();
            response = await ResendRequest(request);
        }

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            TimeSpan retryAfterDelta = response.Headers.RetryAfter.Delta.Value;
            Console.WriteLine(retryAfterDelta + $" ({DateTime.Now.AddSeconds(retryAfterDelta.TotalSeconds)})");
            await delayer.Delay(retryAfterDelta);
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
        // Change me to use SendRequest here
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