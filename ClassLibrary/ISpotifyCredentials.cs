namespace ClassLibrary
{
    public interface ISpotifyCredentials
    {
        string GetUserId();
        string GetRefreshToken();
        string GetEncodedSecret();
    }
}