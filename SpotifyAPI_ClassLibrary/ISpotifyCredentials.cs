namespace ClassLibrary
{
    public interface ISpotifyCredentials
    {
        string GetRefreshToken();
        string GetEncodedSecret();
    }
}