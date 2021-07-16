﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface ILeagueAPIClient
    {
        Task<Account> GetAccountBySummonerName(string summonerName);
        Task<LeagueMatch> GetMatch(string matchId);
        Task<List<string>> GetMatchIds(int queueId, string puuid);
    }
}