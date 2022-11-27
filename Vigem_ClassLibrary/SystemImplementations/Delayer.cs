using System.Diagnostics.CodeAnalysis;

namespace Vigem_ClassLibrary.SystemImplementations
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