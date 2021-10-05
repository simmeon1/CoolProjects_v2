using System;
using System.Text.RegularExpressions;

namespace Common_ClassLibrary
{
    public static class Globals
    {
        public static string GetDateTimeFileNameFriendlyConcatenatedWithString(DateTime time, string @string)
        {
            return $"{GetDateTimeFileNameFriendly(time)}_{@string}";
        }

        public static string GetDateTimeFileNameFriendly(DateTime time)
        {
            return time.ToString("yyyy-MM-dd--HH-mm-ss");
        }

        public static string GetPercentageAndCountString(int i, int maxCount)
        {
            string percentageString = $"{i / (double)maxCount * 100}";
            if (Regex.IsMatch(percentageString, @"^\d+$")) percentageString += ".00";
            else if (Regex.IsMatch(percentageString, @"^\d+\.\d$")) percentageString += "0";
            percentageString += "%";
            percentageString = $"{Regex.Match(percentageString, @"(.*?\.\d\d).*%").Groups[1].Value}%";
            return $"{i}/{maxCount} ({percentageString}) at {DateTime.Now}";
        }
    }
}
