using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface IDDragonRepository
    {
        Task RefreshData(string version);
        Champion GetChampion(int id);
        Rune GetRune(int id);
        StatPerk GetStatPerk(int id);
        Spell GetSpell(int id);
        Item GetItemById(int id);
        ICollection<Item> GetItemsByName(string itemName);
        ArenaAugment GetArenaAugment(int id);
    }
}
