﻿using Common_ClassLibrary;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public class JourneyRetrieverComponents
    {
        public IWebDriver Driver { get; set; }
        public ILogger Logger { get; set; }
        public IDelayer Delayer { get; set; }
        public IJavaScriptExecutor JavaScriptExecutor { get; set; }
        public IWebDriverWaitProvider WebDriverWaitProvider { get; set; }
        public IHttpClient HttpClient { get; set; }
        public JourneyRetrieverComponents(IWebDriver driver, ILogger logger, IWebDriverWaitProvider webDriverWaitProvider, IDelayer delayer, IHttpClient httpClient, IJavaScriptExecutor javaScriptExecutor)
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
            if (navigation != null) navigation.GoToUrl(url);
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

        public IWebElement FindElementByAttribute(By by, string attribute = "innerText", string text = "", ISearchContext container = null, int indexOfElement = -1, int seconds = 10)
        {
            IWebElement element = WebDriverWaitProvider.Until(d =>
            {
                if (container == null) container = d;
                while (true)
                {
                    try
                    {
                        ReadOnlyCollection<IWebElement> webElements = container.FindElements(by);
                        for (int i = 0; i < webElements.Count; i++)
                        {
                            IWebElement webElement = webElements[i];
                            string attr = webElement.GetAttribute(attribute);
                            if (attr == null) attr = "";
                            if (attr.Trim().ToLower().Contains(text.ToLower()))
                            {
                                if (indexOfElement == -1 || i == indexOfElement) return webElement;
                            }
                        }
                        return null;
                    }
                    catch (WebDriverTimeoutException ex)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Log("Error with finding element by attribute. Details:");
                        Log(ex.ToString());
                    }
                }
            }, seconds);
            return element;
        }

        public IWebElement FindElementByAttributeAndClickIt(By by, string attribute = "innerText", string text = "", ISearchContext container = null, int indexOfElement = -1, bool clickElement = true, int seconds = 10)
        {
            while (true)
            {
                try
                {
                    IWebElement element = FindElementByAttribute(by, attribute, text, container, indexOfElement, seconds);
                    WebDriverWaitProvider.Until(WebDriverWaitProvider.ElementIsClickable(element));
                    if (clickElement) element.Click();
                    return element;
                }
                catch (WebDriverTimeoutException ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Log("Error with finding element by attribute and clicking. Details:");
                    Log(ex.ToString());
                }
            }
        }

        public IWebElement FindElementByAttributeAndSendKeysToIt(By by, string attribute = "innerText", string text = "", ISearchContext container = null, int indexOfElement = -1, List<string> keys = null, bool doClearFirst = true, int seconds = 10)
        {
            if (keys == null) keys = new();
            while (true)
            {
                try
                {
                    IWebElement element = FindElementByAttribute(by, attribute, text, container, indexOfElement, seconds);
                    if (doClearFirst) element.Clear();
                    foreach (string key in keys)
                    {
                        element.SendKeys(key);
                    }
                    return element;
                }
                catch (WebDriverTimeoutException ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Log("Error with finding element and sending keys. Details:");
                    Log(ex.ToString());
                }
            }
        }

        public ReadOnlyCollection<IWebElement> FindElementsNew(By by, string attribute = "innerText", string text = "", ISearchContext container = null, int seconds = 10)
        {
            ReadOnlyCollection<IWebElement> elements = WebDriverWaitProvider.Until(d =>
            {
                if (container == null) container = d;
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
                    catch (WebDriverTimeoutException ex)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Log("Error with finding multiple elements. Details:");
                        Log(ex.ToString());
                    }
                }
            }, seconds);
            return elements;
        }
    }
}