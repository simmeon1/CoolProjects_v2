using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpotifyAPI_ClassLibrary
{
    public class SongCLS : IEqualityComparer<SongCLS>
    {
        public string Artist { get; set; } = "";
        public string Song { get; set; } = "";
        public int Year { get; set; }
        public string YouTubeId { get; set; } = "";
        public string YouTubeName { get; set; } = "";
        public long YouTubeViews { get; set; }
        public string SpotifyId { get; set; } = "";
        public string SpotifyArtist { get; set; } = "";
        public string SpotifySong { get; set; } = "";
        public string SpotifyAlbum { get; set; } = "";

        public bool Equals(SongCLS x, SongCLS y)
        {
            return x.SpotifyId.Equals(y.SpotifyId);
        }

        public int GetHashCode([DisallowNull] SongCLS obj)
        {
            return obj.SpotifyId.GetHashCode();
        }
    }
}
