using Newtonsoft.Json.Linq;
using System;

namespace ClassLibrary
{
    public class JsonParser : IJsonParser
    {
        private JObject JObject { get; set; }
        public JsonParser(string json = null)
        {
            SetJObjectBasedOnJson(json);
        }

        private void SetJObjectBasedOnJson(string json)
        {
            JObject = json == null ? null : JObject.Parse(json);
        }

        public void SetJsonToParse(string json)
        {
            SetJObjectBasedOnJson(json);
        }

        public T GetPropertyValue<T>(params object[] propertyPath)
        {
            JToken result = JObject;
            foreach (object propSubPath in propertyPath) result = result[propSubPath];
            return result.ToObject<T>();
        }

    }
}