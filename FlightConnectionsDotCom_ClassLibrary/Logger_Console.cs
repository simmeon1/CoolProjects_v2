using FlightConnectionsDotCom_ClassLibrary.Interfaces;
using System;
using System.Diagnostics;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class Logger_Console : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}