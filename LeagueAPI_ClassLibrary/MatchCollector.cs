using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class MatchCollector
    {
        private ILeagueAPIClient Client { get; set; }
        private ILogger Logger { get; set; }
        public MatchCollector(ILeagueAPIClient client, ILogger logger)
        {
            Client = client;
            Logger = logger;
        }

        /// <summary>
        /// Gets a result saying if the target version is greater than the game version.
        /// </summary>
        /// <param name="targetVersion"></param>
        /// <param name="gameVersion"></param>
        /// <returns>1 if target is greater than game version, 0 if equal, -1 if lesser.</returns>
        public int CompareTargetVersionAgainstGameVersion(string targetVersion, string gameVersion)
        {
            string[] targetVersionArray = targetVersion.Split('.');
            string[] gameVersionArray = gameVersion.Split('.');

            int minLength = Math.Min(targetVersionArray.Length, gameVersionArray.Length);
            for (int i = 0; i < minLength; i++)
            {
                int targetVersionCharDigit = int.Parse(targetVersionArray[i].ToString());
                int gameVersionCharDigit = int.Parse(gameVersionArray[i].ToString());
                if (targetVersionCharDigit != gameVersionCharDigit) return targetVersionCharDigit > gameVersionCharDigit ? 1 : -1;
            }
            return 0;
        }

        public async Task<List<LeagueMatch>> GetMatches(string startPuuid, string targetVersion, int queueId, int maxCount = 0)
        {
            HashSet<string> scannedMatchIds = new();
            Queue<string> puuidQueue = new();
            HashSet<string> puuidsToScan = new();
            List<LeagueMatch> result = new();

            try
            {
                puuidQueue.Enqueue(startPuuid);
                puuidsToScan.Add(startPuuid);
                while (puuidQueue.Count > 0)
                {
                    string puuid = puuidQueue.Dequeue();
                    Logger.Log($"Scanning player {puuid}");
                    List<string> matchIds = await Client.GetMatchIds(queueId, puuid);

                    foreach (string matchId in matchIds)
                    {
                        if (scannedMatchIds.Contains(matchId)) continue;
                        LeagueMatch match = await Client.GetMatch(matchId);
                        scannedMatchIds.Add(matchId);

                        int versionComparisonResult = CompareTargetVersionAgainstGameVersion(targetVersion, match.gameVersion);
                        if (versionComparisonResult == -1) continue;
                        else if (versionComparisonResult == 1) break;

                        result.Add(match);
                        Logger.Log($"Added match {match.matchId}, current count is {result.Count}");
                        if (maxCount > 0 && result.Count >= maxCount) return result;
                        foreach (Participant participant in match.participants)
                        {
                            if (!puuidsToScan.Contains(participant.puuid))
                            {
                                puuidQueue.Enqueue(participant.puuid);
                                puuidsToScan.Add(participant.puuid);
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log("Collections of matches stopped due to exception. Details:");
                Logger.Log(ex.Message);
                Logger.Log($"Matches to be returned: {result.Count}.");
                return result;
            }
        }
    }
}
