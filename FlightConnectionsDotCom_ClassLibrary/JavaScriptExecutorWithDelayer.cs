using FlightConnectionsDotCom_ClassLibrary;
using OpenQA.Selenium;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class JavaScriptExecutorWithDelayer : IJavaScriptExecutorWithDelayer
    {
        private IJavaScriptExecutor JSExecutor { get; set; }
        private IDelayer Delayer { get; set; }

        public JavaScriptExecutorWithDelayer(IJavaScriptExecutor jSExecutor, IDelayer delayer)
        {
            JSExecutor = jSExecutor;
            Delayer = delayer;
        }

        public async Task<object> ExecuteScriptAndWait(string script, int millisecondsDelay, params object[] args)
        {
            object result = JSExecutor.ExecuteScript(script, args);
            await Delayer.Delay(millisecondsDelay);
            return result;
        }
    }
}