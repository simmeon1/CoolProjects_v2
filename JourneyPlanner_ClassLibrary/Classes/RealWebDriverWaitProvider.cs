using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Diagnostics;

namespace JourneyPlanner_ClassLibrary
{
    public class RealWebDriverWaitProvider : IWebDriverWaitProvider
    {
        private IWebDriver Driver { get; set; }

        public RealWebDriverWaitProvider(IWebDriver driver)
        {
            Driver = driver;
        }

        public IAlert WaitUntilAlertIsPresent()
        {
            WebDriverWait wait = new(Driver, TimeSpan.FromMilliseconds(100));
            return wait.Until(ExpectedConditions.AlertIsPresent());
        }

        public TResult Until<TResult>(Func<IWebDriver, TResult> condition, int seconds = 10)
        {
            WebDriverWait wait = new(Driver, TimeSpan.FromSeconds(seconds));
            return wait.Until(condition);
        }

        public Func<IWebDriver, IWebElement> ElementIsClickable(IWebElement element)
        {
            return ExpectedConditions.ElementToBeClickable(element);
        }
    }
}