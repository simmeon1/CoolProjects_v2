using OpenQA.Selenium;
using System;

namespace JourneyPlanner_ClassLibrary
{
    public interface IWebDriverWait
    {
        IAlert Until(Func<IWebDriver, IAlert> func);
    }
}