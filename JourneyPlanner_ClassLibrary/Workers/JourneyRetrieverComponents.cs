using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Interfaces;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class JourneyRetrieverComponents
    {
        public IWebDriver Driver { get; }
        public ILogger Logger { get; }
        public IDelayer Delayer { get; }
        public IJavaScriptExecutor JavaScriptExecutor { get; }
        public IWebDriverWaitProvider WebDriverWaitProvider { get; }
        public IHttpClient HttpClient { get; }

        public JourneyRetrieverComponents(
            IWebDriver driver,
            ILogger logger,
            IWebDriverWaitProvider webDriverWaitProvider,
            IDelayer delayer,
            IHttpClient httpClient,
            IJavaScriptExecutor javaScriptExecutor
        )
        {
            Driver = driver;
            Logger = logger;
            WebDriverWaitProvider = webDriverWaitProvider;
            HttpClient = httpClient;
            Delayer = delayer;
            JavaScriptExecutor = javaScriptExecutor;
        }

        public void Log(string message)
        {
            Logger.Log(message);
        }

        public void NavigateToUrl(string url)
        {
            INavigation navigation = Driver.Navigate();
            navigation?.GoToUrl(url);
        }

        public IWebElement FindElement(FindElementParameters p)
        {
            return DoActionWithElement(() => FindEl(p));
        }
        
        public ReadOnlyCollection<IWebElement> FindElements(FindElementParameters p)
        {
            return DoActionWithElement(() => (p.Container ?? Driver).FindElements(p.BySelector));
        }

        public IWebElement FindElementAndClickIt(FindElementParameters p)
        {
            return DoActionWithElement(
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
                () =>
                {
                    IWebElement element = FindEl(p);
                    if (doClearFirst) element.Clear();
                    element.SendKeys(keys);
                    return element;
                }
            );
        }

        private IWebElement FindEl(FindElementParameters p)
        {
            return WebDriverWaitProvider.Until(
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

        private T DoActionWithElement<T>(Func<T> func)
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
                    HandleException(ex, ref fails, "Error clicking element.");
                }
            }
        }

        private void HandleException(Exception ex, ref int fails, string message)
        {
            if (ex is WebDriverTimeoutException) throw ex;
            if (ex.Message.Contains("Only one usage of each socket address"))
            {
                const int milliseconds = 120000;
                Log(
                    $"Sockets used up. Waiting {TimeSpan.FromMilliseconds(milliseconds).Minutes} minutes. Continues at {DateTime.Now.AddMilliseconds(milliseconds).ToString()}"
                );
                Delayer.Sleep(milliseconds);
                return;
            }

            if (fails++ < 100) return;
            Log($"{message}. Details:");
            Log(ex.ToString());
            throw ex;
        }
    }
}