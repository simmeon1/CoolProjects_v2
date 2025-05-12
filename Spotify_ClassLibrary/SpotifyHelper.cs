using System.Text.RegularExpressions;

namespace Spotify_ClassLibrary;

public static class SpotifyHelper
{
    public static string CleanText(string text, bool isArtist)
    {
        //\bAND\b|& | with
        text = text.ToUpper();
        text = ReplaceTextPattern(text, @"\(.*?\)");
        text = ReplaceTextPattern(text, @"(\bFEAT\b|\bFT\b|\bFEATURING\b|DUET WITH|A DUET WITH|\\|\/).*");

        if (isArtist)
        {
            text = ReplaceTextPattern(text, @"(\bAND\b|\bWITH\b|& ).*");
        }
        
        text = ReplaceTextPattern(text, @"('|^THE )");
        text = ReplaceTextPattern(text, @"[^a-zA-Z0-9 ]", " ");
        text = ReplaceTextPattern(text, @"\s+", " ");
        text = text.Trim();
        return text;
    }
    
    private static string ReplaceTextPattern(string str, string pattern, string replacement = "")
    {
        return Regex.Replace(str, pattern, replacement);
    }

}