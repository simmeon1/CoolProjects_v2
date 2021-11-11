namespace JourneyPlanner_ClassLibrary
{
    public interface IJourneyRetrieverInstanceCreator
    {
        IJourneyRetriever CreateInstance(string className, JourneyRetrieverComponents c);
    }
}