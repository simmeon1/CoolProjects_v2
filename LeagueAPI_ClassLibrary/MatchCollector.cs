using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class MatchCollector : IMatchCollector
    {
        private ILeagueAPIClient Client { get; set; }
        private ILogger Logger { get; set; }
        private IMatchCollectorEventHandler MatchCollectorEventHandler { get; set; }

        public MatchCollector(
            ILeagueAPIClient client,
            ILogger logger,
            IMatchCollectorEventHandler matchCollectorEventHandler
        )
        {
            Client = client;
            Logger = logger;
            MatchCollectorEventHandler = matchCollectorEventHandler;
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
                (maxVersion, minVersion) = (minVersion, maxVersion);
            }

            if (CompareVersions(gameVersion, maxVersion) == 1) return -1;
            return CompareVersions(gameVersion, minVersion) == -1 ? 1 : 0;
        }

        private static string GetSeason(string v)
        {
            return Regex.Replace(v, @"^(\w+)\.(\w+).*", "$1");
        }

        private static string GetPatch(string v)
        {
            return Regex.Replace(v, @"^(\w+)\.(\w+).*", "$2");
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

        public async Task<List<LeagueMatch>> GetMatches(
            string defaultPuuid,
            List<string> rangeOfTargetVersions,
            int maxCount,
            string startMatchId = "",
            List<LeagueMatch> alreadyScannedMatches = null
        )
        {
            MatchCollectorEventHandler.CollectingStarted();
            HashSet<string> scannedMatchIds = new();
            Queue<string> puuidQueue = new();
            HashSet<string> puuidsToScan = new();
            List<LeagueMatch> result = new();

            try
            {
                if (alreadyScannedMatches != null)
                {
                    result.AddRange(alreadyScannedMatches);
                    if (result.Count >= maxCount)
                    {
                        MatchCollectorEventHandler.CollectingFinished();
                        return result;
                    }

                    foreach (LeagueMatch match in alreadyScannedMatches) scannedMatchIds.Add(match.matchId);
                    foreach (Participant p in alreadyScannedMatches.Last().participants) puuidsToScan.Add(p.puuid);
                    foreach (string puuidToScan in puuidsToScan) puuidQueue.Enqueue(puuidToScan);
                }

                int queueId = await GetInitialSearchCriteria(defaultPuuid, startMatchId, puuidQueue, puuidsToScan);
                string queueName = await Client.GetNameOfQueue(queueId);

                while (puuidQueue.Count > 0)
                {
                    string puuid = puuidQueue.Dequeue();
                    Logger.Log($"Scanning player {puuid}");
                    List<string> matchIds = await Client.GetMatchIds(puuid, queueId);

                    foreach (string matchId in matchIds)
                    {
                        if (scannedMatchIds.Contains(matchId)) continue;
                        LeagueMatch match = await Client.GetMatch(matchId);
                        scannedMatchIds.Add(matchId);

                        if (
                            match == null ||
                            match.participants == null ||
                            match.participants.Count == 0 ||
                            match.gameVersion.IsNullOrEmpty()
                        )
                        {
                            Logger.Log($"Skipped adding match {matchId} due to bad data from server.");
                            continue;
                        }

                        int versionComparisonResult = CompareTargetVersionAgainstGameVersion(
                            rangeOfTargetVersions,
                            match.gameVersion
                        );
                        if (versionComparisonResult == -1) continue;
                        if (versionComparisonResult == 1) break;

                        result.Add(match);
                        Logger.Log(
                            $"Added match {matchId} (version {match.gameVersion}, queueId {queueId} {queueName}), current count is {result.Count}"
                        );
                        MatchCollectorEventHandler.MatchAdded(result);
                        if (result.Count >= maxCount)
                        {
                            MatchCollectorEventHandler.CollectingFinished();
                            return result;
                        }

                        foreach (Participant participant in match.participants)
                        {
                            if (puuidsToScan.Contains(participant.puuid)) continue;
                            puuidQueue.Enqueue(participant.puuid);
                            puuidsToScan.Add(participant.puuid);
                        }
                    }
                }

                MatchCollectorEventHandler.CollectingFinished();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log("Collections of matches stopped due to exception. Details:");
                Logger.Log(ex.ToString());
                Logger.Log($"Matches to be returned: {result.Count}.");
                MatchCollectorEventHandler.CollectingFinished();
                return result;
            }
        }

        private async Task<int> GetInitialSearchCriteria(
            string defaultPuuid,
            string startMatchId,
            Queue<string> puuidQueue,
            HashSet<string> puuidsToScan
        )
        {
            int queueId;
            string startPuuid;
            if (!startMatchId.IsNullOrEmpty())
            {
                LeagueMatch match = await Client.GetMatch(startMatchId);
                queueId = match.queueId.Value;
                startPuuid = match.participants[0].puuid;
            }
            else
            {
                List<string> matchIds = await Client.GetMatchIds(defaultPuuid);
                LeagueMatch match = await Client.GetMatch(matchIds[0]);
                queueId = match.queueId.Value;
                startPuuid = defaultPuuid;
            }

            puuidQueue.Enqueue(startPuuid);
            puuidsToScan.Add(startPuuid);
            return queueId;
        }
    }
}