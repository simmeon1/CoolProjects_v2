using OpenQA.Selenium;
using System;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface IWebDriverWait
    {
        IAlert Until(Func<IWebDriver, IAlert> func);
    }
}