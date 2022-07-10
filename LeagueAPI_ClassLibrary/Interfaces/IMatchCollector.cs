using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface IMatchCollector
    {
        Task<List<LeagueMatch>> GetMatches(
            string startPuuid,
            List<string> rangeOfTargetVersions,
            int maxCount,
            string startMatchId = "",
            List<LeagueMatch> alreadyScannedMatches = null
        );
    }
}