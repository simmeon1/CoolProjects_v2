namespace ClassLibrary
{
    public interface ISpotifyToken
    {
        string GetAccessToken();
        string GetTokenType();
        int GetExpiresIn();
        string GetScope();
    }
}