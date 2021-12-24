using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SpotifyAPI_ClassLibrary
{
    public class DataTableCreator
    {
        private Type TypeString { get; set; }
        private Type TypeInt32 { get; set; }
        private Type TypeLong { get; set; }
        private Type TypeDouble { get; set; }
        private Type TypeBool { get; set; }

        public DataTableCreator()
        {
            TypeString = Type.GetType("System.String");
            TypeInt32 = Type.GetType("System.Int32");
            TypeLong = Type.GetType("System.Int64");
            TypeDouble = Type.GetType("System.Double");
            TypeBool = Type.GetType("System.Boolean");
        }

        public List<DataTable> GetTables(List<SongCLS> songs, bool judgePopularityAgainstAllSongs = false)
        {
            List<DataTable> tables = new();
            DataTable mainTable = new("Summary");
            DataColumn doableColumn = new("Doable", TypeBool);
            DataColumn sameDayFinishColumn = new("Same Day Finish", TypeBool);
            mainTable.Columns.AddRange(new List<DataColumn> {
                new("Artist/Song", TypeString),
                new("Year", TypeInt32),
                new("SpotifyId", TypeString),
                new("SpotifyArtist", TypeString),
                new("SpotifySong", TypeString),
                new("SpotifyAlbum", TypeString),
                new("YouTubeId", TypeString),
                new("YouTubeName", TypeString),
                new("YouTubeViews", TypeLong),
                new("Popularity", TypeDouble),
                new("Danceability", TypeDouble),
                new("Energy", TypeDouble),
                //new("Acousticness", TypeDouble),
                //new("Acousticness/Popularity", TypeDouble),
                //new("Danceability/Popularity", TypeDouble),
                //new("Energy/Popularity", TypeDouble),
                //new("Instrumentalness", TypeDouble),
                //new("Instrumentalness/Popularity", TypeDouble),
                //new("Liveness", TypeDouble),
                //new("Liveness/Popularity", TypeDouble),
                //new("Loudness", TypeDouble),
                //new("Loudness/Popularity", TypeDouble),
                //new("Speechiness", TypeDouble),
                //new("Speechiness/Popularity", TypeDouble),
                //new("Tempo", TypeDouble),
                //new("Tempo/Popularity", TypeDouble),
                //new("Valence", TypeDouble),
                //new("Valence/Popularity", TypeDouble),
                new("Danceability+Energy", TypeDouble),
                new("Danceability+Energy+Popularity", TypeDouble),
            }.ToArray());

            Dictionary<int, List<long>> yearsAndTotalViews = GetYearsAndTotalViews(songs);
            Dictionary<int, long> yearsAndMaxViews = GetYearsAndNumberOfViews(songs, yearsAndTotalViews);

            double avgAcousticness = songs.Average(s => s.Acousticness);
            double avgDanceability = songs.Average(s => s.Danceability);
            double avgEnergy = songs.Average(s => s.Energy);
            double avgInstrumentalness = songs.Average(s => s.Instrumentalness);
            double avgLiveness = songs.Average(s => s.Liveness);
            double avgLoudness = songs.Average(s => s.Loudness);
            double avgSpeechiness = songs.Average(s => s.Speechiness);
            double avgTempo = songs.Average(s => s.Tempo);
            double avgValence = songs.Average(s => s.Valence);

            for (int i = 0; i < songs.Count; i++)
            {
                int index = 0;
                SongCLS song = songs[i];
                double songPopularity = GetPopularityFromZeroToOneForSongYear(song, yearsAndMaxViews);
                DataRow row = mainTable.NewRow();
                row[index++] = $"{song.Artist} - {song.Song}";
                row[index++] = song.Year;
                row[index++] = song.SpotifyId;
                row[index++] = song.SpotifyArtist;
                row[index++] = song.SpotifySong;
                row[index++] = song.SpotifyAlbum;
                row[index++] = song.YouTubeId;
                row[index++] = song.YouTubeName;
                row[index++] = song.YouTubeViews;
                row[index++] = songPopularity;
                row[index++] = song.Danceability;
                row[index++] = song.Energy;
                //row[index++] = song.Acousticness;
                //row[index++] = songPopularity + song.Acousticness;
                //row[index++] = songPopularity + song.Danceability;
                //row[index++] = songPopularity + song.Energy;
                //row[index++] = song.Instrumentalness;
                //row[index++] = songPopularity + song.Instrumentalness;
                //row[index++] = song.Liveness;
                //row[index++] = songPopularity + song.Liveness;
                //row[index++] = song.Loudness;
                //row[index++] = songPopularity + song.Loudness;
                //row[index++] = song.Speechiness;
                //row[index++] = songPopularity + song.Speechiness;
                //row[index++] = song.Tempo;
                //row[index++] = songPopularity + song.Tempo;
                //row[index++] = song.Valence;
                //row[index++] = songPopularity + song.Valence;
                row[index++] = song.Danceability + song.Energy;
                row[index++] = song.Danceability + song.Energy + songPopularity;

                mainTable.Rows.Add(row);
            }
            tables.Add(mainTable);
            return tables;
        }

        private static Dictionary<int, long> GetYearsAndNumberOfViews(List<SongCLS> songs, Dictionary<int, List<long>> yearsAndTotalViews)
        {
            Dictionary<int, long> yearsAndNumberOfViews = new();
            long maxViews = songs.Max(s => s.YouTubeViews);
            yearsAndNumberOfViews.Add(0, maxViews);

            foreach (KeyValuePair<int, List<long>> yearAndTotalViews in yearsAndTotalViews)
            {
                yearsAndNumberOfViews.Add(yearAndTotalViews.Key, yearAndTotalViews.Value.Max(v => v));
            }
            return yearsAndNumberOfViews;
        }

        private static Dictionary<int, List<long>> GetYearsAndTotalViews(List<SongCLS> songs)
        {
            Dictionary<int, List<long>> yearsAndTotalViews = new();
            foreach (SongCLS song in songs)
            {
                int songYear = song.Year;
                if (yearsAndTotalViews.ContainsKey(songYear)) yearsAndTotalViews[songYear].Add(song.YouTubeViews);
                else yearsAndTotalViews.Add(songYear, new List<long>() { song.YouTubeViews });
            }

            return yearsAndTotalViews;
        }

        private static double GetPopularityFromZeroToOneForSongDecade(SongCLS song, Dictionary<int, long> yearsAndMaxViews)
        {
            string songDecade = song.Year.ToString().Substring(0, 3);
            long decadeMaxViews = 0;
            foreach (KeyValuePair<int, long> yearAndMaxViews in yearsAndMaxViews)
            {
                if (yearAndMaxViews.Key.ToString().StartsWith(songDecade) && yearAndMaxViews.Value > decadeMaxViews)
                {
                    decadeMaxViews = yearAndMaxViews.Value;
                }
            }
            return Math.Round(((double)song.YouTubeViews / decadeMaxViews), 3);
        }
        
        private static double GetPopularityFromZeroToOneForSongYear(SongCLS song, Dictionary<int, long> yearsAndMaxViews)
        {
            long yearMaxViews = 0;
            foreach (KeyValuePair<int, long> yearAndMaxViews in yearsAndMaxViews)
            {
                if (yearAndMaxViews.Key == song.Year && yearAndMaxViews.Value > yearMaxViews)
                {
                    yearMaxViews = yearAndMaxViews.Value;
                }
            }
            return Math.Round(((double)song.YouTubeViews / yearMaxViews), 3);
        }
    }
}