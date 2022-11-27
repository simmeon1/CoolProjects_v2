namespace Vigem_ClassLibrary.SystemImplementations
{
    public interface IDelayer
    {
        Task Delay(int milliseconds);
    }
}