using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface IHttpClient
    {
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}