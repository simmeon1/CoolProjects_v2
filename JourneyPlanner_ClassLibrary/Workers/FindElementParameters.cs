using System;
using OpenQA.Selenium;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class FindElementParameters
    {
        public By BySelector;
        public Func<IWebElement, bool> Matcher;
        public ISearchContext Container;
        public int Seconds = 10;
        public int Index = 0;

        public static FindElementParameters WithSelector(By by)
        {
            return new FindElementParameters
            {
                BySelector = by
            };
        }
    }
}