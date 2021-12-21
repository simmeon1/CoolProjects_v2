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

        public List<DataTable> GetTables(List<SongCLS> songs)
        {
            List<DataTable> tables = new();
            DataTable mainTable = new("Summary");
            DataColumn doableColumn = new("Doable", TypeBool);
            DataColumn sameDayFinishColumn = new("Same Day Finish", TypeBool);
            mainTable.Columns.AddRange(new List<DataColumn> {
                new("Artist/Song", TypeString),
                new("Year", TypeInt32),
                new("YouTubeId", TypeString),
                new("YouTubeName", TypeString),
                new("YouTubeViews", TypeLong),
                new("SpotifyId", TypeString),
                new("SpotifyArtist", TypeString),
                new("SpotifySong", TypeString),
                new("SpotifyAlbum", TypeString),
                new("Acousticness", TypeDouble),
                new("Acousticness/Popularity", TypeDouble),
                new("Danceability", TypeDouble),
                new("Danceability/Popularity", TypeDouble),
                new("Energy", TypeDouble),
                new("Energy/Popularity", TypeDouble),
                new("Instrumentalness", TypeDouble),
                new("Instrumentalness/Popularity", TypeDouble),
                new("Liveness", TypeDouble),
                new("Liveness/Popularity", TypeDouble),
                new("Loudness", TypeDouble),
                new("Loudness/Popularity", TypeDouble),
                new("Speechiness", TypeDouble),
                new("Speechiness/Popularity", TypeDouble),
                new("Tempo", TypeDouble),
                new("Tempo/Popularity", TypeDouble),
                new("Valence", TypeDouble),
                new("Valence/Popularity", TypeDouble),
                new("ClubFactor", TypeDouble),
            }.ToArray());

            double averageViews = songs.Average(s => s.YouTubeViews);
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
                DataRow row = mainTable.NewRow();
                row[index++] = $"{song.Artist} - {song.Song}";
                row[index++] = song.Year;
                row[index++] = song.YouTubeId;
                row[index++] = song.YouTubeName;
                row[index++] = song.YouTubeViews;
                row[index++] = song.SpotifyId;
                row[index++] = song.SpotifyArtist;
                row[index++] = song.SpotifySong;
                row[index++] = song.SpotifyAlbum;
                row[index++] = song.Acousticness;
                row[index++] = GetPopularityPlusFeatureConfidence(song, averageViews, song.Acousticness, avgAcousticness);
                row[index++] = song.Danceability;
                row[index++] = GetPopularityPlusFeatureConfidence(song, averageViews, song.Danceability, avgDanceability);
                row[index++] = song.Energy;
                row[index++] = GetPopularityPlusFeatureConfidence(song, averageViews, song.Energy, avgEnergy);
                row[index++] = song.Instrumentalness;
                row[index++] = GetPopularityPlusFeatureConfidence(song, averageViews, song.Instrumentalness, avgInstrumentalness);
                row[index++] = song.Liveness;
                row[index++] = GetPopularityPlusFeatureConfidence(song, averageViews, song.Liveness, avgLiveness);
                row[index++] = song.Loudness;
                row[index++] = GetPopularityPlusFeatureConfidence(song, averageViews, song.Loudness, avgLoudness);
                row[index++] = song.Speechiness;
                row[index++] = GetPopularityPlusFeatureConfidence(song, averageViews, song.Speechiness, avgSpeechiness);
                row[index++] = song.Tempo;
                row[index++] = GetPopularityPlusFeatureConfidence(song, averageViews, song.Tempo, avgTempo);
                row[index++] = song.Valence;
                row[index++] = GetPopularityPlusFeatureConfidence(song, averageViews, song.Valence, avgValence);
                row[index++] = GetClubFactor(song, averageViews, avgDanceability, avgEnergy);

                mainTable.Rows.Add(row);
            }
            tables.Add(mainTable);
            return tables;
        }

        private static double GetPopularityPlusFeatureConfidence(SongCLS song, double avgViews, double feature, double avgFeature)
        {
            return Math.Round((100 - ((avgViews / song.YouTubeViews) * 100)) + (100 - ((avgFeature / feature) * 100)), 2);
        }
        
        private static double GetClubFactor(SongCLS song, double avgViews, double avgDance, double avgEnergy)
        {
            return Math.Round((100 - ((avgViews / song.YouTubeViews) * 100)) + (100 - ((avgDance / song.Danceability) * 100)) + (100 - ((avgEnergy / song.Energy) * 100)), 2);
        }
    }
}