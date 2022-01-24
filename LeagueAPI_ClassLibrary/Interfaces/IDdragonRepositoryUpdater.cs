using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface IDdragonRepositoryUpdater
    {
        Task GetLatestDdragonFiles();
    }
}