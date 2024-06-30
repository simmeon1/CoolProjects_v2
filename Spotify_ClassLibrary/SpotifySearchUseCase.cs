using System.Runtime.InteropServices;
using System.Text.Json;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class SpotifySearchUseCase
{
    private readonly ILogger logger;
    private readonly SpotifyClient client;

    public SpotifySearchUseCase(ILogger logger, SpotifyClient client)
    {
        this.logger = logger;
        this.client = client;
    }

    public async Task<List<string>> GetSongIds(List<ArtistSong> songs, int maxCount, ICollection<string> ignoredGenres)
    {
        maxCount = maxCount == 0 ? songs.Count : maxCount;

        Dictionary<string, FullArtistObject> artists = new();
        List<string> songIds = new();
        foreach (ArtistSong song in songs)
        {
            string search = song.artist + " " + song.song;
            var track = await client.GetFirstTrackResult(search);
            if (track == null)
            {
                logger.Log($"No tracks found ({search})");
                continue;
            }

            var artistId = track.artists.First().id;
            if (!artists.ContainsKey(artistId))
            {
                artists.Add(artistId, await client.GetArtist(artistId));
            }
            var artist = artists[artistId];
            var genres = artist.genres;
            var ignoreDueToGenre = ignoredGenres.Count > 0 && genres.Any(g => ignoredGenres.Any(gg => g.ToLower().Contains(gg.ToLower())));
            if (ignoreDueToGenre)
            {
                logger.Log($"Track ignored due to genre ({search}) ({genres.ConcatenateListOfStringsToCommaAndSpaceString()})");
                continue;
            }


            songIds.Add(track.id);
            logger.Log($"Retrieved {songIds.Count} out of {maxCount} song ids ({search})");
            if (songIds.Count >= maxCount)
            {
                return songIds;
            }
        }
        return songIds;
    }
}