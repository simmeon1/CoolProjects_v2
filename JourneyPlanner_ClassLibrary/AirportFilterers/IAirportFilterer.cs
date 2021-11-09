using OpenQA.Selenium;
using System;

namespace JourneyPlanner_ClassLibrary
{
    public interface IAirportFilterer
    {
        bool AirportMeetsCondition(Airport airport);
    }
}