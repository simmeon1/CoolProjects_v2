namespace ClassLibrary
{
    public interface ISpotifyTokenWorker
    {
        bool TokensHaveTheSameData(ISpotifyToken tokenToConfirm, ISpotifyToken token);
        bool TokenIsStillValid(ISpotifyToken token, IDateTimeProvider dateTimeProvider);
    }
}