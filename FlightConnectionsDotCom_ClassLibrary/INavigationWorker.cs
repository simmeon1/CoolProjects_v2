using OpenQA.Selenium;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface INavigationWorker
    {
        void GoToUrl(INavigation navigation, string path);
    }
}