using System.Diagnostics;

namespace Spotify_ClassLibrary;

[DebuggerDisplay("{GetArtistDashSong()}")]
public class ArtistSong(string artist, string song)
{
    public string GetArtistDashSong() => Concatenate(" - ");
    public string GetArtistSpaceSong() => Concatenate(" ");
    private string Concatenate(string joiner) => string.Join(joiner, new List<string> {artist, song});
}

public class ArtistSongEqualityComparer : IEqualityComparer<ArtistSong>
{
    public bool Equals(ArtistSong? x, ArtistSong? y)
    {
        return x?.GetArtistDashSong() == y?.GetArtistDashSong();
    }

    public int GetHashCode(ArtistSong obj)
    {
        return obj.GetArtistDashSong().GetHashCode();
    }
}