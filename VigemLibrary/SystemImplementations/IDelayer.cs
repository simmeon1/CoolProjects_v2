namespace VigemLibrary.SystemImplementations
{
    public interface IDelayer
    {
        Task Delay(int milliseconds);
    }
}