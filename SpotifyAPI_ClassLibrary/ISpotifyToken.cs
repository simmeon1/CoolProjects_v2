using System;

namespace ClassLibrary
{
    public interface ISpotifyToken
    {
        string GetAccessToken();
        string GetTokenType();
        int GetExpiresIn();
        string GetScope();
        DateTime? GetDateTimeCreated();
    }
}