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
        Task<string> GetNameOfQueue(int queueId);
        Task<string> GetDdragonChampions(string version);
        Task<string> GetDdragonItems(string version);
        Task<string> GetDdragonRunes(string version);
        Task<string> GetDdragonStatPerks(string version);
        Task<string> GetDdragonSpells(string version);
    }
}