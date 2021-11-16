using Common_ClassLibrary;
using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public class JourneyRetrieverComponents
    {
        public IJourneyRetrieverEventHandler JourneyRetrieverEventHandler { get; set; }
        public IWebDriver Driver { get; set; }
        public ILogger Logger { get; set; }
        public IDelayer Delayer { get; set; }
        public int DefaultDelay { get; set; } = 500;
        public IWebDriverWaitProvider WebDriverWaitProvider { get; set; }
        public JourneyRetrieverComponents(IJourneyRetrieverEventHandler journeyRetrieverEventHandler, IWebDriver driver, ILogger logger, IDelayer delayer, int defaultDelay, IWebDriverWaitProvider webDriverWaitProvider)
        {
            Driver = driver;
            Logger = logger;
            Delayer = delayer;
            DefaultDelay = defaultDelay;
            JourneyRetrieverEventHandler = journeyRetrieverEventHandler;
            WebDriverWaitProvider = webDriverWaitProvider;
        }

        public void Log(string message)
        {
            Logger.Log(message);
        }

        public async Task Delay(int milliseconds)
        {
            await Delayer.Delay(milliseconds);
        }

        public async Task<ReadOnlyCollection<IWebElement>> FindElementsAndWait(IWebElement element, By by)
        {
            ReadOnlyCollection<IWebElement> result = element.FindElements(by);
            await Delay(DefaultDelay);
            return result;
        }

        public async Task<ReadOnlyCollection<IWebElement>> FindElementsAndWait(By by)
        {
            ReadOnlyCollection<IWebElement> result = Driver.FindElements(by);
            await Delay(DefaultDelay);
            return result;
        }

        public void NavigateToUrl(string url)
        {
            INavigation navigation = Driver.Navigate();
            if (navigation != null) navigation.GoToUrl(url);
        }

        public async Task<IWebElement> FindElementAndWait(By by)
        {
            IWebElement result = Driver.FindElement(by);
            await Delay(DefaultDelay);
            return result;
        }

        public async Task<IWebElement> FindElementAndWait(IWebElement element, By by)
        {
            IWebElement result = element.FindElement(by);
            await Delay(DefaultDelay);
            return result;
        }

        public async Task ClickAndWait(IWebElement element)
        {
            element.Click();
            await Delay(DefaultDelay);
        }
    }
}