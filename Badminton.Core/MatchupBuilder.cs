namespace Badminton.Core;

public class MatchupBuilder(ILogger logger)
{
    public MatchupBuilder() : this(new Logger())
    { }

    public IReadOnlyDictionary<int, MatchupCollection> GetMatchups(
        IEnumerable<string> names,
        int minGames,
        int courtCount
    )
    {
        var namesList = names.Select(n => n.Trim()).Distinct().ToList();
        var ceiling = Math.Ceiling(namesList.Count / Convert.ToDouble(courtCount));
        var nameChunks = namesList.Chunk(Convert.ToInt32(ceiling));
        var resultMap = new Dictionary<int, MatchupCollection>();

        var courtIndex = 1;
        foreach (var nameChunk in nameChunks)
        {
            var nameChunkList = nameChunk.ToList();
            var currentNames = new List<string>();
            var currentPairs = new List<Pairing>();
            var currentMatchups = new List<Matchup>();

            Pairing GetPairing(string p1, string p2)
            {
                var sorted = new[] { p1, p2 }.OrderBy(p => p).ToList();
                return new Pairing(sorted[0], sorted[1]);
            }

            var queue = new List<string>(nameChunkList);
            while (true)
            {
                logger.WriteLine($"Player count is {currentNames.Count}");

                var pick = queue
                    .OrderBy(n => currentNames.TakeLast(currentNames.Count % 4).Contains(n) ? int.MaxValue : 0)
                    .ThenBy(n => currentNames.Count(nn => nn == n) >= minGames)
                    .ThenBy(n =>
                        currentNames.Count % 2 == 0 ? 0 : currentPairs.Count(p => GetPairing(currentNames[^1], n) == p)
                    ).First();
                var indexOf = nameChunkList.IndexOf(pick);
                var nextIndex = indexOf == nameChunkList.Count - 1 ? 0 : indexOf + 1;
                queue = nameChunkList
                    .Slice(nextIndex, nameChunkList.Count - nextIndex)
                    .Concat(nameChunkList[..nextIndex])
                    .ToList();
                logger.WriteLine($"Picked {pick}");
                currentNames.Add(pick);

                if (currentNames.Count != 0 && currentNames.Count % 2 == 0)
                {
                    var lastTwoNames = currentNames.TakeLast(2).ToList();
                    var pairing = GetPairing(lastTwoNames[0], lastTwoNames[1]);
                    logger.WriteLine($"Paired {pairing}");
                    currentPairs.Add(pairing);
                    if (currentPairs.Count % 2 == 0)
                    {
                        var lastTwoPairs = currentPairs.TakeLast(2).ToList();
                        var matchup = new Matchup(lastTwoPairs[0], lastTwoPairs[1]);
                        logger.WriteLine($"Matched up {matchup}");
                        currentMatchups.Add(matchup);
                    }
                }
                if (currentNames.Count % 4 == 0 &&
                    nameChunkList.All(n => currentNames.Count(nn => nn == n) >= minGames))
                {
                    break;
                }
            }
            resultMap.Add(
                courtIndex++,
                new MatchupCollection(nameChunk.Index().ToDictionary(x => x.Index, x => x.Item), currentMatchups)
            );
        }
        return resultMap;
    }

    private class Logger : ILogger
    {
        public void WriteLine(string str)
        { }
    }
}

public interface ILogger
{
    void WriteLine(string str);
}

public record Pairing(string Player1, string Player2);

public record Matchup(Pairing Pairing1, Pairing Pairing2);

public record MatchupCollection(
    IReadOnlyDictionary<int, string> Players,
    IEnumerable<Matchup> Matchups
)
{ }