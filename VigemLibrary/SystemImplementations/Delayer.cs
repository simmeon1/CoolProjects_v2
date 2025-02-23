using System.Diagnostics.CodeAnalysis;

namespace VigemLibrary.SystemImplementations
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