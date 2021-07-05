using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class WebElementWorker : IWebElementWorker
    {
        public void Click(IWebElement element)
        {
            element.Click();
        }

        public void SendKeys(IWebElement element, string keys)
        {
            element.SendKeys(keys);
        }
    }
}
