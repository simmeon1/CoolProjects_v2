using System.Diagnostics;

namespace Spotify_ClassLibrary;

[DebuggerDisplay("{GetSummary()}")]
public class BillboardSong2
{
    public ArtistSong artistSong { get; set; }
    public int score { get; set; }
    public int year { get; set; }

    public string GetSummary()
    {
        return GetArtistDashSong() + " - " + year + " - " + score;
    } 
    
    public string GetArtistDashSong()
    {
        return artistSong.GetArtistDashSong();
    }
}