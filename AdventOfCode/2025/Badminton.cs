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
            expected: new List<Pairing>
            {
                new(Player1: Names[0], Player2: Names[1]),
                new(Player1: Names[0], Player2: Names[2]),
                new(Player1: Names[0], Player2: Names[3]),
                new(Player1: Names[0], Player2: Names[4]),
                new(Player1: Names[0], Player2: Names[5]),
                new(Player1: Names[0], Player2: Names[6]),
                new(Player1: Names[0], Player2: Names[7]),
                new(Player1: Names[0], Player2: Names[8]),
                new(Player1: Names[0], Player2: Names[9]),
                new(Player1: Names[0], Player2: Names[10]),
                new(Player1: Names[1], Player2: Names[2]),
                new(Player1: Names[1], Player2: Names[3]),
                new(Player1: Names[1], Player2: Names[4]),
                new(Player1: Names[1], Player2: Names[5]),
                new(Player1: Names[1], Player2: Names[6]),
                new(Player1: Names[1], Player2: Names[7]),
                new(Player1: Names[1], Player2: Names[8]),
                new(Player1: Names[1], Player2: Names[9]),
                new(Player1: Names[1], Player2: Names[10]),
                new(Player1: Names[2], Player2: Names[3]),
                new(Player1: Names[2], Player2: Names[4]),
                new(Player1: Names[2], Player2: Names[5]),
                new(Player1: Names[2], Player2: Names[6]),
                new(Player1: Names[2], Player2: Names[7]),
                new(Player1: Names[2], Player2: Names[8]),
                new(Player1: Names[2], Player2: Names[9]),
                new(Player1: Names[2], Player2: Names[10]),
                new(Player1: Names[3], Player2: Names[4]),
                new(Player1: Names[3], Player2: Names[5]),
                new(Player1: Names[3], Player2: Names[6]),
                new(Player1: Names[3], Player2: Names[7]),
                new(Player1: Names[3], Player2: Names[8]),
                new(Player1: Names[3], Player2: Names[9]),
                new(Player1: Names[3], Player2: Names[10]),
                new(Player1: Names[4], Player2: Names[5]),
                new(Player1: Names[4], Player2: Names[6]),
                new(Player1: Names[4], Player2: Names[7]),
                new(Player1: Names[4], Player2: Names[8]),
                new(Player1: Names[4], Player2: Names[9]),
                new(Player1: Names[4], Player2: Names[10]),
                new(Player1: Names[5], Player2: Names[6]),
                new(Player1: Names[5], Player2: Names[7]),
                new(Player1: Names[5], Player2: Names[8]),
                new(Player1: Names[5], Player2: Names[9]),
                new(Player1: Names[5], Player2: Names[10]),
                new(Player1: Names[6], Player2: Names[7]),
                new(Player1: Names[6], Player2: Names[8]),
                new(Player1: Names[6], Player2: Names[9]),
                new(Player1: Names[6], Player2: Names[10]),
                new(Player1: Names[7], Player2: Names[8]),
                new(Player1: Names[7], Player2: Names[9]),
                new(Player1: Names[7], Player2: Names[10]),
                new(Player1: Names[8], Player2: Names[9]),
                new(Player1: Names[8], Player2: Names[10]),
                new(Player1: Names[9], Player2: Names[10])
            },
            actual: pairsList
        );
    }

    [Fact]
    public void MatchupMakingAsExpected()
    {
        var matchups = GetMatchup(names: Names.Take(14).ToArray(), shuffle: true, minGames: 2, courtCount: 2);
        var matchupsPrint = GetPrintedMatchups(matchups);
        File.WriteAllText(
            path: "C:\\Users\\simme\\AppData\\Roaming\\JetBrains\\Rider2025.3\\scratches\\matchups.txt",
            // JsonSerializer.Serialize(matchups, new JsonSerializerOptions { WriteIndented = true })
            contents: matchupsPrint
        );
        Assert.Equal(
            expected: matchups,
            actual: new Dictionary<int, List<Matchup>>
            {
                {
                    1, [
                        new Matchup(
                            Pairing1: new Pairing(Player1: Names[0], Player2: Names[1]),
                            Pairing2: new Pairing(Player1: Names[2], Player2: Names[3])
                        ),
                        new Matchup(
                            Pairing1: new Pairing(Player1: Names[4], Player2: Names[5]),
                            Pairing2: new Pairing(Player1: Names[6], Player2: Names[7])
                        ),
                        new Matchup(
                            Pairing1: new Pairing(Player1: Names[8], Player2: Names[9]),
                            Pairing2: new Pairing(Player1: Names[0], Player2: Names[10])
                        ),
                        new Matchup(
                            Pairing1: new Pairing(Player1: Names[1], Player2: Names[2]),
                            Pairing2: new Pairing(Player1: Names[3], Player2: Names[4])
                        ),
                        new Matchup(
                            Pairing1: new Pairing(Player1: Names[5], Player2: Names[6]),
                            Pairing2: new Pairing(Player1: Names[7], Player2: Names[8])
                        ),
                        new Matchup(
                            Pairing1: new Pairing(Player1: Names[9], Player2: Names[10]),
                            Pairing2: new Pairing(Player1: Names[0], Player2: Names[2])
                        )
                    ]
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

            int GetPlayerWillPlayCount(string name)
            {
                return pairsToPlay.Count(p => p.ContainsPlayer(name));
            }


            foreach (var name in nameChunk)
            {
                var namePairs = pairsList
                    .Where(p =>
                        {
                            var pairPlayers = p.GetPlayers().ToList();
                            return !pairsToPlay.Contains(p) && pairPlayers.Contains(name) &&
                                pairPlayers.All(pName => GetPlayerWillPlayCount(pName) != minGames);
                        }
                    )
                    .Take(minGames - GetPlayerWillPlayCount(name))
                    .ToList();
                pairsToPlay.AddRange(namePairs);
            }

            pairsToPlay = pairsToPlay.Where(p => p.GetPlayers()
                    .Select(GetPlayerWillPlayCount)
                    .Sum() == minGames * p.GetPlayers().Count()
            ).ToList();
            var needingMoreGames = nameChunk.Where(n => GetPlayerWillPlayCount(n) == 0);
            foreach (var name in needingMoreGames)
            {
                // ShufflePairs(pairsToPlay);
                while (GetPlayerWillPlayCount(name) < minGames)
                {
                    var firstPair = pairsToPlay[0];
                    pairsToPlay.RemoveAt(0);
                    pairsToPlay.AddRange(
                        [
                            new Pairing(Player1: name, Player2: firstPair.Player1),
                            new Pairing(Player1: name, Player2: firstPair.Player2)
                        ]
                    );
                }
            }

            while (pairsToPlay.Any())
            {
                var ordered = pairsToPlay
                    .OrderByDescending(p =>
                        p.GetPlayers()
                            .Select(GetPlayerWillPlayCount)
                            .Sum()
                    ).ToList();
                var firstPair = ordered[0];
                var secondPair = ordered.First(p =>
                    p.GetPlayers().Concat(firstPair.GetPlayers()).Distinct().Count() == 4
                );
                result.Add(new Matchup(Pairing1: firstPair, Pairing2: secondPair));
                pairsToPlay.Remove(firstPair);
                pairsToPlay.Remove(secondPair);
            }
            resultMap.Add(key: courtIndex++, value: result);
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
                    pairsSet.Add(new Pairing(Player1: sorted[0], Player2: sorted[1]));
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
            allLines.Add(string.Join(separator: "\n", values: lines));
        }

        return string.Join(separator: "\n\n", values: allLines);
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

    private record Matchup(Pairing Pairing1, Pairing Pairing2);
}