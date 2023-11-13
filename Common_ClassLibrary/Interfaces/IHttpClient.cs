using System.Net.Http;
using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public interface IHttpClient
    {
        public Task<HttpResponseMessage> SendRequest(HttpRequestMessage request);
        Task<HttpResponseMessage> GetAsync(string uri);
    }
}