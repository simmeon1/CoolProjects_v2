using ClassLibrary.SpotifyClasses;
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
        private IJsonParser JsonParser { get; set; }

        public SpotifyAPIClient(ISpotifyCredentials credentials, IHttpClient httpClient, IJsonParser jsonParser, IDateTimeProvider dateTimeProvider, ISpotifyTokenWorker tokenWorker, ISpotifyToken token = null)
        {
            HttpClient = httpClient;
            Credentials = credentials;
            JsonParser = jsonParser;
            Token = token;
            TokenWorker = tokenWorker;
            DateTimeProvider = dateTimeProvider;
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

        private void ThrowExceptionDueToBadAPIResponse(string leadingMessage, HttpResponseMessage response, string responseText)
        {
            throw new ArgumentException($"{leadingMessage}{Environment.NewLine}The details are as follows:{Environment.NewLine}" +
                                $"Status code: {response.StatusCode}{Environment.NewLine}" +
                                $"Error details: {responseText}");
        }

        private ISpotifyToken GetSpotifyTokenFromResponse(string responseText, DateTime dateTimeJustBeforeRequest)
        {
            string accessToken = JsonParser.GetPropertyValue<string>(responseText, "access_token");
            int expiresIn = JsonParser.GetPropertyValue<int>(responseText, "expires_in");
            string tokenType = JsonParser.GetPropertyValue<string>(responseText, "token_type");
            string scope = JsonParser.GetPropertyValue<string>(responseText, "scope");
            return TokenWorker.CreateTokenObject(accessToken, expiresIn, scope, tokenType, dateTimeJustBeforeRequest);
        }

        public bool ConfirmTokenAgainstCurrentToken(ISpotifyToken tokenToConfirm)
        {
            return TokenWorker.TokensHaveTheSameData(tokenToConfirm, Token);
        }

        public async Task<List<Playlist>> GetPlaylists()
        {
            await UpdateAccessTokenIfNeededAsync();
            HttpRequestMessage requestMessage = new(HttpMethod.Get, "https://api.spotify.com/v1/me/playlists");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(Token.GetTokenType(), Token.GetAccessToken());
            requestMessage.Content = new StringContent("");
            requestMessage.Content.Headers.ContentType = new("application/json");
            HttpResponseMessage response = await HttpClient.SendRequest(requestMessage);
            string responseText = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK) ThrowExceptionDueToBadAPIResponse("The GetPlaylists request was not successful.", response, responseText);
            List<Playlist> playlists = GetPlaylistsFromJson(responseText);
            return playlists;
        }

        public List<Playlist> GetPlaylistsFromJson(string json)
        {
            List<Playlist> playlists = new();
            List<string> playlistJsons = JsonParser.GetArrayJsons(json, "items");
            foreach (string playlistJson in playlistJsons)
            {
                playlists.Add(new Playlist(
                    JsonParser.GetPropertyValue<string>(playlistJson, "id"),
                    JsonParser.GetPropertyValue<string>(playlistJson, "name"),
                    JsonParser.GetPropertyValue<string>(playlistJson, "description")));
            }
            return playlists;
        }

        private async Task UpdateAccessTokenIfNeededAsync()
        {
            if (!TokenWorker.TokenIsStillValid(Token, DateTimeProvider)) await GetAndSetNewAccessToken();
        }
    }
}
