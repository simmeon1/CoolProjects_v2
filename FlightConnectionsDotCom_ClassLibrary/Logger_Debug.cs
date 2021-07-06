using System.Diagnostics;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class Logger_Debug : ILogger
    {
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }
    }
}