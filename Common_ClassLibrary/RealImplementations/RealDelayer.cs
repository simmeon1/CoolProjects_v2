using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public class RealDelayer : IDelayer
    {
        public async Task Delay(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }
    }
}