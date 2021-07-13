using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class LeagueAPIClient
    {
        public IHttpClient Client { get; set; }
        public string Token { get; set; }
        public Account Account { get; set; }

        private LeagueAPIClient(IHttpClient client, string token, Account account)
        {
            Client = client;
            Token = token;
            Account = account;
        }

        public static async Task<LeagueAPIClient> GetClientInstance(IHttpClient client, string token, string summonerName, Account account = null)
        {
            Account acc = account ?? await GetAccountBySummonerName(client, token, summonerName);
            return new LeagueAPIClient(client, token, acc);
        }

        public static async Task<Account> GetAccountBySummonerName(IHttpClient client, string token, string summonerName)
        {
            string uri = $"https://euw1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}";
            HttpRequestMessage message = GetGetMessageReadyWithToken(uri, token);
            HttpResponseMessage response = await client.SendAsync(message);
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

        private static HttpRequestMessage GetGetMessageReadyWithToken(string uri, string token)
        {
            HttpRequestMessage message = new(HttpMethod.Get, uri);
            message.Headers.Add("X-Riot-Token", token);
            return message;
        }
    }
}