using System;
using System.Collections.Generic;

namespace Common_ClassLibrary
{
    public class Logger_Console : ILogger
    {
        private List<string> LogDetails { get; set; } = new();
        public bool Contains(string message)
        {
            return LogDetails.Contains(message);
        }

        public void Log(string message)
        {
            LogDetails.Add(message);
            Console.WriteLine(message);
        }
    }
}
