using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class Delayer : IDelayer
    {
        public async Task Delay(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }
    }
}