using System.Collections.Generic;

namespace ClassLibrary
{
    public interface IJsonParser
    {
        T GetPropertyValue<T>(string json, params object[] propertyPath);
        List<string> GetArrayJsons(string json, params object[] arrayPath);
    }
}