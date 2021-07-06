using FlightConnectionsDotCom_ClassLibrary;
using OpenQA.Selenium;
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

        public async Task<object> ExecuteScriptAndWait(int millisecondsDelay, string script, params object[] args)
        {
            return await ExecuteWithDelay(millisecondsDelay, script, args);
        }

        public async Task<object> ExecuteScriptAndWait(string script, params object[] args)
        {
            return await ExecuteWithDelay(GetDefaultDelay(), script, args);
        }

        private async Task<object> ExecuteWithDelay(int millisecondsDelay, string script, object[] args)
        {
            object result = JSExecutor.ExecuteScript(script, args);
            await Delayer.Delay(millisecondsDelay);
            return result;
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
    }
}