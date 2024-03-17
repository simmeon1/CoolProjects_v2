namespace Spotify_ClassLibrary;

public class ArtistSong
{
    public string artist { get; set; }
    public string song { get; set; }
    
    public override string ToString()
    {
        return artist + " - " + song;
    }
}