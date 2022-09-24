using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public interface ITableEntry
    {
        string GetCategory();
        List<KeyValuePair<string, object>> GetProperties();
    }
}