using JourneyPlanner_ClassLibrary.Workers;

namespace JourneyPlanner_ClassLibrary.Interfaces
{
    public interface IJourneyRetrieverInstanceCreator
    {
        IJourneyRetriever CreateInstance(string className, JourneyRetrieverComponents c);
    }
}