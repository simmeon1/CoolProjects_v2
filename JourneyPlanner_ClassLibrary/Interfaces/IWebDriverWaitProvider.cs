using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.Interfaces;

public interface IWebDriverWaitProvider
{
    TResult Until<TResult>(
        Func<IWebDriver, TResult?> condition,
        IEnumerable<Type>? exceptionTypes = null,
        int seconds = 10
    );

    IAlert WaitUntilAlertIsPresent();
    Func<IWebDriver, IWebElement> ElementIsClickable(IWebElement element);
}