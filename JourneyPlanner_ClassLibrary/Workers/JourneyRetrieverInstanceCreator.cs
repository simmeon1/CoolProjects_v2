using System;
using System.Diagnostics;

namespace JourneyPlanner_ClassLibrary
{
    public class JourneyRetrieverInstanceCreator : IJourneyRetrieverInstanceCreator
    {
        public IJourneyRetriever CreateInstance(string fullClassName, JourneyRetrieverComponents c = null)
        {
            return (IJourneyRetriever)Activator.CreateInstance(Type.GetType(fullClassName), c ?? new());
        }
    }
}