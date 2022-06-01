namespace MusicPlaylistBuilder_ClassLibrary
{
    public class SongCLS
    {
        public string Artist { get; set; } = "";
        public string Song { get; set; } = "";
        public int Year { get; set; }
        public int Peak { get; set; }
        public int WeeksInTopTen { get; set; }
        public string GetSearchTerms()
        {
            return $"{Artist} {Song}";
        }

        public override string ToString()
        {
            return $"{Artist} - {Song} - {Year} - {Peak} - {WeeksInTopTen}";
        }
    }
}
