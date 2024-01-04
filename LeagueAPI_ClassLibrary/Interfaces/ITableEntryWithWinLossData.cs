using Common_ClassLibrary;

namespace LeagueAPI_ClassLibrary
{
    public interface ITableEntryWithWinLossData: ITableEntry
    {
        public WinLossData GetWinLossData();
    }
}