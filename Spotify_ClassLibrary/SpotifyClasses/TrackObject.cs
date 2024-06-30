#pragma warning disable CS8618
public class TrackObject
{
    public SimplfiedAlbumObject album { get; set; }
    public ArtistObject[] artists { get; set; }
    public string[] available_markets { get; set; }
    public int disc_number { get; set; }
    public int duration_ms { get; set; }
    // public bool explicit { get; set; }
    public External_ids external_ids { get; set; }
    public External_urls external_urls { get; set; }
    public string href { get; set; }
    public string id { get; set; }
    public bool is_local { get; set; }
    public string name { get; set; }
    public int popularity { get; set; }
    public string? preview_url { get; set; }
    public int track_number { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
}