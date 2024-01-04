using System.Collections.Generic;

namespace Common_ClassLibrary
{
    public interface ITableEntry
    {
        string GetCategory();
        string GetIdentifier();
        List<KeyValuePair<string, object>> GetProperties();
    }
}