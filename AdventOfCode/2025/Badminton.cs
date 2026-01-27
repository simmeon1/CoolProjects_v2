using Xunit.Abstractions;

namespace AdventOfCode._2025;

public class Badminton(ITestOutputHelper testOutputHelper)
{
    private static readonly string[] Names =
    [
        "Alfa",
        "Bravo",
        "Charlie",
        "Delta",
        "Echo",
        "Foxtrot",
        "Golf",
        "Hotel",
        "India",
        "Juliett",
        "Kilo",
        "Lima",
        "Mike",
        "November",
        "Oscar",
        "Papa",
        "Quebec",
        "Romeo",
        "Sierra",
        "Tango",
        "Uniform",
        "Victor",
        "Whiskey",
        "Xray",
        "Yankee",
        "Zulu"
    ];

    [Fact]
    public void PairingsAreCorrect()
    {
        var pairsList = CreatePairs(Names);
        Assert.Equal(
            new List<Pairing>
            {
                new(Names[0], Names[1]),
                new(Names[0], Names[2]),
                new(Names[0], Names[3]),
                new(Names[0], Names[4]),
                new(Names[0], Names[5]),
                new(Names[0], Names[6]),
                new(Names[0], Names[7]),
                new(Names[0], Names[8]),
                new(Names[0], Names[9]),
                new(Names[0], Names[10]),
                new(Names[1], Names[2]),
                new(Names[1], Names[3]),
                new(Names[1], Names[4]),
                new(Names[1], Names[5]),
                new(Names[1], Names[6]),
                new(Names[1], Names[7]),
                new(Names[1], Names[8]),
                new(Names[1], Names[9]),
                new(Names[1], Names[10]),
                new(Names[2], Names[3]),
                new(Names[2], Names[4]),
                new(Names[2], Names[5]),
                new(Names[2], Names[6]),
                new(Names[2], Names[7]),
                new(Names[2], Names[8]),
                new(Names[2], Names[9]),
                new(Names[2], Names[10]),
                new(Names[3], Names[4]),
                new(Names[3], Names[5]),
                new(Names[3], Names[6]),
                new(Names[3], Names[7]),
                new(Names[3], Names[8]),
                new(Names[3], Names[9]),
                new(Names[3], Names[10]),
                new(Names[4], Names[5]),
                new(Names[4], Names[6]),
                new(Names[4], Names[7]),
                new(Names[4], Names[8]),
                new(Names[4], Names[9]),
                new(Names[4], Names[10]),
                new(Names[5], Names[6]),
                new(Names[5], Names[7]),
                new(Names[5], Names[8]),
                new(Names[5], Names[9]),
                new(Names[5], Names[10]),
                new(Names[6], Names[7]),
                new(Names[6], Names[8]),
                new(Names[6], Names[9]),
                new(Names[6], Names[10]),
                new(Names[7], Names[8]),
                new(Names[7], Names[9]),
                new(Names[7], Names[10]),
                new(Names[8], Names[9]),
                new(Names[8], Names[10]),
                new(Names[9], Names[10])
            },
            pairsList
        );
    }

    [Fact]
    public void MatchupMakingAsExpected()
    {
        var matchups = GetMatchup(Names.Take(14).ToArray(), true, 2, 2);
        var matchupsPrint = GetPrintedMatchups(matchups);
        File.WriteAllText(
            "C:\\Users\\simme\\AppData\\Roaming\\JetBrains\\Rider2025.3\\scratches\\matchups.txt",
            // JsonSerializer.Serialize(matchups, new JsonSerializerOptions { WriteIndented = true })
            matchupsPrint
        );
        Assert.Equal(
            matchups,
            new Dictionary<int, List<Matchup>>
            {
                {
                    1, new List<Matchup>
                    {
                        new(new Pairing(Names[0], Names[1]), new Pairing(Names[2], Names[3])),
                        new(new Pairing(Names[4], Names[5]), new Pairing(Names[6], Names[7])),
                        new(new Pairing(Names[8], Names[9]), new Pairing(Names[0], Names[10])),
                        new(new Pairing(Names[1], Names[2]), new Pairing(Names[3], Names[4])),
                        new(new Pairing(Names[5], Names[6]), new Pairing(Names[7], Names[8])),
                        new(new Pairing(Names[9], Names[10]), new Pairing(Names[0], Names[2]))
                    }
                }
            }
        );
    }

