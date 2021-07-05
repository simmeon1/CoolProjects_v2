using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface IDelayer
    {
        Task Delay(int milliseconds);
    }
}