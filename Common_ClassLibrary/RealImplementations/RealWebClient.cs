using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public class RealWebClient : IWebClient
    {
        private ILogger Logger { get; set; }
        private WebClient Client { get; set; }
        private int LastDownloadPercentage { get; set; }
        private bool LoggingProgress { get; set; }

        public RealWebClient(ILogger logger)
        {
            Logger = logger;
            Client = new WebClient();
            Client.DownloadProgressChanged += DownloadProgressChanged;
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (LoggingProgress) return;
            LoggingProgress = true;
            int progressPercentage = e.ProgressPercentage;
            
            long mbsReceived = GetMegaBytesFromBytes(e.BytesReceived);
            long mbsTotal = GetMegaBytesFromBytes(e.TotalBytesToReceive);

            if (progressPercentage == LastDownloadPercentage)
            {
                LoggingProgress = false;
                return;
            }
            Logger.Log($"File downloaded at {progressPercentage}% ({mbsReceived}/{mbsTotal})");
            LastDownloadPercentage = progressPercentage;
            LoggingProgress = false;
        }

        private static long GetMegaBytesFromBytes(long bytes)
        {
            return bytes / (1024 * 1024);
        }

        public Task DownloadFileTaskAsync(string downloadLink, string fileName)
        {
            return Client.DownloadFileTaskAsync(downloadLink, fileName);
        }
    }
}