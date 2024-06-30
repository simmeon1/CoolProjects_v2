// ReSharper disable InconsistentNaming
#pragma warning disable CS8618
public class FullArtistObject
{
    public External_urls external_urls { get; set; }
    public Followers followers { get; set; }
    public string[] genres { get; set; }
    public string href { get; set; }
    public string id { get; set; }
    public ImageObject[] images { get; set; }
    public string name { get; set; }
    public int popularity { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
}

public class Followers
{
    public string? href { get; set; }
    public int total { get; set; }
}
