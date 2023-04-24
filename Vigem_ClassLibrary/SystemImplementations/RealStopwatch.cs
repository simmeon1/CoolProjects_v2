using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Vigem_ClassLibrary.SystemImplementations
{
    [ExcludeFromCodeCoverage]
    public class RealStopwatch : IStopwatch
    {
        private readonly Stopwatch stopwatch;

        public RealStopwatch()
        {
            stopwatch = new Stopwatch();
        }
        
        public void Restart()
        {
            stopwatch.Restart();
        }
        
        public void Stop()
        {
            stopwatch.Stop();
        }

        public void WaitUntilTimestampReached(double ts)
        {
            while (GetElapsedTotalMilliseconds() < ts) {
                //continue until it's time
            }
        }
        
        public void Wait(double milliseconds)
        {
            WaitUntilTimestampReached(GetElapsedTotalMilliseconds() + milliseconds);
        }
        
        private double GetElapsedTotalMilliseconds()
        {
            return stopwatch.Elapsed.TotalMilliseconds;
        }
    }
}