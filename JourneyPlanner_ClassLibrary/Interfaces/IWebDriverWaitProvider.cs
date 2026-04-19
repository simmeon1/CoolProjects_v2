using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.Interfaces;

public interface IWebDriverWaitProvider
{
    IAlert WaitUntilAlertIsPresent();
}