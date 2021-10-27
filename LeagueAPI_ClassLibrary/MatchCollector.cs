using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class MatchCollector : IMatchCollector
    {
        private ILeagueAPIClient Client { get; set; }
        private ILogger Logger { get; set; }
        public MatchCollector(ILeagueAPIClient client, ILogger logger)
        {
            Client = client;
            Logger = logger;
        }

        /// <summary>
        /// Gets a result saying if the target versions are greater than the game version.
        /// </summary>
        /// <param name="rangeOfTargetVersions"></param>
        /// <param name="gameVersion"></param>
        /// <returns>1 if targets are greater than game version, 0 if equal, -1 if lesser.</returns>
        public static int CompareTargetVersionAgainstGameVersion(List<string> rangeOfTargetVersions, string gameVersion)
        {
            int gameVersionInt = GetVersion(gameVersion);
            List<string> rangeOfTargetVersionsOrdered = rangeOfTargetVersions.OrderBy(v => GetVersion(v)).ToList();
            int minVersionInt = GetVersion(rangeOfTargetVersionsOrdered.First());
            int maxVersionInt = GetVersion(rangeOfTargetVersionsOrdered.Last());
            if (gameVersionInt >= minVersionInt && gameVersionInt <= maxVersionInt) return 0;
            else if (gameVersionInt > maxVersionInt) return -1;
            return 1;
        }

        private static int GetVersion(string gameVersion)
        {
            return int.Parse(Regex.Replace(gameVersion, @"\D", "").Substring(0, 4));
        }

        public async Task<List<LeagueMatch>> GetMatches(string startPuuid, int queueId, List<string> rangeOfTargetVersions, int maxCount = 0)
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

                        if (match == null || match.queueId == 0 || match.participants.Count == 0)
                        {
                            Logger.Log($"Skipped adding match {match.matchId} due to bad data from server.");
                            continue;
                        }
                        int versionComparisonResult = CompareTargetVersionAgainstGameVersion(rangeOfTargetVersions, match.gameVersion);
                        if (versionComparisonResult == -1) continue;
                        else if (versionComparisonResult == 1) break;

                        result.Add(match);
                        Logger.Log($"Added match {match.matchId} (version {match.gameVersion}, queueId {match.queueId}), current count is {result.Count}");
                        if (maxCount > 0 && result.Count >= maxCount) return result;
                        foreach (Participant participant in match.participants)
                        {
                            if (puuidsToScan.Contains(participant.puuid)) continue;
                            puuidQueue.Enqueue(participant.puuid);
                            puuidsToScan.Add(participant.puuid);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log("Collections of matches stopped due to exception. Details:");
                Logger.Log(ex.ToString());
                Logger.Log($"Matches to be returned: {result.Count}.");
                return result;
            }
        }
    }
}
