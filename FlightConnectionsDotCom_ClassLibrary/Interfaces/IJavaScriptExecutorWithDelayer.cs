using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary.Interfaces
{
    public interface IJavaScriptExecutorWithDelayer
    {
        int GetDefaultDelay();
        IDelayer GetDelayer();
        IJavaScriptExecutor GetJsExecutor();
        Task RunScript(string script, params object[] args);
        Task RunScript(int millisecondsDelay, string script, params object[] args);
        Task<ReadOnlyCollection<IWebElement>> RunScriptAndGetElements(string script, params object[] args);
        Task<ReadOnlyCollection<IWebElement>> RunScriptAndGetElements(int millisecondsDelay, string script, params object[] args);
        Task<IWebElement> RunScriptAndGetElement(string script, params object[] args);
        Task<IWebElement> RunScriptAndGetElement(int millisecondsDelay, string script, params object[] args);
        Task<T> RunScriptAndGetObject<T>(string script, params object[] args);
        Task<T> RunScriptAndGetObject<T>(int millisecondsDelay, string script, params object[] args);
        Task<string> RunScriptAndGetString(string script, params object[] args);
        Task<string> RunScriptAndGetString(int millisecondsDelay, string script, params object[] args);
    }
}