using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public class RealWebClient : IWebClient
    {
        private WebClient Client { get; set; }

        public RealWebClient()
        {
            Client = new();
        }

        public Task DownloadFileTaskAsync(string downloadLink, string fileName)
        {
            return Client.DownloadFileTaskAsync(downloadLink, fileName);
        }
    }
}