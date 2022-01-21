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
            string minVersion = rangeOfTargetVersions.First();
            string maxVersion = rangeOfTargetVersions.Last();
            if (CompareVersions(minVersion, maxVersion) == 1)
            {
                string temp = maxVersion;
                maxVersion = minVersion;
                minVersion = temp;
            }

            if (CompareVersions(gameVersion, maxVersion) == 1) return -1;
            else if (CompareVersions(gameVersion, minVersion) == -1) return 1;
            return 0;
        }

        private static string GetSeason(string v)
        {
            return Regex.Replace(v, @"^(\w+)\.(\w+).*", "$1").ToString();
        }
        
        private static string GetPatch(string v)
        {
            return Regex.Replace(v, @"^(\w+)\.(\w+).*", "$2").ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns>1 if v1 is later than v2, 0 if equal, -1 if earlier.</returns>
        private static int CompareVersions(string v1, string v2)
        {
            int v1Season = int.Parse(GetSeason(v1));
            int v2Season = int.Parse(GetSeason(v2));
            
            if (v1Season > v2Season) return 1;
            if (v1Season < v2Season) return -1;
            
            int v1Patch = int.Parse(GetPatch(v1));
            int v2Patch = int.Parse(GetPatch(v2));

            if (v1Patch > v2Patch) return 1;
            if (v1Patch < v2Patch) return -1;
            return 0;
        }

        public async Task<List<LeagueMatch>> GetMatches(string startPuuid, int queueId, List<string> rangeOfTargetVersions, int maxCount, List<LeagueMatch> alreadyScannedMatches = null)
        {
            HashSet<string> scannedMatchIds = new();
            Queue<string> puuidQueue = new();
            HashSet<string> puuidsToScan = new();
            List<LeagueMatch> result = new();

            try
            {
                puuidQueue.Enqueue(startPuuid);
                puuidsToScan.Add(startPuuid);

                if (alreadyScannedMatches != null)
                {
                    result.AddRange(alreadyScannedMatches);
                    if (result.Count >= maxCount) return result;

                    foreach (LeagueMatch match in alreadyScannedMatches) scannedMatchIds.Add(match.matchId);
                    foreach (Participant p in alreadyScannedMatches.Last().participants) puuidsToScan.Add(p.puuid);
                    foreach (string puuidToScan in puuidsToScan) puuidQueue.Enqueue(puuidToScan);
                }

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

                        if (match == null || match.queueId == 0 || match.participants == null || match.participants.Count == 0)
                        {
                            Logger.Log($"Skipped adding match {match.matchId} due to bad data from server.");
                            continue;
                        }
                        int versionComparisonResult = CompareTargetVersionAgainstGameVersion(rangeOfTargetVersions, match.gameVersion);
                        if (versionComparisonResult == -1) continue;
                        else if (versionComparisonResult == 1) break;

                        result.Add(match);
                        Logger.Log($"Added match {match.matchId} (version {match.gameVersion}, queueId {match.queueId}), current count is {result.Count}");
                        if (result.Count >= maxCount) return result;
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
