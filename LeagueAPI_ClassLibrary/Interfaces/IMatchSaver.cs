using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public interface IMatchSaver
    {
        List<string> SaveMatches(List<LeagueMatch> matches);
    }
}