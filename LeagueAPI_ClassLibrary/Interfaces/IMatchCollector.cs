using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface IMatchCollector
    {
        Task<List<LeagueMatch>> GetMatches(string startPuuid, int queueId, List<string> rangeOfTargetVersions, int maxCount, List<LeagueMatch> alreadyScannedMatches = null);
    }
}