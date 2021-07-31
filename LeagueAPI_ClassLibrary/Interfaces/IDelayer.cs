using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface IDelayer
    {
        Task Delay(int milliseconds);
    }
}