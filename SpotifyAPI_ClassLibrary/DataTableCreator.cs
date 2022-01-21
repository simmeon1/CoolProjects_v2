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

            Dictionary<int, List<SongCLS>> yearsAndOrderedSongs = GetYearsAndSongsOrderedByViews(songs);
            //Dictionary<int, long> yearsAndMaxViews = GetYearsAndMaxViews(songs, yearsAndSongs);

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
                double songPopularity = GetPopularityFromZeroToOneForSongDecade(song, yearsAndOrderedSongs);
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

        private static Dictionary<int, long> GetYearsAndMaxViews(List<SongCLS> songs, Dictionary<int, List<SongCLS>> yearsAndSongs)
        {
            Dictionary<int, long> yearsAndNumberOfViews = new();
            long maxViews = songs.Max(s => s.YouTubeViews);
            yearsAndNumberOfViews.Add(0, maxViews);

            foreach (KeyValuePair<int, List<SongCLS>> yearAndSongs in yearsAndSongs)
            {
                yearsAndNumberOfViews.Add(yearAndSongs.Key, yearAndSongs.Value.Max(s => s.YouTubeViews));
            }
            return yearsAndNumberOfViews;
        }

        private static Dictionary<int, List<SongCLS>> GetYearsAndSongsOrderedByViews(List<SongCLS> songs)
        {
            Dictionary<int, List<SongCLS>> yearsAndSongs = new();
            foreach (SongCLS song in songs)
            {
                int songYear = song.Year;
                if (yearsAndSongs.ContainsKey(songYear)) yearsAndSongs[songYear].Add(song);
                else yearsAndSongs.Add(songYear, new List<SongCLS>() { song });
            }

            Dictionary<int, List<SongCLS>>.KeyCollection keys = yearsAndSongs.Keys;
            foreach (int key in keys)
            {
                List<SongCLS> songss = yearsAndSongs[key];
                yearsAndSongs[key] = songss.OrderByDescending(s => s.YouTubeViews).ToList();
            }

            return yearsAndSongs;
        }

        private static double GetPopularityFromZeroToOneForSongDecade(SongCLS song, Dictionary<int, List<SongCLS>> yearsAndOrderedSongs)
        {
            Dictionary<string, List<SongCLS>> decadesAndOrderedSongs = new();

            foreach (KeyValuePair<int, List<SongCLS>> yearAndOrderedSongs in yearsAndOrderedSongs)
            {
                int year = yearAndOrderedSongs.Key;
                if (year == 0) continue;

                string decade = year.ToString().Substring(0, 3);
                if (decadesAndOrderedSongs.ContainsKey(decade)) decadesAndOrderedSongs[decade].AddRange(yearAndOrderedSongs.Value);
                else decadesAndOrderedSongs.Add(decade, new List<SongCLS>(yearAndOrderedSongs.Value));
            }

            Dictionary<string, List<SongCLS>>.KeyCollection keys = decadesAndOrderedSongs.Keys;
            foreach (string key in keys)
            {
                List<SongCLS> songs = decadesAndOrderedSongs[key];
                decadesAndOrderedSongs[key] = songs.OrderByDescending(s => s.YouTubeViews).ToList();
            }

            string songDecade = song.Year.ToString().Substring(0, 3);
            List<SongCLS> decadeSongs = decadesAndOrderedSongs[songDecade];
            int decadeSongCount = decadeSongs.Count;
            int songIndex = decadeSongs.IndexOf(song);
            return Math.Round((double)(decadeSongCount - songIndex) / decadeSongCount, 3);
        }

        private static double GetPopularityFromZeroToOneForSongYear(SongCLS song, Dictionary<int, List<SongCLS>> yearsAndOrderedSongs)
        {
            List<SongCLS> yearSongs = yearsAndOrderedSongs[song.Year];
            int yearSongCount = yearSongs.Count;
            int songIndex = yearSongs.IndexOf(song);
            return Math.Round((double)(yearSongCount - songIndex) / yearSongCount, 3);
        }
    }
}