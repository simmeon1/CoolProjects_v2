using System.Collections.Generic;

namespace Common_ClassLibrary
{
    public interface ITableEntry
    {
        string GetIdentifier();
        string GetCategory();
        List<KeyValuePair<string, object>> GetProperties();
    }
}