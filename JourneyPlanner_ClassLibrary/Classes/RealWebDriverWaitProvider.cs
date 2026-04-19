using System;
using JourneyPlanner_ClassLibrary.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace JourneyPlanner_ClassLibrary.Classes;

public class RealWebDriverWaitProvider(IWebDriver driver) : IWebDriverWaitProvider
{
    public IAlert WaitUntilAlertIsPresent()
    {
        WebDriverWait wait = new (driver, TimeSpan.FromMilliseconds(100));
        return wait.Until(ExpectedConditions.AlertIsPresent());
    }
}