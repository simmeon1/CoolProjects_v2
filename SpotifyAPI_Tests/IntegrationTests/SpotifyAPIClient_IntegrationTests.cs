using Common_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpotifyAPI_ClassLibrary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyAPI_Tests.IntegrationTests
{
    [TestClass]
    public class SpotifyAPIClient_IntegrationTests
    {
        public TestContext TestContext { get; set; }
        private ISpotifyCredentials Credentials { get; set; }
        private SpotifyAPIClient Client { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Credentials = File.ReadAllText((string)TestContext.Properties["credentialsPath"]).DeserializeObject<SpotifyCredentials>();
            Client = new(new RealHttpClient(), Credentials, new SpotifyTokenWorker(), new RealDateTimeProvider());
        }

        [TestMethod]
        public async Task GetPlaylists_AtLeastOneResult()
        {
            List<Playlist> result = await Client.GetPlaylists();
            Assert.IsTrue(result.Count > 0);
        }

        [Ignore]
        [TestMethod]
        public async Task TestAsync()
        {
            List<SongCLS> songs = File.ReadAllText(@"C:\Users\simme\source\repos\json files from laptop\TopTenUKandUSSingles_lessFiller.json").DeserializeObject<List<SongCLS>>();
            songs = songs.Distinct(new SongCLS()).OrderByDescending(x => x.YouTubeViews).ToList();
            songs.RemoveAll(x => string.IsNullOrEmpty(x.SpotifyId));
            await Client.AddSongsToPlaylist(songs, "3op8x6eK3VuQ5klTuo6C3i");
            //songs = songs.OrderByDescending(x => x.YouTubeViews).ToList();
            //List<SongCLS> songs50to59 = songs.Where(x => x.Year >= 1950 && x.Year <= 1959).Take(1).ToList();
            //List<SongCLS> songs60to69 = songs.Where(x => x.Year >= 1960 && x.Year <= 1969).Take(24).ToList();
            //List<SongCLS> songs70to79 = songs.Where(x => x.Year >= 1970 && x.Year <= 1979).Take(222).ToList();
            //List<SongCLS> songs80to89 = songs.Where(x => x.Year >= 1980 && x.Year <= 1989).Take(672).ToList();
            //List<SongCLS> songs90to99 = songs.Where(x => x.Year >= 1990 && x.Year <= 1999).Take(880).ToList();
            //List<SongCLS> songs00to09 = songs.Where(x => x.Year >= 2000 && x.Year <= 2009).Take(864).ToList();
            //List<SongCLS> songs10to19 = songs.Where(x => x.Year >= 2010 && x.Year <= 2019).Take(778).ToList();
            //List<SongCLS> songs20to21 = songs.Where(x => x.Year >= 2020 && x.Year <= 2021).Take(46).ToList();

            //List<SongCLS> songsLimited = new();
            //songsLimited.AddRange(songs50to59);
            //songsLimited.AddRange(songs60to69);
            //songsLimited.AddRange(songs70to79);
            //songsLimited.AddRange(songs80to89);
            //songsLimited.AddRange(songs90to99);
            //songsLimited.AddRange(songs00to09);
            //songsLimited.AddRange(songs10to19);
            //songsLimited.AddRange(songs20to21);
            //songsLimited = songsLimited.OrderByDescending(x => x.YouTubeViews).ToList();
        }

        [Ignore]
        [TestMethod]
        public async Task TestAsync2()
        {
            List<SongCLS> songs = File.ReadAllText(@"C:\Users\simme\source\repos\json files from laptop\TopTenUKandUSSingles.json").DeserializeObject<List<SongCLS>>();
            songs = songs.Distinct(new SongCLS()).OrderByDescending(x => x.YouTubeViews).ToList();
            songs.RemoveAll(x => string.IsNullOrEmpty(x.SpotifyId));
            List<string> spotifyIds = songs.Select(s => s.SpotifyId).ToList();
            await Client.AddAudioFeaturesToSongs(songs);
            File.WriteAllText(@"C:\Users\simme\source\repos\json files from laptop\TopTenUKandUSSingles.json", songs.SerializeObject(Newtonsoft.Json.Formatting.Indented));
        }
    }
}
