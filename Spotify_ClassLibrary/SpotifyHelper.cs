using System.Text.RegularExpressions;

namespace Spotify_ClassLibrary;

public static class SpotifyHelper
{
    public static List<Dictionary<string, List<BillboardSong>>> GetDateSongMaps(params List<List<BillboardList>> listOfBillBoardLists)
    {
        Dictionary<string, List<BillboardSong>> GetDictFromBillboardList(List<BillboardList> billboardLists) =>
            billboardLists.ToDictionary(x => x.date, y => y.data);
        List<Dictionary<string, List<BillboardSong>>> dateSongMaps = [..listOfBillBoardLists.Select(GetDictFromBillboardList)];
        MakeSongsMoreGeneric(dateSongMaps);
        return dateSongMaps;
    }

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

    private static void MakeSongsMoreGeneric(List<Dictionary<string, List<BillboardSong>>> dateSongMaps)
    {
        foreach (var songMap in dateSongMaps)
        {
            foreach (var songs in songMap.Values)
            {
                foreach (var song in songs)
                {
                    song.song = CleanText(song.song, false);
                    song.artist = CleanText(song.artist, true);
                }
            }
        }
    }

    private static string ReplaceTextPattern(string str, string pattern, string replacement = "")
    {
        return Regex.Replace(str, pattern, replacement);
    }
}