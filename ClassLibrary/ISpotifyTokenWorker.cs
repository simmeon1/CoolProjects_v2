using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class ISpotifyTokenWorker
    {
        public static bool TokensHaveTheSameData(ISpotifyToken tokenOne, ISpotifyToken tokenTwo)
        {
            return tokenOne.GetAccessToken().Equals(tokenTwo.GetAccessToken()) &&
                tokenOne.GetExpiresIn() == tokenTwo.GetExpiresIn() &&
                tokenOne.GetScope().Equals(tokenTwo.GetScope()) &&
                tokenOne.GetTokenType().Equals(tokenTwo.GetTokenType());
        }
    }
}
