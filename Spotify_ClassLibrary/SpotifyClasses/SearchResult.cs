// ReSharper disable InconsistentNaming
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class SearchResult
{
    public Tracks tracks { get; set; }
}

public class Tracks
{
    public string href { get; set; }
    public TrackObject[] items { get; set; }
    public int limit { get; set; }
    public string next { get; set; }
    public int offset { get; set; }
    public string? previous { get; set; }
    public int total { get; set; }
}

public class External_ids
{
    public string isrc { get; set; }
}

public class External_urls
{
    public string spotify { get; set; }
}

