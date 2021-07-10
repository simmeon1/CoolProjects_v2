using FlightConnectionsDotCom_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary.Interfaces;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class JavaScriptExecutorWithDelayer : IJavaScriptExecutorWithDelayer
    {
        private IJavaScriptExecutor JSExecutor { get; set; }
        private IDelayer Delayer { get; set; }
        private int DefaultDelay { get; set; }

        public JavaScriptExecutorWithDelayer(IJavaScriptExecutor jSExecutor, IDelayer delayer, int defaultDelay)
        {
            JSExecutor = jSExecutor;
            Delayer = delayer;
            DefaultDelay = defaultDelay;
        }

        public int GetDefaultDelay()
        {
            return DefaultDelay;
        }

        public IDelayer GetDelayer()
        {
            return Delayer;
        }

        public IJavaScriptExecutor GetJsExecutor()
        {
            return JSExecutor;
        }
        private async Task<object> ExecuteWithDelay(int millisecondsDelay, string script, object[] args)
        {
            object result = JSExecutor.ExecuteScript(script, args);
            await Delayer.Delay(millisecondsDelay);
            return result;
        }


        public async Task RunScript(string script, params object[] args)
        {
            await ExecuteWithDelay(GetDefaultDelay(), script, args);
        }

        public async Task RunScript(int millisecondsDelay, string script, params object[] args)
        {
            await ExecuteWithDelay(millisecondsDelay, script, args);
        }

        public async Task<ReadOnlyCollection<IWebElement>> RunScriptAndGetElements(string script, params object[] args)
        {
            return (ReadOnlyCollection<IWebElement>)await ExecuteWithDelay(GetDefaultDelay(), script, args);
        }

        public async Task<ReadOnlyCollection<IWebElement>> RunScriptAndGetElements(int millisecondsDelay, string script, params object[] args)
        {
            return (ReadOnlyCollection<IWebElement>)await ExecuteWithDelay(millisecondsDelay, script, args);
        }

        public async Task<IWebElement> RunScriptAndGetElement(string script, params object[] args)
        {
            return (IWebElement)await ExecuteWithDelay(GetDefaultDelay(), script, args);
        }

        public async Task<IWebElement> RunScriptAndGetElement(int millisecondsDelay, string script, params object[] args)
        {
            return (IWebElement)await ExecuteWithDelay(millisecondsDelay, script, args);
        }

        public async Task<T> RunScriptAndGetObject<T>(string script, params object[] args)
        {
            return (T)await ExecuteWithDelay(GetDefaultDelay(), script, args);
        }

        public async Task<T> RunScriptAndGetObject<T>(int millisecondsDelay, string script, params object[] args)
        {
            return (T)await ExecuteWithDelay(millisecondsDelay, script, args);
        }

        public async Task<string> RunScriptAndGetString(string script, params object[] args)
        {
            return (string)await ExecuteWithDelay(GetDefaultDelay(), script, args);
        }

        public async Task<string> RunScriptAndGetString(int millisecondsDelay, string script, params object[] args)
        {
            return (string)await ExecuteWithDelay(millisecondsDelay, script, args);
        }
    }
}