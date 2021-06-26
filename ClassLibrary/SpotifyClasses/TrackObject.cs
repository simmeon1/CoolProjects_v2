using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.SpotifyClasses
{
    public class TrackObject
    {
        public SimplifiedAlbumObject album { get; set; }
        public List<ArtistObject> artists { get; set; }
        public List<string> available_markets { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        public bool explicitt { get; set; }
        public ExternalIdObject external_ids { get; set; }
        public ExternalUrlObject external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public bool is_local { get; set; }
        public bool is_playable { get; set; }
        public object linked_from { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string preview_url { get; set; }
        public TrackRestrictionObject restrictions { get; set; }
        public int track_number { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }
}
