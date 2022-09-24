using Newtonsoft.Json.Linq;

namespace LeagueAPI_ClassLibrary
{
    public interface IDDragonRepository
    {
        Champion GetChampion(int id);
        Item GetItem(int id);
        Rune GetRune(int id);
        StatPerk GetStatPerk(int id);
        Spell GetSpell(int id);
        void RefreshData();
        Item GetItem(string itemName);
    }
}
