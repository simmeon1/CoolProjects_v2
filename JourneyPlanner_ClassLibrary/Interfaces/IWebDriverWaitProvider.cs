using System;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.Interfaces
{
    public interface IWebDriverWaitProvider
    {
        TResult Until<TResult>(Func<IWebDriver, TResult> condition, int seconds = 10);
        IAlert WaitUntilAlertIsPresent();
        Func<IWebDriver, IWebElement> ElementIsClickable(IWebElement element);
    }
}