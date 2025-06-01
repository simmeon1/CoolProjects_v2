// Custom, less info than track object

using System.Diagnostics;

[DebuggerDisplay("{artist_name} - {name} - {release_date} - {popularity}")]
public class SimpleTrackObject
{
    public string id { get; set; }
    public string name { get; set; }
    public string artist_id { get; set; }
    public string artist_name { get; set; }
    public string release_date { get; set; }
    public int popularity { get; set; }
}