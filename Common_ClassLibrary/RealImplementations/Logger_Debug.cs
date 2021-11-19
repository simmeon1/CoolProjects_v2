using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Common_ClassLibrary
{
    public class Logger_Debug : ILogger
    {
        private Queue<string> LogDetails { get; set; } = new();
        public bool Contains(string message)
        {
            return LogDetails.Contains(message);
        }

        public string GetContent()
        {
            return string.Join(Environment.NewLine, LogDetails.ToArray());
        }

        public void Log(string message)
        {
            LogDetails.Enqueue(message);
            Debug.WriteLine(message);
        }
    }
}
