using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface IMatchCollector
    {
        Task<List<LeagueMatch>> GetMatches(string startPuuid, int queueId, string targetVersion = null, int maxCount = 0);
    }
}