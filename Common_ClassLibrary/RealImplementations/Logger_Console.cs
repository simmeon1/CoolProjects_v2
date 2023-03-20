using System;
using System.Collections.Generic;
using System.Text;

namespace Common_ClassLibrary
{
    public class Logger_Console : ILogger
    {
        private readonly Queue<string> logDetails = new();
        public bool Contains(string message)
        {
            return logDetails.Contains(message);
        }

        public string GetContent()
        {
            StringBuilder sb = new();
            string[] messages = logDetails.ToArray();
            foreach (string message in messages)
            {
                sb.AppendLine(message);
            }
            return sb.ToString();
        }

        public void Log(string message)
        {
            logDetails.Enqueue(message);
            Console.WriteLine(message);
        }

        public void ReadKey()
        {
            Console.ReadKey();
        }
    }
}
