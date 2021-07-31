namespace Common_ClassLibrary
{
    public interface ILogger
    {
        void Log(string message);
        bool Contains(string message);
    }
}
