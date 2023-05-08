namespace Vigem_ClassLibrary.SystemImplementations
{
    public interface IStopwatch
    {
        void Restart();
        void Stop();
        void WaitUntilTimestampReached(double ts);
        void WaitUntilTrue(Func<bool> action);
        void Wait(double milliseconds);
    }
}