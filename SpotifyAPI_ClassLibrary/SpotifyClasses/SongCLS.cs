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
        public double Acousticness { get; set; }
        public double Danceability { get; set; }
        public double Energy { get; set; }
        public double Instrumentalness { get; set; }
        public double Liveness { get; set; }
        public double Loudness { get; set; }
        public double Speechiness { get; set; }
        public double Tempo { get; set; }
        public double Valence { get; set; }

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
