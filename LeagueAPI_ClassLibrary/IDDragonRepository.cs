using Newtonsoft.Json.Linq;

namespace LeagueAPI_ClassLibrary
{
    public interface IDDragonRepository
    {
        JObject GetChampion(int id);
    }
}
