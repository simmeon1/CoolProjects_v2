using ClassLibrary.SpotifyClasses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class SpotifyAPIClient
    {
        private IHttpClient HttpClient { get; set; }
        private ISpotifyCredentials Credentials { get; set; }
        private ISpotifyToken Token { get; set; }
        private ISpotifyTokenWorker TokenWorker { get; set; }
        private IDateTimeProvider DateTimeProvider { get; set; }

        public SpotifyAPIClient(IHttpClient httpClient, ISpotifyCredentials credentials, ISpotifyTokenWorker tokenWorker, IDateTimeProvider dateTimeProvider, ISpotifyToken token = null)
        {
            HttpClient = httpClient;
            Credentials = credentials;
            TokenWorker = tokenWorker;
            DateTimeProvider = dateTimeProvider;
            Token = token;
        }

        public async Task GetAndSetNewAccessToken()
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", Credentials.GetEncodedSecret());
            requestMessage.Content = new StringContent($"grant_type=refresh_token&refresh_token={Credentials.GetRefreshToken()}", Encoding.UTF8, "application/x-www-form-urlencoded");
            DateTime dateTimeJustBeforeRequest = DateTimeProvider.GetDateTimeNow();
            HttpResponseMessage response = await HttpClient.SendRequest(requestMessage);
            string responseText = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK) ThrowExceptionDueToBadAPIResponse("The token request was not successful.", response, responseText);
            Token = GetSpotifyTokenFromResponse(responseText, dateTimeJustBeforeRequest);
        }

        private static void ThrowExceptionDueToBadAPIResponse(string leadingMessage, HttpResponseMessage response, string responseText)
        {
            throw new ArgumentException($"{leadingMessage}{Environment.NewLine}The details are as follows:{Environment.NewLine}" +
                                $"Status code: {response.StatusCode}{Environment.NewLine}" +
                                $"Error details: {responseText}");
        }

        private ISpotifyToken GetSpotifyTokenFromResponse(string responseText, DateTime dateTimeJustBeforeRequest)
        {
            JObject tokenResponse = JObject.Parse(responseText);
            string accessToken = tokenResponse["access_token"].ToString();
            int expiresIn = int.Parse(tokenResponse["expires_in"].ToString());
            string tokenType = tokenResponse["token_type"].ToString();
            string scope = tokenResponse["scope"].ToString();
            return TokenWorker.CreateTokenObject(accessToken, expiresIn, scope, tokenType, dateTimeJustBeforeRequest);
        }

        public bool ConfirmTokenAgainstCurrentToken(ISpotifyToken tokenToConfirm)
        {
            return TokenWorker.TokensHaveTheSameData(tokenToConfirm, Token);
        }

        public async Task<List<Playlist>> GetPlaylists()
        {
            HttpRequestMessage requestMessage = await GetRequestMessageWithJsonContentAndAuthorization(HttpMethod.Get, "https://api.spotify.com/v1/me/playlists", "");
            HttpResponseMessage response = await HttpClient.SendRequest(requestMessage);
            string responseText = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK) ThrowExceptionDueToBadAPIResponse("The GetPlaylists request was not successful.", response, responseText);
            return GetPlaylistsFromJson(responseText);
        }

        private async Task<HttpRequestMessage> GetRequestMessageWithJsonContentAndAuthorization(HttpMethod method, string requestUri, string content)
        {
            HttpRequestMessage requestMessage = new(method, requestUri);
            requestMessage.Content = new StringContent(content);
            requestMessage.Content.Headers.ContentType = new("application/json");
            if (!TokenWorker.TokenIsStillValid(Token, DateTimeProvider)) await GetAndSetNewAccessToken();
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(Token.GetTokenType(), Token.GetAccessToken());
            return requestMessage;
        }

        private static List<Playlist> GetPlaylistsFromJson(string json)
        {
            List<Playlist> playlists = new();
            JObject playlistsJson = JObject.Parse(json);
            foreach (JToken playlistJson in playlistsJson["items"])
            {
                playlists.Add(new Playlist(
                    playlistJson["id"].ToString(),
                    playlistJson["name"].ToString(),
                    playlistJson["description"].ToString()));
            }
            return playlists;
        }
    }
}
