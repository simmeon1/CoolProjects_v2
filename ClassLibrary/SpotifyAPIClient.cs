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
        private ISpotifyToken SpotifyToken { get; set; }
        private IJsonParser JsonParser { get; set; }

        public SpotifyAPIClient(ISpotifyCredentials credentials, IHttpClient httpClient, IJsonParser jsonParser, ISpotifyToken spotifyToken = null)
        {
            HttpClient = httpClient;
            Credentials = credentials;
            JsonParser = jsonParser;
            SpotifyToken = spotifyToken;
        }

        public async Task GetAndSetNewAccessToken()
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", Credentials.GetEncodedSecret());
            requestMessage.Content = new StringContent($"grant_type=refresh_token&refresh_token={Credentials.GetRefreshToken()}", Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await HttpClient.SendRequest(requestMessage);
            string responseText = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ArgumentException($"The token request was not successful. The details are as follows:{Environment.NewLine}" +
                    $"Status code: {response.StatusCode}{Environment.NewLine}" +
                    $"Error details: {responseText}");
            }
            SpotifyToken = GetSpotifyTokenFromResponse(responseText);
        }

        private ISpotifyToken GetSpotifyTokenFromResponse(string responseText)
        {
            JsonParser.SetJsonToParse(responseText);
            string accessToken = JsonParser.GetPropertyValue<string>("access_token");
            int expiresIn = JsonParser.GetPropertyValue<int>("expires_in");
            string tokenType = JsonParser.GetPropertyValue<string>("token_type");
            string scope = JsonParser.GetPropertyValue<string>("scope");
            return new SpotifyToken(accessToken, expiresIn, scope, tokenType);
        }

        public bool ConfirmTokenAgainstCurrentToken(ISpotifyToken tokenToConfirm)
        {
            return ISpotifyTokenWorker.TokensHaveTheSameData(tokenToConfirm, SpotifyToken);
        }
    }
}
