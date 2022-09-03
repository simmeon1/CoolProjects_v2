﻿using System;
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

        public async Task Delay(int milliseconds)
        {
            await Delayer.Delay(milliseconds);
        }

        public void NavigateToUrl(string url)
        {
            INavigation navigation = Driver.Navigate();
            navigation?.GoToUrl(url);
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

        public IWebElement FindElement(FindElementParameters p)
        {
            IWebElement element = WebDriverWaitProvider.Until(
                d =>
                {
                    int fails = 0;
                    try
                    {
                        int index = -1;
                        ReadOnlyCollection<IWebElement> webElements = (p.Container ?? d).FindElements(p.BySelector);
                        foreach (IWebElement webElement in webElements)
                        {
                            if (p.Matcher != null)
                            {
                                if (!p.Matcher.Invoke(webElement)) continue;
                                index++;
                                if (index == p.Index) return webElement;
                            }
                            else
                            {
                                index++;
                                if (index == p.Index) return webElement;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex, ref fails, "Error with finding element.");
                    }
                    return null;
                },
                p.Seconds
            );
            return element;
        }
        
        public IWebElement FindElementAndClickIt(FindElementParameters p)
        {
            int fails = 0;
            while (true)
            {
                try
                {
                    IWebElement element = FindElement(p);
                    element.Click();
                    return element;
                }
                catch (Exception ex)
                {
                    HandleException(ex, ref fails, "Error clicking element.");
                }
            }
        }
        
        public void FindElementAndSendKeysToIt(FindElementParameters p, bool doClearFirst, List<string> keys)
        {
            int fails = 0;
            while (true)
            {
                try
                {
                    IWebElement element = FindElement(p);
                    if (doClearFirst) element.Clear();
                    foreach (string key in keys)
                    {
                        element.SendKeys(key);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    HandleException(ex, ref fails, "Error clicking element.");
                }
            }
        }
    }
}