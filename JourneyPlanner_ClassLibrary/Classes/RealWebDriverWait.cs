using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Diagnostics;

namespace JourneyPlanner_ClassLibrary
{
    public class RealWebDriverWait : IWebDriverWait
    {
        private IWebDriver Driver { get; set; }

        public RealWebDriverWait(IWebDriver driver)
        {
            Driver = driver;
        }

        public IAlert Until(Func<IWebDriver, IAlert> func)
        {
            WebDriverWait wait = new(Driver, TimeSpan.FromMilliseconds(100));
            return wait.Until(ExpectedConditions.AlertIsPresent());
        }
    }
}