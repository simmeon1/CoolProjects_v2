using System;

namespace SpotifyAPI_ClassLibrary
{
    public class SpotifyToken : ISpotifyToken
    {
        public SpotifyToken(string accessToken, int expiresIn, string scope, string tokenType, DateTime? dateTimeCreated)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            Scope = scope;
            TokenType = tokenType;
            DateTimeCreated = dateTimeCreated;
        }

        private string AccessToken { get; set; }
        private int ExpiresIn { get; set; }
        private string Scope { get; set; }
        private string TokenType { get; set; }
        private DateTime? DateTimeCreated { get; set; }

        public string GetAccessToken()
        {
            return AccessToken;
        }

        public DateTime? GetDateTimeCreated()
        {
            return DateTimeCreated;
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