namespace SpotifyAPI_ClassLibrary
{
    public class SpotifyCredentials : ISpotifyCredentials
    {
        public string EncodedSecret { get; set; }
        public string RefreshToken { get; set; }
        public string GetEncodedSecret()
        {
            return EncodedSecret;
        }

        public string GetRefreshToken()
        {
            return RefreshToken;
        }
    }
}