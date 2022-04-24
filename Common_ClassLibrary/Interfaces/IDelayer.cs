using System;
using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public interface IDelayer
    {
        Task Delay(int milliseconds);
        Task Delay(TimeSpan timeSpan);
        void Sleep(int milliseconds);
        Task GetCompletedTask();
    }
}