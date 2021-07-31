using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class RealHttpClient : IHttpClient
    {
        private HttpClient Client { get; set; }

        public RealHttpClient()
        {
            Client = new();
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return Client.SendAsync(request);
        }
    }
}