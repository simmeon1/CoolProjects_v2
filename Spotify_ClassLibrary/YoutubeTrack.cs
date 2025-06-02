using System.Diagnostics;

namespace Spotify_ClassLibrary;

[DebuggerDisplay("{name} - {views}")]
public record YoutubeTrack(string id, string name, long views);