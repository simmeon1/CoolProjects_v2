using System.Net.Http;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> SendRequest(HttpRequestMessage request);
    }
}