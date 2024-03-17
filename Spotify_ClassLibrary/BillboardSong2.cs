namespace Spotify_ClassLibrary;

public class BillboardSong2
{
    public string song { get; set; }
    public string artist { get; set; }
    public int score { get; set; }
    public int year { get; set; }

    public override string ToString()
    {
        return artist + " - " + song + " - " + year + " - " + score;
    }
}