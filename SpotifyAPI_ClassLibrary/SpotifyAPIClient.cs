using Common_ClassLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPI_ClassLibrary
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
            DateTime dateTimeJustBeforeRequest = DateTimeProvider.Now();
            string responseText = await SendRequest(requestMessage);
            Token = GetSpotifyTokenFromResponse(responseText, dateTimeJustBeforeRequest);
        }

        public async Task AddSongsToPlaylist(List<SongCLS> songs, string playlistId)
        {
            string template = @"{'uris':" + "arrayPlaceHolder" + @"}";
            List<HttpRequestMessage> requests = new();
            List<string> uris = new();
            foreach (SongCLS song in songs)
            {
                uris.Add($"spotify:track:{song.SpotifyId}");
                if (uris.Count < 100) continue;
                await AddRequestToAddSongs(playlistId, template, requests, uris);
                uris.Clear();
            }
            if (uris.Count > 0) await AddRequestToAddSongs(playlistId, template, requests, uris);
            foreach (HttpRequestMessage request in requests) await SendRequest(request);
        }

        public bool ConfirmTokenAgainstCurrentToken(ISpotifyToken tokenToConfirm)
        {
            return TokenWorker.TokensHaveTheSameData(tokenToConfirm, Token);
        }

        public async Task<List<Playlist>> GetPlaylists()
        {
            HttpRequestMessage requestMessage = await GetRequestMessageWithJsonContentAndAuthorization(HttpMethod.Get, "https://api.spotify.com/v1/me/playlists", "");
            string responseText = await SendRequest(requestMessage);
            return GetPlaylistsFromJson(responseText);
        }

        public async Task AddAudioFeaturesToSongs(List<SongCLS> songs)
        {
            List<string> responses = new();

            const string baseUri = "https://api.spotify.com/v1/audio-features?ids=";
            List<string> idsForRquest = new();
            for (int i = 0; i < songs.Count; i++)
            {
                idsForRquest.Add(songs[i].SpotifyId);
                if (idsForRquest.Count != 100) continue;
                await GetAudioFeaturesForSongs(responses, baseUri + idsForRquest.ConcatenateListOfStringsToCommaString());
                idsForRquest.Clear();
            }
            if (idsForRquest.Count > 0) await GetAudioFeaturesForSongs(responses, baseUri + idsForRquest.ConcatenateListOfStringsToCommaString());

            foreach (string response in responses)
            {
                JObject jo = JObject.Parse(response);
                foreach (JToken songData in jo["audio_features"])
                {
                    if (songData.ToString().IsNullOrEmpty()) continue;
                    string songId = songData["id"].ToString();
                    SongCLS songInList = songs.FirstOrDefault(s => s.SpotifyId.Equals(songId));
                    songInList.Acousticness = (double)songData["acousticness"];
                    songInList.Danceability = (double)songData["danceability"];
                    songInList.Energy = (double)songData["energy"];
                    songInList.Instrumentalness = (double)songData["instrumentalness"];
                    songInList.Liveness = (double)songData["liveness"];
                    songInList.Loudness = (double)songData["loudness"];
                    songInList.Speechiness = (double)songData["speechiness"];
                    songInList.Tempo = (double)songData["tempo"];
                    songInList.Valence = (double)songData["valence"];
                }
            }
        }

        private async Task GetAudioFeaturesForSongs(List<string> responses, string uri)
        {
            HttpRequestMessage requestMessage = await GetRequestMessageWithJsonContentAndAuthorization(HttpMethod.Get, uri, "");
            responses.Add(await SendRequest(requestMessage));
        }

        private async Task AddRequestToAddSongs(string playlistId, string template, List<HttpRequestMessage> requests, List<string> uris)
        {
            requests.Add(await GetRequestMessageWithJsonContentAndAuthorization(
                                HttpMethod.Post, $"https://api.spotify.com/v1/playlists/{playlistId}/tracks",
                                template.Replace("arrayPlaceHolder", uris.SerializeObject()).Replace("'", "\"")));
        }

        private async Task<string> SendRequest(HttpRequestMessage requestMessage)
        {
            HttpResponseMessage response = await HttpClient.SendRequest(requestMessage);
            string responseText = await response.Content.ReadAsStringAsync();
            if (((int)response.StatusCode).ToString()[0] != '2') ThrowExceptionDueToBadAPIResponse("The request was not successful.", response, responseText);
            return responseText;
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
