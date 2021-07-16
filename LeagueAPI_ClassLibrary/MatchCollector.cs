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
        public MatchCollector(ILeagueAPIClient client)
        {
            Client = client;
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

        public async Task<List<LeagueMatch>> GetMatches(string startPuuid, string targetVersion, int queueId)
        {
            HashSet<string> scannedMatchIds = new();
            HashSet<string> scannedPuuids = new();
            Queue<string> puuidQueue = new();
            List<LeagueMatch> result = new();

            try
            {
                puuidQueue.Enqueue(startPuuid);
                while (puuidQueue.Count > 0)
                {
                    string puuid = puuidQueue.Dequeue();
                    if (scannedPuuids.Contains(puuid)) continue;

                    List<string> matchIds = await Client.GetMatchIds(queueId, puuid);
                    scannedPuuids.Add(puuid);

                    foreach (string matchId in matchIds)
                    {
                        if (scannedMatchIds.Contains(matchId)) continue;
                        LeagueMatch match = await Client.GetMatch(matchId);
                        scannedMatchIds.Add(matchId);

                        int versionComparisonResult = CompareTargetVersionAgainstGameVersion(targetVersion, match.gameVersion);
                        if (versionComparisonResult == -1) continue;
                        else if (versionComparisonResult == 1) break;

                        result.Add(match);
                        foreach (Participant participant in match.participants) if (!scannedPuuids.Contains(participant.puuid)) puuidQueue.Enqueue(participant.puuid);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
    }
}
