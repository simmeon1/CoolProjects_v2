using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface IDDragonRepository
    {
        Task RefreshData(string version);
        Champion GetChampion(int id);
        Item GetItem(int id);
        Rune GetRune(int id);
        StatPerk GetStatPerk(int id);
        Spell GetSpell(int id);
        Item GetItem(string itemName);
    }
}
