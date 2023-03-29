namespace Spotify_ClassLibrary;

public class SpotifyCredentials
{
    public string AccessToken { get; }
    public string TokenType { get; }
    public int ExpiresIn { get; }
    public string RefreshToken { get; }
    public string Scope { get; }

    public SpotifyCredentials(string accessToken, string tokenType, int expiresIn, string refreshToken, string scope)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        ExpiresIn = expiresIn;
        RefreshToken = refreshToken;
        Scope = scope;
    }
}