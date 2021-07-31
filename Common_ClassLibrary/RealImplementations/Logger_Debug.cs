using System.Collections.Generic;
using System.Diagnostics;

namespace Common_ClassLibrary
{
    public class Logger_Debug : ILogger
    {
        private List<string> LogDetails { get; set; } = new();
        public bool Contains(string message)
        {
            return LogDetails.Contains(message);
        }

        public void Log(string message)
        {
            LogDetails.Add(message);
            Debug.WriteLine(message);
        }
    }
}
