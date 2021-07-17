using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
        
        public static string ConcatenateListOfStringsToCommaString(this List<string> list)
        {
            if (list == null) return "";
            StringBuilder result = new("");
            foreach (string str in list)
            {
                if (result.Length > 0) result.Append(", ");
                result.Append(str.ToString());
            }
            return result.ToString();
        }
    }
}
