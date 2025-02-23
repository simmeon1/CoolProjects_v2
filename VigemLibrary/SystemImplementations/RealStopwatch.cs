using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace VigemLibrary.SystemImplementations
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
            WaitUntilTrue(() => GetElapsedTotalMilliseconds() >= ts);
        }
        
        public void Wait(double milliseconds)
        {
            WaitUntilTimestampReached(GetElapsedTotalMilliseconds() + milliseconds);
        }
        
        public void WaitUntilTrue(Func<bool> action)
        {
            while (!action.Invoke()) {}
        }
        
        public double GetElapsedTotalMilliseconds()
        {
            return stopwatch.Elapsed.TotalMilliseconds;
        }
    }
}