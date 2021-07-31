using Common_ClassLibrary;
using System;

namespace SpotifyAPI_ClassLibrary
{
    public interface ISpotifyTokenWorker
    {
        bool TokensHaveTheSameData(ISpotifyToken tokenToConfirm, ISpotifyToken token);
        bool TokenIsStillValid(ISpotifyToken token, IDateTimeProvider dateTimeProvider);
        ISpotifyToken CreateTokenObject(string accessToken, int expiresIn, string scope, string tokenType, DateTime? dateTimeCreated);
    }
}