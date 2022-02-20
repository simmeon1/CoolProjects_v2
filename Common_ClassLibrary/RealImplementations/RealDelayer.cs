using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public class RealDelayer : IDelayer
    {
        public async Task Delay(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }

        public async Task Delay(TimeSpan timeSpan)
        {
            await Task.Delay(timeSpan);
        }

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }
}