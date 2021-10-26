using Common_ClassLibrary;
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
        private IHttpClient Client { get; set; }
        private string Token { get; set; }
        private IDelayer Delayer { get; set; }
        private ILogger Logger { get; set; }
        public LeagueAPIClient(IHttpClient client, string token, IDelayer delayer, ILogger logger)
        {
            Client = client;
            Token = token;
            Delayer = delayer;
            Logger = logger;
        }

        public async Task<Account> GetAccountBySummonerName(string summonerName)
        {
            JObject obj = await GetJObjectFromResponse($"https://euw1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}");
            return obj == null ? null : new Account((string)obj["id"], (string)obj["accountId"], (string)obj["puuid"], (string)obj["name"]);
        }

        private async Task<JObject> GetJObjectFromResponse(string uri)
        {
            string response = await GetResponse(uri);
            return response == null ? null : JObject.Parse(response);
        }

        private async Task<JArray> GetJArrayFromResponse(string uri)
        {
            string response = await GetResponse(uri);
            return response == null ? new JArray() : JArray.Parse(response);
        }

        private async Task<string> GetResponse(string uri)
        {
            HttpResponseMessage response = await SendRequest(uri);
            string responseMessage = await response.Content.ReadAsStringAsync();
            if (((int)response.StatusCode).ToString()[0] == '2') return responseMessage;
            if ((int)response.StatusCode >= 400 && (int)response.StatusCode != (int)HttpStatusCode.Forbidden) return null;
            throw new InvalidOperationException(
                $"The request was not successful.{Environment.NewLine}" +
                $"URI: {uri}.{Environment.NewLine}" +
                $"Status code: {response.StatusCode}.{Environment.NewLine}" +
                $"Message: {responseMessage}"
            );
        }

        private async Task<HttpResponseMessage> SendRequest(string uri)
        {
            while (true)
            {
                HttpRequestMessage message = GetMessageReadyWithToken(uri);
                try
                {
                    HttpResponseMessage response = await Client.SendRequest(message);
                    while (response.StatusCode == HttpStatusCode.TooManyRequests || (int)response.StatusCode >= 500)
                    {
                        double millisecondsToWait = 1000;
                        if (response.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            try
                            {
                                millisecondsToWait = response.Headers.RetryAfter.Delta.Value.TotalMilliseconds;
                            }
                            catch (Exception) { }
                        }

                        Logger.Log($"Last request failed due to status code {response.StatusCode}.");
                        LogTimeToWaitBeforeRetrying(TimeSpan.FromMilliseconds(millisecondsToWait).TotalSeconds);
                        await Delayer.Delay(Convert.ToInt32(millisecondsToWait));
                        message = GetMessageReadyWithToken(uri);
                        response = await Client.SendRequest(message);
                    }
                    return response;
                }
                catch (Exception ex)
                {
                    Logger.Log($"There was an error with sending request {uri}. Details:");
                    Logger.Log(ex.Message);
                    double secondsToWait = 10;
                    LogTimeToWaitBeforeRetrying(secondsToWait);
                    await Delayer.Delay(Convert.ToInt32(TimeSpan.FromSeconds(secondsToWait).TotalMilliseconds));
                }
            }
        }

        private void LogTimeToWaitBeforeRetrying(double timeToWaitInSeconds)
        {
            Logger.Log($"Waiting {timeToWaitInSeconds} seconds before retrying (at {DateTime.Now.AddSeconds(timeToWaitInSeconds)}).");
        }

        private HttpRequestMessage GetMessageReadyWithToken(string uri)
        {
            HttpRequestMessage message = new(HttpMethod.Get, uri);
            message.Headers.Add("X-Riot-Token", Token);
            return message;
        }

        public async Task<List<string>> GetMatchIds(int queueId, string puuid)
        {
            JArray array = await GetJArrayFromResponse($"https://europe.api.riotgames.com/lol/match/v5/matches/by-puuid/{puuid}/ids?queue={queueId}&start=0&count=100");
            List<string> ids = new();
            foreach (JToken id in array) ids.Add(id.ToString());
            return ids;
        }

        public async Task<LeagueMatch> GetMatch(string matchId)
        {
            JObject obj = await GetJObjectFromResponse($"https://europe.api.riotgames.com/lol/match/v5/matches/{matchId}");
            if (obj == null) return null;
            LeagueMatch match = new();
            match.gameVersion = obj["info"]["gameVersion"].ToString();
            match.matchId = obj["metadata"]["matchId"].ToString();
            match.mapId = int.Parse(obj["info"]["mapId"].ToString());
            match.queueId = int.Parse(obj["info"]["queueId"].ToString());

            long duration = long.Parse(obj["info"]["gameDuration"].ToString());
            match.duration = obj["info"]["gameEndTimestamp"] == null ? TimeSpan.FromMilliseconds(duration) : TimeSpan.FromSeconds(duration);

            List<Participant> participants = new();
            JToken arr = obj["info"]["participants"];
            foreach (JToken p in arr)
            {
                Participant participant = new();
                participant.championId = int.Parse(p["championId"].ToString());
                participant.puuid = p["puuid"].ToString();
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