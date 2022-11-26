namespace Vigem_ClassLibrary
{
    public class Delayer : IDelayer
    {
        public Task Delay(int milliseconds)
        {
            return Task.Delay(milliseconds);
        }
    }
}