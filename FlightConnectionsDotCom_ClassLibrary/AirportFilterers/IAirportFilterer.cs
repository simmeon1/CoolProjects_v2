using OpenQA.Selenium;
using System;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface IAirportFilterer
    {
        bool AirportMeetsCondition(Airport airport);
    }
}