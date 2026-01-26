using Xunit.Abstractions;

namespace AdventOfCode._2025;

public class Badminton(ITestOutputHelper testOutputHelper)
{
    private static readonly string[] Names =
    [
        "aaa",
        "bbb",
        "ccc",
        "ddd",
        "eee",
        "fff",
        "ggg",
        "hhh",
        "iii",
        "jjj",
        "kkk"
    ];
    
    [Fact]
    public void PairingsAreCorrect()
    {
        var pairsList = CreatePairs();
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
    public void ShufflingIsGood()
    {
        var pairsList = CreatePairs().ToList();
        var matchups = GetMatchups(pairsList: pairsList, shuffle: false, minGames: 2);
        Assert.Equal(
            expected: matchups,
            actual:
            [
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
        );
    }

    private static List<Matchup> GetMatchups(List<Pairing> pairsList, bool shuffle, int minGames)
    {
        var result = new List<Matchup>();
        var playerHasPlayedMap = pairsList.SelectMany(p =>
            new[] { p.Player1, p.Player2 }
        ).Distinct().ToDictionary(keySelector: p => p, elementSelector: _ => 0);
        var pairingsHavePlayedMap = pairsList.ToDictionary(keySelector: p => p, elementSelector: _ => 0);

        while (true)
        {
            if (shuffle)
            {
                ShufflePairs(pairsList);
            }
            var prioritizedPairGroups = pairsList
                .GroupBy(p => playerHasPlayedMap[p.Player1] + playerHasPlayedMap[p.Player2]
                )
                .OrderBy(g => g.Key);

            var matchup = CreateMatchup(
                prioritizedPairGroups: prioritizedPairGroups,
                pairingsHavePlayedMap: pairingsHavePlayedMap
            );

            var matchupPairings = new Matchup(Pairing1: matchup.Pairing1, Pairing2: matchup.Pairing2);
            result.Add(matchupPairings);

            foreach (var pairing in new[] { matchupPairings.Pairing1, matchupPairings.Pairing2 })
            {
                pairingsHavePlayedMap[pairing]++;
                foreach (var player in new[] { pairing.Player1, pairing.Player2 })
                {
                    playerHasPlayedMap[player]++;
                }
            }
            if (playerHasPlayedMap.Values.Min() >= minGames)
            {
                break;
            }
        }
        return result;
    }

    private static (Pairing Pairing1, Pairing Pairing2) CreateMatchup(
        IOrderedEnumerable<IGrouping<int, Pairing>> prioritizedPairGroups,
        Dictionary<Pairing, int> pairingsHavePlayedMap
    )
    {
        var matchup = new List<Pairing>();
        foreach (var prioritizedPairGroup in prioritizedPairGroups)
        {
            foreach (var pair in prioritizedPairGroup.OrderBy(p => pairingsHavePlayedMap[p]))
            {
                var matchupNames = matchup.SelectMany(p =>
                    new[] { p.Player1, p.Player2 }
                ).ToList();
                if (!matchupNames.Contains(pair.Player1) && !matchupNames.Contains(pair.Player2))
                {
                    matchup.Add(pair);
                    if (matchup.Count == 2)
                    {
                        return (matchup[0], matchup[1]);
                    }
                }
            }
        }
        return (matchup[0], matchup[1]);
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

    private static Pairing[] CreatePairs()
    {
        var pairsSet = new HashSet<Pairing>();
        for (var i = 0; i < Names.Length; i++)
        {
            var player1 = Names[i];
            for (var j = 0; j < Names.Length; j++)
            {
                var player2 = Names[j];
                if (player1 != player2)
                {
                    var sorted = new[] { player1, player2 }.Order().ToArray();
                    pairsSet.Add(new Pairing(Player1: sorted[0], Player2: sorted[1]));
                }
            }
        }
        return pairsSet.OrderBy(p => p.Player1).ThenBy(p => p.Player2).ToArray();
    }

    private record Pairing(string Player1, string Player2);

    private record Matchup(Pairing Pairing1, Pairing Pairing2);
}