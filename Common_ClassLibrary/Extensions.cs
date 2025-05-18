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

        public static string SerializeObject(this object obj, Formatting formatting = Formatting.None, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(obj, formatting, settings ?? new JsonSerializerSettings());
        }

        public static T DeserializeObject<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str)!;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string ConcatenateListOfStringsToCommaAndSpaceString(this IEnumerable<string> list)
        {
            return ConcatenateListOfStrings(list, ", ");
        }
        
        public static string ConcatenateListOfStringsToCommaString(this IEnumerable<string> list)
        {
            return ConcatenateListOfStrings(list, ",");
        }
        
        public static string ConcatenateListOfStringsToDashString(this IEnumerable<string> list)
        {
            return ConcatenateListOfStrings(list, "-");
        }

        private static string ConcatenateListOfStrings(IEnumerable<string> list, string replacement)
        {
            return string.Join(replacement, list);
        }
    }
}
