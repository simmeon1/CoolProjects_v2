namespace Spotify_ClassLibrary;

public class BillboardSong
{
    public string song { get; set; }
    public string artist { get; set; }
    public int this_week { get; set; }
    public int? last_week { get; set; }
    public int peak_position { get; set; }
    public int weeks_on_chart { get; set; }
}