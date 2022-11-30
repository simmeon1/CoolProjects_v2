namespace Vigem_ClassLibrary.SystemImplementations
{
    public interface IStopwatch
    {
        double GetElapsedTotalMilliseconds();
        void Reset();
        void Start();
        void Stop();
    }
}