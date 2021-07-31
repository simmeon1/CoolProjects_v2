namespace SpotifyAPI_ClassLibrary
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
