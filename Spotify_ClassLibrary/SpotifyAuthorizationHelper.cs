using System.Web;

namespace Spotify_ClassLibrary;

public class SpotifyAuthorizationHelper
{
    public static string GetEncodedAuthorizeUrl(string clientId, string callback)
    {
        string scopes = "ugc-image-upload user-read-playback-state user-modify-playback-state user-read-currently-playing app-remote-control streaming playlist-read-private playlist-read-collaborative playlist-modify-private playlist-modify-public user-follow-modify user-follow-read user-read-playback-position user-top-read user-read-recently-played user-library-modify user-library-read user-read-email user-read-private user-read-email";
        return $"https://accounts.spotify.com/authorize?response_type=code&client_id={clientId}&scope={HttpUtility.UrlEncode(scopes)}&redirect_uri={HttpUtility.UrlEncode(callback)}";
    }

    public static string GetBase64EncodedString(string clientId, string clientSecret)
    {
        byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
        return Convert.ToBase64String(plainTextBytes);
    }
}