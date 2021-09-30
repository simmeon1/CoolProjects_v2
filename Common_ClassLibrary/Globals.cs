using System;
using System.Text.RegularExpressions;

namespace Common_ClassLibrary
{
    public static class Globals
    {
        public static string GetDateConcatenatedWithGuid(DateTime time, string guid)
        {
            return $"{time:yyyy-MM-dd--HH-mm-ss}_{guid}";
        }

        public static string GetPercentageAndCountString(int i, int maxCount)
        {
            int currentCount = i + 1;
            string percentageString = $"{currentCount / (double)maxCount * 100}%";
            Match match = Regex.Match(percentageString, @"(.*?\.\d\d).*%");
            if (match.Success) percentageString = $"{match.Groups[1].Value}%";
            return $"{currentCount}/{maxCount} ({percentageString})";
        }
    }
}
