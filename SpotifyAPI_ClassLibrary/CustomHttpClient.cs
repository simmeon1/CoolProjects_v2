using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class CustomHttpClient : IHttpClient
    {
        private HttpClient httpClient;

        public CustomHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            return await httpClient.SendAsync(request);
        }
    }
}
