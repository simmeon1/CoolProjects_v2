using System.Diagnostics.CodeAnalysis;

namespace Vigem_ClassLibrary
{
    [ExcludeFromCodeCoverage]
    public class Delayer : IDelayer
    {
        public Task Delay(int milliseconds)
        {
            return Task.Delay(milliseconds);
        }
    }
}