using System.Net.Http;
using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public class RealHttpClient : IHttpClient
    {
        private readonly HttpClient client;

        public RealHttpClient()
        {
            client = new HttpClient();
        }

        public Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            return client.SendAsync(request);
        }
    }
}