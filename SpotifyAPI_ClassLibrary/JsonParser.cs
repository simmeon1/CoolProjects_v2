using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ClassLibrary
{
    public class JsonParser : IJsonParser
    {
        public T GetPropertyValue<T>(string json, params object[] propertyPath)
        {
            JToken result = GetJson(json, propertyPath);
            return result.ToObject<T>();
        }

        private static JToken GetJson(string json, object[] propertyPath)
        {
            JToken result = JObject.Parse(json);
            foreach (object propSubPath in propertyPath) result = result[propSubPath];
            return result;
        }

        public List<string> GetArrayJsons(string json, params object[] arrayPath)
        {
            List<string> list = new();
            JToken jsons = GetJson(json, arrayPath);
            foreach (JToken jsonText in jsons) list.Add(jsonText.ToString());
            return list;
        }
    }
}