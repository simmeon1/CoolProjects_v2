using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.SpotifyClasses
{
    public class Playlist
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Playlist(string id, string name, string description)
        {
            ID = id;
            Name = name;
            Description = description;
        }
    }
}
