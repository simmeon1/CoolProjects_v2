using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class LeagueAPIClient
    {
        private IHttpClient Client { get; set; }
        private string Token { get; set; }

        public LeagueAPIClient(IHttpClient client, string token)
        {
            Client = client;
            Token = token;
        }

        public async Task<Account> GetAccountBySummonerName(string summonerName)
        {
            string uri = $"https://euw1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}";
            HttpRequestMessage message = GetGetMessageReadyWithToken(uri);
            HttpResponseMessage response = await Client.SendAsync(message);
            string responseMessage = await response.Content.ReadAsStringAsync();
            ThrowExceptionIfRequestIsNotOK(uri, response, responseMessage);

            JObject obj = JObject.Parse(responseMessage);
            return new Account((string)obj["id"], (string)obj["accountId"], (string)obj["puuid"], (string)obj["name"]);
        }

        private static void ThrowExceptionIfRequestIsNotOK(string uri, HttpResponseMessage response, string responseMessage)
        {
            if (response.StatusCode == HttpStatusCode.OK) return;
            throw new InvalidOperationException(
                $"The request was not successful.{Environment.NewLine}" +
                $"URI: {uri}.{Environment.NewLine}" +
                $"Status code: {response.StatusCode}.{Environment.NewLine}" +
                $"Message: {responseMessage}"
            );
        }

        private HttpRequestMessage GetGetMessageReadyWithToken(string uri)
        {
            HttpRequestMessage message = new(HttpMethod.Get, uri);
            message.Headers.Add("X-Riot-Token", Token);
            return message;
        }
    }
}