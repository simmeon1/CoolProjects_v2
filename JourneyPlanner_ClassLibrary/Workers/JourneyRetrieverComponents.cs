using Common_ClassLibrary;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.Workers;

public record JourneyRetrieverComponents(
    IWebDriver driver,
    ILogger logger,
    IJavaScriptExecutor jsExecutor
);