using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public interface IMatchAddedHandler
    {
        void MatchAdded(List<LeagueMatch> matches);
    }
}