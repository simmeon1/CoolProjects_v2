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

        public JourneyRetrieverComponents(IWebDriver driver, ILogger logger,
            IWebDriverWaitProvider webDriverWaitProvider, IDelayer delayer, IHttpClient httpClient,
            IJavaScriptExecutor javaScriptExecutor)
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

        public async Task Delay(int milliseconds)
        {
            await Delayer.Delay(milliseconds);
        }

        public void NavigateToUrl(string url)
        {
            INavigation navigation = Driver.Navigate();
            navigation?.GoToUrl(url);
        }

        public bool ElementsContainsClass(IWebElement element, string targetCls)
        {
            string[] elementClasses = element.GetAttribute("class").Split(" ", StringSplitOptions.RemoveEmptyEntries);
            foreach (string cls in elementClasses)
            {
                if (cls != null && cls.Equals(targetCls)) return true;
            }

            return false;
        }

        public IWebElement FindElementByAttribute(By by, string attribute = "innerText", string text = "",
            ISearchContext container = null, int indexOfElement = -1, int seconds = 10)
        {
            IWebElement element = WebDriverWaitProvider.Until(d =>
            {
                container ??= d;
                int fails = 0;
                while (true)
                {
                    try
                    {
                        ReadOnlyCollection<IWebElement> webElements = container.FindElements(by);
                        for (int i = 0; i < webElements.Count; i++)
                        {
                            IWebElement webElement = webElements[i];
                            string attr = webElement.GetAttribute(attribute) ?? "";
                            if (!attr.Trim().ToLower().Contains(text.ToLower())) continue;
                            if (indexOfElement == -1 || i == indexOfElement) return webElement;
                        }
                        return null;
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex, ref fails, "Error with finding element by attribute.");
                    }
                }
            }, seconds);
            return element;
        }

        private void HandleException(Exception ex, ref int fails, string message)
        {
            if (ex is WebDriverTimeoutException) throw ex;
            if (ex.Message.Contains("Only one usage of each socket address"))
            {
                const int milliseconds = 120000;
                Log(
                    $"Sockets used up. Waiting {TimeSpan.FromMilliseconds(milliseconds).Minutes} minutes. Continues at {DateTime.Now.AddMilliseconds(milliseconds).ToString()}");
                Delayer.Sleep(milliseconds);
                return;
            }

            if (fails++ < 100) return;
            Log($"{message}. Details:");
            Log(ex.ToString());
            throw ex;
        }

        public IWebElement FindElementByAttributeAndClickIt(By by, string attribute = "innerText", string text = "",
            ISearchContext container = null, int indexOfElement = -1, bool clickElement = true, int seconds = 10)
        {
            int fails = 0;
            while (true)
            {
                try
                {
                    IWebElement element =
                        FindElementByAttribute(by, attribute, text, container, indexOfElement, seconds);
                    WebDriverWaitProvider.Until(WebDriverWaitProvider.ElementIsClickable(element));
                    if (clickElement) element.Click();
                    return element;
                }
                catch (Exception ex)
                {
                    HandleException(ex, ref fails, "Error with finding element by attribute and clicking.");
                }
            }
        }

        public IWebElement FindElementByAttributeAndSendKeysToIt(By by, string attribute = "innerText",
            string text = "", ISearchContext container = null, int indexOfElement = -1, List<string> keys = null,
            bool doClearFirst = true, int seconds = 10)
        {
            keys ??= new List<string>();
            int fails = 0;
            while (true)
            {
                try
                {
                    IWebElement element =
                        FindElementByAttribute(by, attribute, text, container, indexOfElement, seconds);
                    if (doClearFirst) element.Clear();
                    foreach (string key in keys)
                    {
                        element.SendKeys(key);
                    }
                    return element;
                }
                catch (Exception ex)
                {
                    HandleException(ex, ref fails, "Error with finding element and sending keys.");
                }
            }
        }

        public ReadOnlyCollection<IWebElement> FindElementsNew(By by, string attribute = "innerText", string text = "",
            ISearchContext container = null, int seconds = 10)
        {
            ReadOnlyCollection<IWebElement> elements = WebDriverWaitProvider.Until(d =>
            {
                container ??= d;
                int fails = 0;
                while (true)
                {
                    List<IWebElement> results = new();
                    try
                    {
                        ReadOnlyCollection<IWebElement> webElements = container.FindElements(by);
                        foreach (IWebElement webElement in webElements)
                        {
                            string attr = webElement.GetAttribute(attribute);
                            if (attr == null) attr = "";
                            if (attr.Trim().ToLower().Contains(text.ToLower()))
                            {
                                results.Add(webElement);
                            }
                        }
                        return new ReadOnlyCollection<IWebElement>(results);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex, ref fails, "Error with finding multiple elements.");
                    }
                }
            }, seconds);
            return elements;
        }
    }
}