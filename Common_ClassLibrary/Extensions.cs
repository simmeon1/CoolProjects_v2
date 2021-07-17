using Newtonsoft.Json;
using System;

namespace Common_ClassLibrary
{
    public static class Extensions
    {
        public static T CloneObject<T>(this T obj)
        {
            return obj.SerializeObject().DeserializeObject<T>();
        }
        
        public static string SerializeObject(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        
        public static T DeserializeObject<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
