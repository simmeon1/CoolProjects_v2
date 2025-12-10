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
        private const string ddragonCdn = "http://ddragon.leagueoflegends.com/cdn/";
        private readonly string token;
        private readonly IHttpClient client;
        private readonly IDelayer delayer;
        private readonly ILogger logger;

        public LeagueAPIClient(IHttpClient client, string token, IDelayer delayer, ILogger logger)
        {
            this.client = client;
            this.token = token;
            this.delayer = delayer;
            this.logger = logger;
        }

        public async Task<Account> GetAccountBySummonerName(string summonerName)
        {
            JObject obj = await GetJObjectFromResponse(
                $"https://euw1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}"
            );
            return obj == null
                ? null
                : new Account(
                    (string) obj["id"],
                    (string) obj["accountId"],
                    (string) obj["puuid"],
                    (string) obj["name"]
                );
        }

        public async Task<SpectatorData> GetSpectatorDataByEncryptedSummonerId(string encryptedSummonerId)
        {
            JObject obj = await GetJObjectFromResponse(
                $"https://euw1.api.riotgames.com/lol/spectator/v4/active-games/by-summoner/{encryptedSummonerId}"
            );
            if (obj == null) return null;

            JToken participantsArr = obj["participants"];
            List<SpectatedParticipant> participants = new();
            foreach (JToken p in participantsArr)
            {
                SpectatedParticipant participant = new()
                {
                    teamId = int.Parse(p["teamId"].ToString()),
                    championId = int.Parse(p["championId"].ToString()),
                    summonerId = p["summonerId"].ToString()
                };
                participants.Add(participant);
            }

            return obj == null ? null : new SpectatorData(participants);
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
            if (response.IsSuccessStatusCode) return responseMessage;

            int responseStatusCode = (int) response.StatusCode;
            if (responseStatusCode >= 400 && responseStatusCode != (int) HttpStatusCode.Forbidden) return null;
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
                    HttpResponseMessage response = await client.SendRequest(message);
                    while (response.StatusCode == HttpStatusCode.TooManyRequests || (int) response.StatusCode >= 500)
                    {
                        double millisecondsToWait = 1000;
                        if (response.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            try
                            {
                                millisecondsToWait = response.Headers.RetryAfter.Delta.Value.TotalMilliseconds;
                            }
                            catch (Exception)
                            {
                            }
                        }

                        logger.Log($"Last request failed due to status code {response.StatusCode}.");
                        LogTimeToWaitBeforeRetrying(TimeSpan.FromMilliseconds(millisecondsToWait).TotalSeconds);
                        await delayer.Delay(Convert.ToInt32(millisecondsToWait));
                        message = GetMessageReadyWithToken(uri);
                        response = await client.SendRequest(message);
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    logger.Log($"There was an error with sending request {uri}. Details:");
                    logger.Log(ex.Message);
                    double secondsToWait = 10;
                    LogTimeToWaitBeforeRetrying(secondsToWait);
                    await delayer.Delay(Convert.ToInt32(TimeSpan.FromSeconds(secondsToWait).TotalMilliseconds));
                }
            }
        }

        private void LogTimeToWaitBeforeRetrying(double timeToWaitInSeconds)
        {
            logger.Log(
                $"Waiting {timeToWaitInSeconds} seconds before retrying (at {DateTime.Now.AddSeconds(timeToWaitInSeconds)})."
            );
        }

        private HttpRequestMessage GetMessageReadyWithToken(string uri)
        {
            HttpRequestMessage message = new(HttpMethod.Get, uri);
            message.Headers.Add("X-Riot-Token", token);
            return message;
        }

        public async Task<List<string>> GetMatchIds(string puuid, int queueId = 0)
        {
            string uri =
                $"https://europe.api.riotgames.com/lol/match/v5/matches/by-puuid/{puuid}/ids?start=0&count=100";
            if (queueId > 0) uri += $"&queue={queueId}";
            JArray array = await GetJArrayFromResponse(uri);
            List<string> ids = new();
            foreach (JToken id in array) ids.Add(id.ToString());
            // ids = new List<string> { "EUW1_7582808885" };
            return ids;
        }

        public async Task<LeagueMatch> GetMatch(string matchId)
        {
            JObject obj =
                await GetJObjectFromResponse($"https://europe.api.riotgames.com/lol/match/v5/matches/{matchId}");
            if (obj == null) return null;
            try
            {
                LeagueMatch match = new()
                {
                    gameVersion = obj["info"]["gameVersion"].ToString(),
                    matchId = obj["metadata"]["matchId"].ToString(),
                    mapId = int.Parse(obj["info"]["mapId"].ToString()),
                    queueId = int.Parse(obj["info"]["queueId"].ToString())
                };

                long duration = long.Parse(obj["info"]["gameDuration"].ToString());
                match.duration = obj["info"]["gameEndTimestamp"] == null
                    ? TimeSpan.FromMilliseconds(duration)
                    : TimeSpan.FromSeconds(duration);

                List<Participant> participants = new();
                JToken arr = obj["info"]["participants"];
                foreach (JToken p in arr)
                {
                    Participant participant = new()
                    {
                        championId = int.Parse(p["championId"].ToString()),
                        puuid = p["puuid"].ToString(),
                        physicalDamageDealtToChampions = int.Parse(p["physicalDamageDealtToChampions"].ToString()),
                        magicDamageDealtToChampions = int.Parse(p["magicDamageDealtToChampions"].ToString()),
                        item0 = int.Parse(p["item0"].ToString()),
                        item1 = int.Parse(p["item1"].ToString()),
                        item2 = int.Parse(p["item2"].ToString()),
                        item3 = int.Parse(p["item3"].ToString()),
                        item4 = int.Parse(p["item4"].ToString()),
                        item5 = int.Parse(p["item5"].ToString()),
                        item6 = int.Parse(p["item6"].ToString()),
                        playerAugment1 = int.Parse(p["playerAugment1"].ToString()),
                        playerAugment2 = int.Parse(p["playerAugment2"].ToString()),
                        playerAugment3 = int.Parse(p["playerAugment3"].ToString()),
                        playerAugment4 = int.Parse(p["playerAugment4"].ToString()),
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
                    };
                    participants.Add(participant);
                }

                match.participants = participants;
                return match;
            }
            catch (Exception e)
            {
                logger.Log($"There was an error parsing match {matchId}. Details: {e.Message}");
                return null;
            }
        }

        public async Task<List<string>> GetLatestVersions()
        {
            return (await GetJArrayFromResponse("https://ddragon.leagueoflegends.com/api/versions.json"))
                .ToObject<List<string>>();
        }

        public async Task<string> GetNameOfQueue(int queueId)
        {
            JArray queues = await GetJArrayFromResponse("https://static.developer.riotgames.com/docs/lol/queues.json");
            foreach (JToken queue in queues)
            {
                if ((int) queue["queueId"] == queueId) return (string) queue["map"];
            }

            return "";
        }

        public async Task<string> GetDdragonChampions(string version)
        {
            return await GetResponse($"{ddragonCdn}{version}/data/en_US/champion.json");
        }

        public async Task<string> GetDdragonItems(string version)
        {
            return await GetResponse($"{ddragonCdn}{version}/data/en_US/item.json");
        }

        public async Task<string> GetDdragonRunes(string version)
        {
            return await GetResponse($"{ddragonCdn}{version}/data/en_US/runesReforged.json");
        }

        public async Task<string> GetDdragonStatPerks(string version)
        {
            return await Task.FromResult(@"{
    '5008': '+9 Adaptive Force',
    '5005': '+10% Attack Speed',
    '5007': '+8 Ability Haste',
    '5002': '+6 Armor',
    '5003': '+8 Magic Resist',
    '5001': '+15-140 Health (based on level)'
  }");
        }

        public async Task<string> GetDdragonSpells(string version)
        {
            return await GetResponse($"{ddragonCdn}{version}/data/en_US/summoner.json");
        }

        public async Task<string> GetDdragonArenaAugments(string version)
        {
            //Trim third set of digits as they are not online
            var digitSets = version.Split(".", StringSplitOptions.RemoveEmptyEntries);
            return await GetResponse($"https://raw.communitydragon.org/{digitSets[0]}.{digitSets[1]}/cdragon/arena/en_us.json");
        }
    }
}