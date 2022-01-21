using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public interface IWebClient
    {
        Task DownloadFileTaskAsync(string downloadLink, string fileName);
    }
}