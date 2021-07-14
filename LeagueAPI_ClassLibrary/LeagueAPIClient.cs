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

        public async Task<List<LeagueMatch>> GetMatches(List<string> matchIds, string targetVersion = null)
        {
            List<LeagueMatch> matches = new();
            foreach (string matchId in matchIds)
            {
                JObject obj = await GetJObjectFromResponse(Client, Token, $"https://europe.api.riotgames.com/lol/match/v5/matches/{matchId}");
                LeagueMatch match = new();
                match.gameVersion = obj["info"]["gameVersion"].ToString();
                if (targetVersion != null)
                {
                    int versionComparisonResult = CompareTargetVersionAgainstGameVersion(targetVersion, match.gameVersion);
                    if (versionComparisonResult == 1) break;
                    else if (versionComparisonResult == -1) continue;
                }
                match.matchId = obj["metadata"]["matchId"].ToString();
                match.mapId = int.Parse(obj["info"]["mapId"].ToString());
                match.queueId = int.Parse(obj["info"]["queueId"].ToString());

                List<Participant> participants = new();
                JToken arr = obj["info"]["participants"];
                foreach (JToken p in arr)
                {
                    participants.Add(new Participant()
                    {
                        championId = int.Parse(p["championId"].ToString()),
                        item0 = int.Parse(p["item0"].ToString()),
                        item1 = int.Parse(p["item1"].ToString()),
                        item2 = int.Parse(p["item2"].ToString()),
                        item3 = int.Parse(p["item3"].ToString()),
                        item4 = int.Parse(p["item4"].ToString()),
                        item5 = int.Parse(p["item5"].ToString()),
                        item6 = int.Parse(p["item6"].ToString()),
                        perk1_1 = int.Parse(p["perks"]["styles"][0]["selections"][0]["perk"].ToString()),
                        perk1_2 = int.Parse(p["perks"]["styles"][0]["selections"][1]["perk"].ToString()),
                        perk1_3 = int.Parse(p["perks"]["styles"][0]["selections"][2]["perk"].ToString()),
                        perk1_4 = int.Parse(p["perks"]["styles"][0]["selections"][3]["perk"].ToString()),
                        perk2_1 = int.Parse(p["perks"]["styles"][1]["selections"][0]["perk"].ToString()),
                        perk2_2 = int.Parse(p["perks"]["styles"][1]["selections"][1]["perk"].ToString()),
                        perkTree_1 = int.Parse(p["perks"]["styles"][0]["style"].ToString()),
                        perkTree_2 = int.Parse(p["perks"]["styles"][1]["style"].ToString()),
                        statPerkDefense = int.Parse(p["perks"]["statPerks"]["defense"].ToString()),
                        statPerkFlex = int.Parse(p["perks"]["statPerks"]["flex"].ToString()),
                        statPerkOffense = int.Parse(p["perks"]["statPerks"]["offense"].ToString()),
                        summoner1Id = int.Parse(p["summoner1Id"].ToString()),
                        summoner2Id = int.Parse(p["summoner2Id"].ToString()),
                        win = bool.Parse(p["win"].ToString())
                    });
                }
                match.participants = participants;
                matches.Add(match);
            }
            return matches;
        }


        /// <summary>
        /// Gets a result saying if the target version is greater than the game version.
        /// </summary>
        /// <param name="targetVersion"></param>
        /// <param name="gameVersion"></param>
        /// <returns>1 if target is greater than game version, 0 if equal, -1 if lesser.</returns>
        public static int CompareTargetVersionAgainstGameVersion(string targetVersion, string gameVersion)
        {
            string[] targetVersionArray = targetVersion.Split('.');
            string[] gameVersionArray = gameVersion.Split('.');

            int maxLength = Math.Max(targetVersionArray.Length, gameVersionArray.Length);
            for (int i = 0; i < maxLength; i++)
            {
                int targetVersionCharDigit = i > targetVersionArray.Length - 1 ? 0 : int.Parse(targetVersionArray[i].ToString());
                int gameVersionCharDigit = i > gameVersionArray.Length - 1 ? 0 : int.Parse(gameVersionArray[i].ToString());
                if (targetVersionCharDigit != gameVersionCharDigit) return targetVersionCharDigit > gameVersionCharDigit ? 1 : -1;
            }
            return 0;
        }
    }
}