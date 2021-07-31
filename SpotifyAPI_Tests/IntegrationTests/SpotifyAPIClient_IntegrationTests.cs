using Common_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpotifyAPI_ClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ClassLibrary
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
        public async Task GetPlaylists_AtLeastOneResultAsync()
        {
            List<Playlist> result = await Client.GetPlaylists();
            Assert.IsTrue(result.Count > 0);
        }
    }
}
