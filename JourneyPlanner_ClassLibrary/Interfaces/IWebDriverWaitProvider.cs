using OpenQA.Selenium;
using System;

namespace JourneyPlanner_ClassLibrary
{
    public interface IWebDriverWaitProvider
    {
        TResult Until<TResult>(Func<IWebDriver, TResult> condition, int seconds = 10);
        IAlert WaitUntilAlertIsPresent();
    }
}