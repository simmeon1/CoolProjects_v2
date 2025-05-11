using System.Diagnostics;

namespace Spotify_ClassLibrary;

[DebuggerDisplay("{GetArtistDashSong()}")]
public class ArtistSong
{
    public string artist { get; init; }
    public string song { get; init; }

    public ArtistSong(string artist, string song)
    {
        this.artist = artist;
        this.song = song;
    }

    public string GetArtistDashSong() => Concatenate(" - ");
    public string GetArtistSpaceSong() => Concatenate(" ");
    private string Concatenate(string joiner) => string.Join(joiner, new List<string> {artist, song});
}