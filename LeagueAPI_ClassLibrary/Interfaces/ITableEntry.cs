using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public interface ITableEntry
    {
        string GetCategory();
        string GetIdentifier();
        List<KeyValuePair<string, object>> GetProperties();
    }
}