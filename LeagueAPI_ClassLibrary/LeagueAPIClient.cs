using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        public static LeagueAPIClient GetClientInstance(IHttpClient client, string token, Account account)
        {
            return new LeagueAPIClient(client, token, account);
        }

        public static async Task<LeagueAPIClient> GetClientInstance(IHttpClient client, string token, string summonerName)
        {
            return new LeagueAPIClient(client, token, await GetAccountBySummonerName(client, token, summonerName));
        }

        public static async Task<Account> GetAccountBySummonerName(IHttpClient client, string token, string summonerName)
        {
            string responseMessage = await GetResponse(client, token, $"https://euw1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}");
            JObject obj = JObject.Parse(responseMessage);
            return new Account((string)obj["id"], (string)obj["accountId"], (string)obj["puuid"], (string)obj["name"]);
        }

        private static async Task<string> GetResponse(IHttpClient client, string token, string uri)
        {
            HttpRequestMessage message = GetGetMessageReadyWithToken(uri, token);
            HttpResponseMessage response = await client.SendAsync(message);
            string responseMessage = await response.Content.ReadAsStringAsync();
            ThrowExceptionIfRequestIsNotOK(uri, response, responseMessage);
            return responseMessage;
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

        public async Task<List<string>> GetMatchIds(int queueId)
        {
            string responseMessage = await GetResponse(Client, Token, $"https://europe.api.riotgames.com/lol/match/v5/matches/by-puuid/{Account.Puuid}/ids?queue={queueId}&start=0&count=100");
            JArray array = JArray.Parse(responseMessage);
            List<string> ids = new();
            foreach (JToken id in array) ids.Add(id.ToString());
            return ids;
        }
    }
}