using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JourneyPlanner_ClassLibrary
{
    public interface IJourneyRetrieverEventHandler
    {
        void InformOfPathDataFullyCollected(string path);
    }
}