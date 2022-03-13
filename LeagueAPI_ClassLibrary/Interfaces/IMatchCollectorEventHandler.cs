using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public interface IMatchCollectorEventHandler
    {
        void CollectingStarted();
        void MatchAdded(List<LeagueMatch> matches);
        void CollectingFinished();
    }
}