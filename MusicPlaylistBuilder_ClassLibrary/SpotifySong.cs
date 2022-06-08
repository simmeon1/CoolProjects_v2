using System.Collections.Generic;
using Common_ClassLibrary;

namespace MusicPlaylistBuilder_ClassLibrary
{
    public class SpotifySong
    {
        public List<string> Artists { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }

        public SpotifySong(List<string> artists, string name, string id)
        {
            Artists = artists;
            Name = name;
            Id = id;
        }

        private string GetArtistsString()
        {
            return Artists.ConcatenateListOfStringsToCommaAndSpaceString();
        }

        public override string ToString()
        {
            return $"{GetArtistsString()} - {Name} - {Id}";
        }
    }
}
