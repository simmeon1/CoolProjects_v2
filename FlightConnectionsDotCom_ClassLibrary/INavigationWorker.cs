using OpenQA.Selenium;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface INavigationWorker
    {
        Task GoToUrl(INavigation navigation, string path);
    }
}