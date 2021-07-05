﻿using OpenQA.Selenium;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface IWebElementWorker
    {
        void Click(IWebElement element);
        void SendKeys(IWebElement element, string keys);
    }
}