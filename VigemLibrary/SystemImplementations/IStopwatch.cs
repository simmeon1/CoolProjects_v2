namespace VigemLibrary.SystemImplementations
{
    public interface IStopwatch
    {
        void Restart();
        void Stop();
        void WaitUntilTimestampReached(double ts);
        void WaitUntilTrue(Func<bool> action);
        void Wait(double milliseconds);
        double GetElapsedTotalMilliseconds();
    }
}