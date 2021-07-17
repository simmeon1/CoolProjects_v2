using Newtonsoft.Json.Linq;

namespace LeagueAPI_ClassLibrary
{
    public interface IDDragonRepository
    {
        Champion GetChampion(int id);
        Item GetItem(int id);
    }
}
