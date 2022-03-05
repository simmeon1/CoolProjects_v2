using System;
using JourneyPlanner_ClassLibrary.Interfaces;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class JourneyRetrieverInstanceCreator : IJourneyRetrieverInstanceCreator
    {
        public IJourneyRetriever CreateInstance(string fullClassName, JourneyRetrieverComponents c)
        {
            return (IJourneyRetriever)Activator.CreateInstance(Type.GetType(fullClassName), c);
        }
    }
}