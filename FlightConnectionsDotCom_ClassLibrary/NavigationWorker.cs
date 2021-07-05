using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class NavigationWorker : INavigationWorker
    {
        public void GoToUrl(INavigation navigation, string path)
        {
            navigation.GoToUrl(path);
        }
    }
}
