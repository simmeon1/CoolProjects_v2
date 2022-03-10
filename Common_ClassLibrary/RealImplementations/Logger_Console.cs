using System;
using System.Collections.Generic;

namespace Common_ClassLibrary
{
    public class Logger_Console : ILogger
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
            Console.WriteLine(message);
        }

        public void ReadKey()
        {
            Console.ReadKey();
        }
    }
}
