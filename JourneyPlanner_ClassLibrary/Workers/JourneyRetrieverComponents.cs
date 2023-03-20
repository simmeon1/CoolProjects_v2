using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Interfaces;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class JourneyRetrieverComponents
    {
        private readonly IWebDriver driver;
        private readonly ILogger logger;
        private readonly IDelayer delayer;
        private readonly IJavaScriptExecutor jsExecutor;
        private readonly IWebDriverWaitProvider wait;
        private readonly IHttpClient http;

        public JourneyRetrieverComponents(
            IWebDriver driver,
            ILogger logger,
            IWebDriverWaitProvider wait,
            IDelayer delayer,
            IHttpClient http,
            IJavaScriptExecutor jsExecutor
        )
        {
            this.driver = driver;
            this.logger = logger;
            this.wait = wait;
            this.http = http;
            this.delayer = delayer;
            this.jsExecutor = jsExecutor;
        }

        public void Log(string message)
        {
            logger.Log(message);
        }

        public void NavigateToUrl(string url)
        {
            INavigation navigation = driver.Navigate();
            navigation?.GoToUrl(url);
        }

        public IWebElement FindElement(FindElementParameters p)
        {
            return DoActionWithElement(p, () => FindEl(p));
        }

        public ReadOnlyCollection<IWebElement> FindElements(FindElementParameters p)
        {
            return DoActionWithElement(p, () => (p.Container ?? driver).FindElements(p.BySelector));
        }

        public IWebElement FindElementAndClickIt(FindElementParameters p)
        {
            return DoActionWithElement(
                p,
                () =>
                {
                    IWebElement element = FindEl(p);
                    element.Click();
                    return element;
                }
            );
        }

        public IWebElement FindElementAndSendKeysToIt(FindElementParameters p, bool doClearFirst, string keys)
        {
            return DoActionWithElement(
                p,
                () =>
                {
                    IWebElement element = FindEl(p);
                    if (doClearFirst) element.Clear();
                    element.SendKeys(keys);
                    return element;
                }
            );
        }

        public object ExecuteScript(string script, params object[] args)
        {
            return jsExecutor.ExecuteScript(script, args);
        }

        public TResult Until<TResult>(Func<IWebDriver, TResult> condition, int seconds = 10)
        {
            return wait.Until(condition, seconds);
        }

        public void Sleep(int milliseconds)
        {
            delayer.Sleep(milliseconds);
        }

        public object GetLoggerContent()
        {
            return logger.GetContent();
        }

        public Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            return http.SendRequest(request);
        }

        private IWebElement FindEl(FindElementParameters p)
        {
            return wait.Until(
                d =>
                {
                    int index = 0;
                    ReadOnlyCollection<IWebElement> webElements = (p.Container ?? d).FindElements(p.BySelector);
                    foreach (IWebElement webElement in webElements)
                    {
                        if (p.Matcher != null && !p.Matcher.Invoke(webElement)) continue;
                        if (index++ == p.Index) return webElement;
                    }

                    return null;
                },
                p.Seconds
            );
        }

        private T DoActionWithElement<T>(FindElementParameters p, Func<T> func)
        {
            int fails = 0;
            while (true)
            {
                try
                {
                    return func.Invoke();
                }
                catch (Exception ex)
                {
                    HandleException(ex, ref fails, p);
                }
            }
        }

        private void HandleException(Exception ex, ref int fails, FindElementParameters p)
        {
            if (ex is WebDriverTimeoutException) throw ex;
            if (ex.Message.Contains("Only one usage of each socket address"))
            {
                const int milliseconds = 120000;
                Log(
                    $"Sockets used up. Waiting {TimeSpan.FromMilliseconds(milliseconds).Minutes} minutes. Continues at {DateTime.Now.AddMilliseconds(milliseconds).ToString()}"
                );
                delayer.Sleep(milliseconds);
                return;
            }

            if (fails++ < 100) return;
            Log($"Error executing action with parameters:");
            Log(p.GetDescription());
            Log($"Exception details:");
            Log(ex.ToString());
            throw ex;
        }
    }
}