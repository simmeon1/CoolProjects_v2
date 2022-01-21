using Common_ClassLibrary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface IDdragonRepositoryUpdater
    {
        Task GetLatestDdragonFiles();
        Task<List<string>> GetParsedListOfVersions(List<string> unparsedVersions);
    }
}