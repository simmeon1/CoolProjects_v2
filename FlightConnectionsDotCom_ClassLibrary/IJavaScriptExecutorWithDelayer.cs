using OpenQA.Selenium;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface IJavaScriptExecutorWithDelayer
    {
        int GetDefaultDelay();
        Task<object> ExecuteScriptAndWait(string script, params object[] args);
        Task<object> ExecuteScriptAndWait(int millisecondsDelay, string script, params object[] args);
        IDelayer GetDelayer();
        IJavaScriptExecutor GetJsExecutor();
    }
}