    private static IReadOnlyDictionary<int, List<Matchup>> GetMatchup(
        string[] names,
        bool shuffle,
        int minGames,
        int courtCount
    )
    {
        var ceiling = Math.Ceiling(names.Length / Convert.ToDouble(courtCount));
        var nameChunks = names.Chunk(Convert.ToInt32(ceiling));
        var resultMap = new Dictionary<int, List<Matchup>>();

        var courtIndex = 1;
        foreach (var nameChunk in nameChunks)
        {
            var result = new List<Matchup>();
            var pairsList = CreatePairs(nameChunk).ToList();

            var pairsToPlay = new List<Pairing>();

            int GetPlayerWillPlayCount(string name) => pairsToPlay.Count(p => p.ContainsPlayer(name));
            int GetPlayersPlayCountSum(Pairing p) => p.GetPlayers().Select(GetPlayerWillPlayCount).Sum();

            foreach (var name in nameChunk)
            {
                var namePairs = pairsList
                    .Where(p =>
                        {
                            var pairPlayers = p.GetPlayers().ToList();
                            return !pairsToPlay.Contains(p) && pairPlayers.Contains(name) &&
                                pairPlayers.All(pName => GetPlayerWillPlayCount(pName) < minGames);
                        }
                    )
                    .Take(minGames - GetPlayerWillPlayCount(name))
                    .ToList();
                pairsToPlay.AddRange(namePairs);
            }

            pairsToPlay = pairsToPlay.Where(p => GetPlayersPlayCountSum(p) >= minGames * 2).ToList();
            var needingMoreGames = nameChunk.Where(n => GetPlayerWillPlayCount(n) == 0);
            foreach (var name in needingMoreGames)
            {
                // ShufflePairs(pairsToPlay);
                while (GetPlayerWillPlayCount(name) < minGames)
                {
                    var pairToSeparate = pairsToPlay.MaxBy(GetPlayersPlayCountSum)!;
                    pairsToPlay.Remove(pairToSeparate);
                    pairsToPlay.AddRange(
                        [
                            new Pairing(name, pairToSeparate.Player1),
                            new Pairing(name, pairToSeparate.Player2)
                        ]
                    );
                }
            }

            while (pairsToPlay.Any())
            {
                Pairing? FindUniquePairing(IEnumerable<Pairing> allPairing, Pairing firstPair) =>
                    allPairing.FirstOrDefault(p =>
                        p.GetPlayers().Concat(firstPair.GetPlayers()).Distinct().Count() == 4
                    );

                var ordered = pairsToPlay.OrderByDescending(GetPlayersPlayCountSum).ToList();
                var firstPair = ordered[0];
                var secondPair = FindUniquePairing(ordered, firstPair) ?? FindUniquePairing(
                    result.SelectMany(m => m.GetPairings()),
                    firstPair
                )!;
                result.Add(new Matchup(firstPair, secondPair));
                pairsToPlay.Remove(firstPair);
                pairsToPlay.Remove(secondPair);
            }
            resultMap.Add(courtIndex++, result);
        }
        return resultMap;
    }

    private static void ShufflePairs(List<Pairing> pairsList)
    {
        var rng = new Random();
        var n = pairsList.Count;
        while (n > 1)
        {
            n--;
            var k = rng.Next(n + 1);
            (pairsList[k], pairsList[n]) = (pairsList[n], pairsList[k]);
        }
    }

    private static Pairing[] CreatePairs(string[] names)
    {
        var pairsSet = new HashSet<Pairing>();
        for (var i = 0; i < names.Length; i++)
        {
            var player1 = names[i];
            for (var j = 0; j < names.Length; j++)
            {
                var player2 = names[j];
                if (player1 != player2)
                {
                    var sorted = new[] { player1, player2 }.Order().ToArray();
                    pairsSet.Add(new Pairing(sorted[0], sorted[1]));
                }
            }
        }
        return pairsSet.OrderBy(p => p.Player1).ThenBy(p => p.Player2).ToArray();
    }

    private string GetPrintedMatchups(IReadOnlyDictionary<int, List<Matchup>> matchupsMap)
    {
        var allLines = new List<string>();
        foreach (var (courtIndex, matchups) in matchupsMap)
        {
            var lines = new List<string> { $"Court {courtIndex}" };
            foreach (var matchup in matchups)
            {
                lines.Add(
                    $"{matchup.Pairing1.Player1}/{matchup.Pairing1.Player2} - {matchup.Pairing2.Player1}/{matchup.Pairing2.Player2}"
                );
            }
            allLines.Add(string.Join("\n", lines));
        }

        return string.Join("\n\n", allLines);
    }

    private record Pairing(string Player1, string Player2)
    {
        public IEnumerable<string> GetPlayers()
        {
            return [Player1, Player2];
        }

        public bool ContainsPlayer(string name)
        {
            return GetPlayers().Contains(name);
        }
    }

    private record Matchup(Pairing Pairing1, Pairing Pairing2)
    {
        public IEnumerable<Pairing> GetPairings()
        {
            return [Pairing1, Pairing2];
        }
    }
}