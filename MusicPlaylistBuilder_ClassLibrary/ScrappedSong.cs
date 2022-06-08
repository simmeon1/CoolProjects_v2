using System;

namespace MusicPlaylistBuilder_ClassLibrary
{
    public class ScrappedSong
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public DateTime Date { get; set; }
        public int Peak { get; set; } = int.MaxValue;
        public int Stay { get; set; } = int.MinValue;

        public ScrappedSong(string title, string artist, DateTime date)
        {
            Title = title;
            Artist = artist;
            Date = date;
        }

        public void SetHigherPeak(int peak)
        {
            if (peak < Peak) Peak = peak;
        }

        public void SetLongerStay(int stay)
        {
            if (stay > Stay) Stay = stay;
        }
        
        public void SetEarlierDate(DateTime date)
        {
            if (date.CompareTo(Date) < 0) Date = date;
        }

        public override string ToString()
        {
            return $"{Artist} - {Title} - {Peak} - {Stay} - {Date}";
        }
    }
}