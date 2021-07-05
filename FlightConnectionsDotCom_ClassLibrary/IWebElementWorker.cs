using OpenQA.Selenium;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface IWebElementWorker
    {
        void Click(IWebElement fromField);
        void SendKeys(IWebElement fromDivInput, string v);
    }
}