using System.Runtime.CompilerServices;
using System.Text.Json;
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
        var pairsList = CreatePairs(Names.Take(3).ToArray());
        Assert.Equal(
            new List<Pairing>
            {
                new("Alfa", "Bravo"),
                new("Alfa", "Charlie"),
                new("Bravo", "Charlie")
            },
            pairsList
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith14Players4Games2Courts()
    {
        var matchups = GetMatchup(Names.Take(14).ToArray(), true, 4, 2);
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Delta", "Echo")),
                        new Matchup(new Pairing("Foxtrot", "Alfa"), new Pairing("Golf", "Delta")),
                        new Matchup(new Pairing("Bravo", "Echo"), new Pairing("Foxtrot", "Alfa")),
                        new Matchup(new Pairing("Charlie", "Delta"), new Pairing("Golf", "Alfa")),
                        new Matchup(new Pairing("Charlie", "Echo"), new Pairing("Foxtrot", "Bravo")),
                        new Matchup(new Pairing("Golf", "Alfa"), new Pairing("Bravo", "Delta")),
                        new Matchup(new Pairing("Foxtrot", "Charlie"), new Pairing("Golf", "Echo"))
                    ]
                },
                {
                    2,
                    [
                        new Matchup(new Pairing("India", "Juliett"), new Pairing("Kilo", "Lima")),
                        new Matchup(new Pairing("Mike", "Hotel"), new Pairing("November", "Kilo")),
                        new Matchup(new Pairing("India", "Lima"), new Pairing("Mike", "Hotel")),
                        new Matchup(new Pairing("Juliett", "Kilo"), new Pairing("November", "Hotel")),
                        new Matchup(new Pairing("Juliett", "Lima"), new Pairing("Mike", "India")),
                        new Matchup(new Pairing("November", "Hotel"), new Pairing("India", "Kilo")),
                        new Matchup(new Pairing("Mike", "Juliett"), new Pairing("November", "Lima"))
                    ]
                }
            },
            matchups
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith4Players3Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(4).ToArray(), true, 3, 1);
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Alfa", "Charlie"), new Pairing("Bravo", "Delta")),
                        new Matchup(new Pairing("Alfa", "Delta"), new Pairing("Bravo", "Charlie"))
                    ]
                }
            },
            matchups
        );
    }


    [Fact]
    public void MatchupsAsExpectedWith5Players4Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(5).ToArray(), true, 4, 1);
        File.WriteAllText(
            "C:\\Users\\simme\\AppData\\Roaming\\JetBrains\\Rider2025.3\\scratches\\scratch_18.json",
            JsonSerializer.Serialize(matchups)
        );
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Alfa", "Charlie"), new Pairing("Bravo", "Delta")),
                        new Matchup(new Pairing("Alfa", "Delta"), new Pairing("Bravo", "Charlie")),
                        new Matchup(new Pairing("Alfa", "Echo"), new Pairing("Bravo", "Charlie")),
                        new Matchup(new Pairing("Delta", "Echo"), new Pairing("Echo", "Echo"))
                    ]
                }
            },
            matchups
        );
    }


    [Fact]
    public void MatchupsAsExpectedWith6Players4Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(6).ToArray(), true, 4, 1);
        File.WriteAllText(
            "C:\\Users\\simme\\AppData\\Roaming\\JetBrains\\Rider2025.3\\scratches\\scratch_18.json",
            JsonSerializer.Serialize(matchups)
        );
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Echo", "Foxtrot"), new Pairing("Alfa", "Charlie")),
                        new Matchup(new Pairing("Bravo", "Delta"), new Pairing("Echo", "Foxtrot")),
                        new Matchup(new Pairing("Alfa", "Delta"), new Pairing("Bravo", "Charlie")),
                        new Matchup(new Pairing("Echo", "Foxtrot"), new Pairing("Alfa", "Echo")),
                        new Matchup(new Pairing("Bravo", "Foxtrot"), new Pairing("Charlie", "Delta"))
                    ]
                }
            },
            matchups
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith7Players4Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(7).ToArray(), true, 4, 1);
        Assert.Equal(
            matchups,
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Delta", "Echo")),
                        new Matchup(new Pairing("Foxtrot", "Alfa"), new Pairing("Golf", "Delta")),
                        new Matchup(new Pairing("Bravo", "Echo"), new Pairing("Foxtrot", "Alfa")),
                        new Matchup(new Pairing("Charlie", "Delta"), new Pairing("Golf", "Alfa")),
                        new Matchup(new Pairing("Charlie", "Echo"), new Pairing("Foxtrot", "Bravo")),
                        new Matchup(new Pairing("Golf", "Alfa"), new Pairing("Bravo", "Delta")),
                        new Matchup(new Pairing("Foxtrot", "Charlie"), new Pairing("Golf", "Echo"))
                    ]
                }
            }
        );
    }

    private IReadOnlyDictionary<int, List<Matchup>> GetMatchup(
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
            var currentNames = new List<string>();
            var currentPairs = new List<Pairing>();
            var currentMatchups = new List<Matchup>();

            Pairing GetPairing(string p1, string p2)
            {
                var sorted = new[] { p1, p2 }.OrderBy(p => p).ToList();
                return new Pairing(sorted[0], sorted[1]);
            }

            while (true)
            {
                var iOrderedEnumerable = nameChunk
                    .OrderBy(n => currentNames.TakeLast(currentNames.Count % 4).Contains(n) ? int.MaxValue : 0)
                    .ThenBy(n => currentNames.Count(nn => nn == n) < minGames);
                
                var orderedEnumerable = iOrderedEnumerable
                    .ThenBy(n =>
                        {
                            if (currentNames.Count % 2 == 0)
                            {
                                return 0;
                            }
                            var count = currentPairs.Count(p =>
                                {
                                    var pairing = GetPairing(currentNames[^1], n);
                                    var b = pairing == p;
                                    return b;
                                }
                            );
                            return count;
                        }
                    );
                
                var pick = orderedEnumerable.First();
                testOutputHelper.WriteLine("Picked " + pick);
                currentNames.Add(pick);
                
                if (currentNames.Count != 0 && currentNames.Count % 2 == 0)
                {
                    var lastTwoNames = currentNames.TakeLast(2).ToList();
                    var pairing = GetPairing(lastTwoNames[0], lastTwoNames[1]);
                    testOutputHelper.WriteLine("Paired " + pairing);
                    currentPairs.Add(pairing);
                    if (currentPairs.Count % 2 == 0)
                    {
                        var lastTwoPairs = currentPairs.TakeLast(2).ToList();
                        var matchup = new Matchup(lastTwoPairs[0], lastTwoPairs[1]);
                        testOutputHelper.WriteLine("Matched up " + matchup);
                        currentMatchups.Add(matchup);
                    }
                }
                if (currentNames.Count % 4 == 0 &&
                    nameChunk.All(n => currentNames.Count(nn => nn == n) >= minGames))
                {
                    break;
                }
            }
            resultMap.Add(courtIndex++, currentMatchups);
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