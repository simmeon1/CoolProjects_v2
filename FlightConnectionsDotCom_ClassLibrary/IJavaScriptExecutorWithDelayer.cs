using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface IJavaScriptExecutorWithDelayer
    {
        Task<object> ExecuteScriptAndWait(string script, int millisecondsDelay, params object[] args);
    }
}