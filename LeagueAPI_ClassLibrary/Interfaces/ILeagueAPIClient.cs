using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface ILeagueAPIClient
    {
        Task<Account> GetAccountBySummonerName(string summonerName);
        Task<LeagueMatch> GetMatch(string matchId);
        Task<List<string>> GetMatchIds(string puuid, int queueId = 0);
        Task<List<string>> GetLatestVersions();
        Task<List<string>> GetParsedListOfVersions(List<string> unparsedVersions);
        Task<string> GetNameOfQueue(int queueId);
    }
}