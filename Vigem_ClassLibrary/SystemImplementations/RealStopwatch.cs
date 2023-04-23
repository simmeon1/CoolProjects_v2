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
            stopwatch = new();
        }

        public double GetElapsedTotalMilliseconds()
        {
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        public void Reset()
        {
            stopwatch.Reset();
        }

        public void Restart()
        {
            stopwatch.Restart();
        }

        public void Start()
        {
            stopwatch.Start();
        }

        public void Stop()
        {
            stopwatch.Stop();
        }
    }
}