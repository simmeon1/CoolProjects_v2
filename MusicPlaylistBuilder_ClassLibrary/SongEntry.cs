using System;

namespace MusicPlaylistBuilder_ClassLibrary
{
    public class SongEntry
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public DateTime Date { get; set; }
        public int Peak { get; set; } = int.MaxValue;
        public int Stay { get; set; } = int.MinValue;

        public SongEntry(string title, string artist, DateTime date)
        {
            Title = title;
            Artist = artist;
            Date = date;
        }

        public void SetPeak(int peak)
        {
            if (peak < Peak) Peak = peak;
        }

        public void SetStay(int stay)
        {
            if (stay > Stay) Stay = stay;
        }

        public override string ToString()
        {
            return $"{Artist} - {Title} - {Peak} - {Stay} - {Date}";
        }
    }
}