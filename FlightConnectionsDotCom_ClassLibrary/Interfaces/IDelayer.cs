using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary.Interfaces
{
    public interface IDelayer
    {
        Task Delay(int milliseconds);
    }
}