using Common_ClassLibrary;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
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

        public IWebElement FindElementWithAttribute(By by, string attribute = "innerText", string text = "", bool clickElement = true, ISearchContext container = null)
        {
            IWebElement button = WebDriverWaitProvider.Until(d =>
            {
                if (container == null) container = d;
                while (true)
                {
                    try
                    {
                        ReadOnlyCollection<IWebElement> webElements = container.FindElements(by);
                        foreach (IWebElement webElement in webElements)
                        {
                            string attr = webElement.GetAttribute(attribute);
                            if (attr == null) attr = "";
                            if (attr.Trim().ToLower().Contains(text.ToLower())) return webElement;
                        }
                        return null;
                    }
                    catch (StaleElementReferenceException)
                    {
                        //try again
                    }
                }
            });
            if (clickElement) ClickElementWhenClickable(button);
            return button;
        }

        public void ClickElementWhenClickable(IWebElement element)
        {
            while (true)
            {
                try
                {
                    WebDriverWaitProvider.Until(ExpectedConditions.ElementToBeClickable(element));
                    element.Click();
                    return;
                }
                catch (ElementClickInterceptedException)
                {
                    //try again
                }
            }
        }
    }
}