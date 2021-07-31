namespace SpotifyAPI_ClassLibrary
{
    public interface ISpotifyCredentials
    {
        string GetRefreshToken();
        string GetEncodedSecret();
    }
}