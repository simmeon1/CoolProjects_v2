using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class LeagueAPIClient : ILeagueAPIClient
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
            JObject obj = await GetJObjectFromResponse(client, token, $"https://euw1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}");
            return new Account((string)obj["id"], (string)obj["accountId"], (string)obj["puuid"], (string)obj["name"]);
        }

        private static async Task<JObject> GetJObjectFromResponse(IHttpClient client, string token, string uri)
        {
            return JObject.Parse(await GetResponse(client, token, uri));
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

        public async Task<LeagueMatch> GetMatch(string matchId)
        {
            JObject obj = await GetJObjectFromResponse(Client, Token, $"https://europe.api.riotgames.com/lol/match/v5/matches/{matchId}");
            LeagueMatch match = new();
            match.gameVersion = obj["info"]["gameVersion"].ToString();
            match.matchId = obj["metadata"]["matchId"].ToString();
            match.mapId = int.Parse(obj["info"]["mapId"].ToString());
            match.queueId = int.Parse(obj["info"]["queueId"].ToString());

            List<Participant> participants = new();
            JToken arr = obj["info"]["participants"];
            foreach (JToken p in arr)
            {
                Participant participant = new();
                participant.championId = int.Parse(p["championId"].ToString());
                participant.item0 = int.Parse(p["item0"].ToString());
                participant.item1 = int.Parse(p["item1"].ToString());
                participant.item2 = int.Parse(p["item2"].ToString());
                participant.item3 = int.Parse(p["item3"].ToString());
                participant.item4 = int.Parse(p["item4"].ToString());
                participant.item5 = int.Parse(p["item5"].ToString());
                participant.item6 = int.Parse(p["item6"].ToString());
                participant.perk1_1 = int.Parse(p["perks"]["styles"][0]["selections"][0]["perk"].ToString());
                participant.perk1_2 = int.Parse(p["perks"]["styles"][0]["selections"][1]["perk"].ToString());
                participant.perk1_3 = int.Parse(p["perks"]["styles"][0]["selections"][2]["perk"].ToString());
                participant.perk1_4 = int.Parse(p["perks"]["styles"][0]["selections"][3]["perk"].ToString());
                participant.perk2_1 = int.Parse(p["perks"]["styles"][1]["selections"][0]["perk"].ToString());
                participant.perk2_2 = int.Parse(p["perks"]["styles"][1]["selections"][1]["perk"].ToString());
                participant.perkTree_1 = int.Parse(p["perks"]["styles"][0]["style"].ToString());
                participant.perkTree_2 = int.Parse(p["perks"]["styles"][1]["style"].ToString());
                participant.statPerkDefense = int.Parse(p["perks"]["statPerks"]["defense"].ToString());
                participant.statPerkFlex = int.Parse(p["perks"]["statPerks"]["flex"].ToString());
                participant.statPerkOffense = int.Parse(p["perks"]["statPerks"]["offense"].ToString());
                participant.summoner1Id = int.Parse(p["summoner1Id"].ToString());
                participant.summoner2Id = int.Parse(p["summoner2Id"].ToString());
                participant.win = bool.Parse(p["win"].ToString());
                participants.Add(participant);
            }
            match.participants = participants;
            return match;
        }
    }
}