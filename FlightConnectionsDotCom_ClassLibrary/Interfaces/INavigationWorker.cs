using OpenQA.Selenium;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary.Interfaces
{
    public interface INavigationWorker
    {
        Task GoToUrl(INavigation navigation, string path, bool openInNewTab = false);
    }
}