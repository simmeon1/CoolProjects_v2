namespace ClassLibrary
{
    public class SpotifyToken : ISpotifyToken
    {
        public SpotifyToken(string accessToken, int expiresIn, string scope, string tokenType)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            Scope = scope;
            TokenType = tokenType;
        }

        private string AccessToken { get; set; }
        private int ExpiresIn { get; set; }
        private string Scope { get; set; }
        private string TokenType { get; set; }

        public string GetAccessToken()
        {
            return AccessToken;
        }

        public int GetExpiresIn()
        {
            return ExpiresIn;
        }

        public string GetScope()
        {
            return Scope;
        }

        public string GetTokenType()
        {
            return TokenType;
        }
    }
}