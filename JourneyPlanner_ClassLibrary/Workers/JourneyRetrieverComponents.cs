using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Interfaces;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.Workers;

public record JourneyRetrieverComponents(
    IWebDriver driver,
    ILogger logger,
    IWebDriverWaitProvider wait,
    IDelayer delayer,
    IJavaScriptExecutor jsExecutor
);