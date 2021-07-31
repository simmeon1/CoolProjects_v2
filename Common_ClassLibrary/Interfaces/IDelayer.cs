using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public interface IDelayer
    {
        Task Delay(int milliseconds);
    }
}