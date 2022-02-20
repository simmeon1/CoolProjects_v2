using Common_ClassLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MusicPlaylistBuilder
{
    public class SpotifyAPIClient
    {
        private const string root = "https://api.spotify.com/v1/";
        private IHttpClient http;
        private IDelayer delayer;
        private SpotifyCredentials credentials;
        private string token = "aaa";

        public SpotifyAPIClient(IHttpClient http, IDelayer delayer, SpotifyCredentials credentials)
        {
            this.http = http;
            this.delayer = delayer;
            this.credentials = credentials;
        }

        public async Task<string> GetIdOfFirstResultOfSearch(string query)
        {
            JObject responseJson = await GetJObjectFromRequestResponse(HttpMethod.Get, $"{root}search?q={HttpUtility.UrlEncode(query)}&type=track&limit=1");
            JToken items = responseJson["tracks"]["items"];
            return !items.Any() ? "" : items[0]["id"].ToString();
        }

        public async Task<string> GetUserId()
        {
            JObject responseJson = await GetJObjectFromRequestResponse(HttpMethod.Get, $"{root}me");
            return responseJson["id"].ToString();
        }
        
        public async Task<string> CreatePlaylist(string name, string userId)
        {
            Dictionary<string, object> options = new();
            options.Add("name", name);
            options.Add("public", false);
            JObject responseJson = await GetJObjectFromRequestResponse(HttpMethod.Post, $"{root}users/{userId}/playlists", Serialize(options));
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

                Dictionary<string, object> options = new();
                options.Add("uris", partOfSongIds);
                await GetJObjectFromRequestResponse(HttpMethod.Post, $"{root}playlists/{playlistId}/tracks", Serialize(options));
            }
        }

        private static string Serialize(object obj)
        {
            return obj.SerializeObject(Formatting.Indented);
        }

        private async Task<JObject> GetJObjectFromRequestResponse(HttpMethod httpMethod, string requestUri, string content = "")
        {
            return JObject.Parse(await SendRequest(CreateRequestMessage(httpMethod, requestUri, content)));
        }

        private async Task UpdateAccessToken()
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials.EncodedSecret);
            requestMessage.Content = new StringContent($"grant_type=refresh_token&refresh_token={credentials.RefreshToken}", Encoding.UTF8, "application/x-www-form-urlencoded");
            string responseText = await SendRequest(requestMessage);
            JObject tokenResponse = JObject.Parse(responseText);
            token = tokenResponse["access_token"].ToString();
        }

        private async Task<string> SendRequest(HttpRequestMessage request)
        {
            HttpResponseMessage response = await http.SendRequest(request);
            HttpStatusCode responseStatusCode = response.StatusCode;

            if (responseStatusCode == HttpStatusCode.Unauthorized)
            {
                await UpdateAccessToken();
                response = await ResendRequest(request);
            }

            if (responseStatusCode == HttpStatusCode.TooManyRequests)
            {
                await delayer.Delay(response.Headers.RetryAfter.Delta.Value);
                response = await ResendRequest(request);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<HttpResponseMessage> ResendRequest(HttpRequestMessage originalRequest)
        {
            HttpRequestMessage clonedRequest = CreateRequestMessage(originalRequest.Method, originalRequest.RequestUri.ToString(), await originalRequest.Content.ReadAsStringAsync());
            return await http.SendRequest(clonedRequest);
        }

        private HttpRequestMessage CreateRequestMessage(HttpMethod method, string requestUri, string content = "")
        {
            HttpRequestMessage requestMessage = new(method, requestUri);
            requestMessage.Content = new StringContent(content);
            requestMessage.Content.Headers.ContentType = new("application/json");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return requestMessage;
        }
    }
}
