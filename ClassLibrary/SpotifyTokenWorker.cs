using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
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
            return dateTimeProvider.GetDateTimeNow().CompareTo(tokenExpirationTime) <= 0;
        }
    }
}
