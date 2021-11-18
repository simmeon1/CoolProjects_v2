using Common_ClassLibrary;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JourneyPlanner_ClassLibrary
{
    public class JourneyRetrieverComponents
    {
        public IJourneyRetrieverEventHandler JourneyRetrieverEventHandler { get; set; }
        public IWebDriver Driver { get; set; }
        public ILogger Logger { get; set; }
        public IWebDriverWaitProvider WebDriverWaitProvider { get; set; }
        public IHttpClient HttpClient { get; set; }
        public JourneyRetrieverComponents(IJourneyRetrieverEventHandler journeyRetrieverEventHandler, IWebDriver driver, ILogger logger, IWebDriverWaitProvider webDriverWaitProvider, IHttpClient httpClient)
        {
            Driver = driver;
            Logger = logger;
            JourneyRetrieverEventHandler = journeyRetrieverEventHandler;
            WebDriverWaitProvider = webDriverWaitProvider;
            HttpClient = httpClient;
        }

        public void Log(string message)
        {
            Logger.Log(message);
        }

        public void NavigateToUrl(string url)
        {
            INavigation navigation = Driver.Navigate();
            if (navigation != null) navigation.GoToUrl(url);
        }

        public IWebElement FindElementByAttribute(By by, string attribute = "innerText", string text = "", ISearchContext container = null, int indexOfElement = -1)
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
                                if (indexOfElement < 0 || i == indexOfElement) return webElement;
                            }
                        }
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Log("Error with finding element by attribute. Details:");
                        Log(ex.ToString());
                        if (ex.GetType().Equals(typeof(WebDriverTimeoutException))) throw;
                    }
                }
            });
            return element;
        }

        public IWebElement FindElementByAttributeAndClickIt(By by, string attribute = "innerText", string text = "", ISearchContext container = null, int indexOfElement = -1)
        {
            while (true)
            {
                try
                {
                    IWebElement element = FindElementByAttribute(by, attribute, text, container, indexOfElement);
                    WebDriverWaitProvider.Until(ExpectedConditions.ElementToBeClickable(element));
                    element.Click();
                    return element;
                }
                catch (Exception ex)
                {
                    Log("Error with finding element by attribute and clicking. Details:");
                    Log(ex.ToString());
                    if (ex.GetType().Equals(typeof(WebDriverTimeoutException))) throw;
                }
            }
        }

        public IWebElement FindElementByAttributeAndSendKeysToIt(By by, string attribute = "innerText", string text = "", ISearchContext container = null, int indexOfElement = -1, List<string> keys = null, bool doClearFirst = true)
        {
            if (keys == null) keys = new();
            while (true)
            {
                try
                {
                    IWebElement element = FindElementByAttribute(by, attribute, text, container, indexOfElement);
                    if (doClearFirst) element.Clear();
                    foreach (string key in keys)
                    {
                        element.SendKeys(key);
                    }
                    return element;
                }
                catch (Exception ex)
                {
                    Log("Error with finding element by attribute and sending keys. Details:");
                    Log(ex.ToString());
                    if (ex.GetType().Equals(typeof(WebDriverTimeoutException))) throw;
                }
            }
        }

        public ReadOnlyCollection<IWebElement> FindElementsNew(By by, string attribute = "innerText", string text = "", ISearchContext container = null)
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
                    catch (Exception ex)
                    {
                        Log("Error with finding elements. Details:");
                        Log(ex.ToString());
                        if (ex.GetType().Equals(typeof(WebDriverTimeoutException))) throw;
                    }
                }
            });
            return elements;
        }
    }
}