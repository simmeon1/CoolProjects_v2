using Common_ClassLibrary;
using System;

namespace SpotifyAPI_ClassLibrary
{
    public class SpotifyTokenWorker : ISpotifyTokenWorker
    {
        public bool TokensHaveTheSameData(ISpotifyToken tokenOne, ISpotifyToken tokenTwo)
        {
            return tokenOne.GetAccessToken().Equals(tokenTwo.GetAccessToken()) &&
                tokenOne.GetExpiresIn() == tokenTwo.GetExpiresIn() &&
                tokenOne.GetScope().Equals(tokenTwo.GetScope()) &&
                tokenOne.GetTokenType().Equals(tokenTwo.GetTokenType()) &&
                tokenOne.GetDateTimeCreated().Value.CompareTo(tokenTwo.GetDateTimeCreated().Value) == 0;
        }

        public bool TokenIsStillValid(ISpotifyToken token, IDateTimeProvider dateTimeProvider)
        {
            DateTime? tokenCreationTime = token?.GetDateTimeCreated();
            if (tokenCreationTime == null) return false;
            DateTime tokenExpirationTime = token.GetDateTimeCreated().Value.AddSeconds(token.GetExpiresIn());
            return dateTimeProvider.Now().CompareTo(tokenExpirationTime) <= 0;
        }

        public ISpotifyToken CreateTokenObject(string accessToken, int expiresIn, string scope, string tokenType, DateTime? dateTimeCreated)
        {
            return new SpotifyToken(accessToken, expiresIn, scope, tokenType, dateTimeCreated);
        }
    }
}
