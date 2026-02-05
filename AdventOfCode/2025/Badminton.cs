using System.Text.Json;
using Xunit.Abstractions;

namespace AdventOfCode._2025;

public class Badminton(ITestOutputHelper testOutputHelper)
{
    private static readonly string[] Names =
    [
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
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
    public void MatchupsAsExpectedWith5Players1Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(5).ToArray(), true, 1, 1);
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Alfa", "Echo"), new Pairing("Bravo", "Charlie"))
                    ]
                }
            },
            matchups
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith5Players2Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(5).ToArray(), true, 2, 1);
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Alfa", "Echo"), new Pairing("Bravo", "Charlie")),
                        new Matchup(new Pairing("Delta", "Echo"), new Pairing("Alfa", "Charlie"))
                    ]
                }
            },
            matchups
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith5Players3Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(5).ToArray(), true, 3, 1);
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Alfa", "Echo"), new Pairing("Bravo", "Charlie")),
                        new Matchup(new Pairing("Delta", "Echo"), new Pairing("Alfa", "Charlie")),
                        new Matchup(new Pairing("Bravo", "Delta"), new Pairing("Charlie", "Echo"))
                    ]
                }
            },
            matchups
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith5Players5Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(5).ToArray(), true, 5, 1);
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Alfa", "Echo"), new Pairing("Bravo", "Charlie")),
                        new Matchup(new Pairing("Delta", "Echo"), new Pairing("Alfa", "Charlie")),
                        new Matchup(new Pairing("Alfa", "Delta"), new Pairing("Bravo", "Echo")),
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Echo")),
                        new Matchup(new Pairing("Bravo", "Delta"), new Pairing("Charlie", "Echo")),
                        new Matchup(new Pairing("Delta", "Echo"), new Pairing("Alfa", "Charlie"))
                    ]
                }
            },
            matchups
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
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Echo", "Foxtrot"), new Pairing("Alfa", "Golf")),
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Delta", "Echo")),
                        new Matchup(new Pairing("Foxtrot", "Golf"), new Pairing("Alfa", "Charlie")),
                        new Matchup(new Pairing("Delta", "Foxtrot"), new Pairing("Bravo", "Golf")),
                        new Matchup(new Pairing("Charlie", "Echo"), new Pairing("Alfa", "Foxtrot")),
                        new Matchup(new Pairing("Bravo", "Delta"), new Pairing("Echo", "Golf"))
                    ]
                },
                {
                    2,
                    [
                        new Matchup(new Pairing("Hotel", "India"), new Pairing("Juliett", "Kilo")),
                        new Matchup(new Pairing("Lima", "Mike"), new Pairing("Hotel", "November")),
                        new Matchup(new Pairing("India", "Juliett"), new Pairing("Kilo", "Lima")),
                        new Matchup(new Pairing("Mike", "November"), new Pairing("Hotel", "Juliett")),
                        new Matchup(new Pairing("Kilo", "Mike"), new Pairing("India", "November")),
                        new Matchup(new Pairing("Juliett", "Lima"), new Pairing("Hotel", "Mike")),
                        new Matchup(new Pairing("India", "Kilo"), new Pairing("Lima", "November"))
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
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Alfa", "Echo"), new Pairing("Bravo", "Charlie")),
                        new Matchup(new Pairing("Delta", "Echo"), new Pairing("Alfa", "Charlie")),
                        new Matchup(new Pairing("Alfa", "Delta"), new Pairing("Bravo", "Echo")),
                        new Matchup(new Pairing("Bravo", "Delta"), new Pairing("Charlie", "Echo"))
                    ]
                }
            },
            matchups
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith8Players4Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(8).ToArray(), true, 3, 1);
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
                        new Matchup(new Pairing("1", "2"), new Pairing("3", "4")),
                        new Matchup(new Pairing("5", "6"), new Pairing("7", "8")),
                        new Matchup(new Pairing("1", "3"), new Pairing("4", "5")),
                        new Matchup(new Pairing("6", "7"), new Pairing("1", "8")),
                        new Matchup(new Pairing("2", "3"), new Pairing("4", "6")),
                        new Matchup(new Pairing("2", "7"), new Pairing("5", "8"))
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
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Echo", "Foxtrot"), new Pairing("Alfa", "Charlie")),
                        new Matchup(new Pairing("Delta", "Echo"), new Pairing("Alfa", "Foxtrot")),
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Delta", "Foxtrot")),
                        new Matchup(new Pairing("Alfa", "Delta"), new Pairing("Bravo", "Echo")),
                        new Matchup(new Pairing("Charlie", "Echo"), new Pairing("Bravo", "Foxtrot"))
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
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Echo", "Foxtrot"), new Pairing("Alfa", "Golf")),
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Delta", "Echo")),
                        new Matchup(new Pairing("Foxtrot", "Golf"), new Pairing("Alfa", "Charlie")),
                        new Matchup(new Pairing("Delta", "Foxtrot"), new Pairing("Bravo", "Golf")),
                        new Matchup(new Pairing("Charlie", "Echo"), new Pairing("Alfa", "Foxtrot")),
                        new Matchup(new Pairing("Bravo", "Delta"), new Pairing("Echo", "Golf"))
                    ]
                }
            },
            matchups
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith4Players1Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(4).ToArray(), true, 1, 1);
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta"))
                    ]
                }
            },
            matchups
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith4Players2Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(4).ToArray(), true, 2, 1);
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Alfa", "Charlie"), new Pairing("Bravo", "Delta"))
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
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Alfa", "Delta"))
                    ]
                }
            },
            matchups
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith4Players4Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(4).ToArray(), true, 4, 1);
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Alfa", "Charlie"), new Pairing("Bravo", "Delta")),
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Alfa", "Delta")),
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Alfa", "Delta"))
                    ]
                }
            },
            matchups
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith4Players5Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(4).ToArray(), true, 5, 1);
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Alfa", "Charlie"), new Pairing("Bravo", "Delta")),
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Alfa", "Delta")),
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Alfa", "Delta")),
                        new Matchup(new Pairing("Bravo", "Delta"), new Pairing("Alfa", "Charlie"))
                    ]
                }
            },
            matchups
        );
    }

    [Fact]
    public void MatchupsAsExpectedWith4Players6Games1Courts()
    {
        var matchups = GetMatchup(Names.Take(4).ToArray(), true, 6, 1);
        Assert.Equal(
            new Dictionary<int, List<Matchup>>
            {
                {
                    1,
                    [
                        new Matchup(new Pairing("Alfa", "Bravo"), new Pairing("Charlie", "Delta")),
                        new Matchup(new Pairing("Alfa", "Charlie"), new Pairing("Bravo", "Delta")),
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Alfa", "Delta")),
                        new Matchup(new Pairing("Bravo", "Charlie"), new Pairing("Alfa", "Delta")),
                        new Matchup(new Pairing("Bravo", "Delta"), new Pairing("Alfa", "Charlie")),
                        new Matchup(new Pairing("Charlie", "Delta"), new Pairing("Alfa", "Bravo"))
                    ]
                }
            },
            matchups
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
                testOutputHelper.WriteLine("Player count is " + currentNames.Count);

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
                    nameChunkList.All(n => currentNames.Count(nn => nn == n) >= minGames))
                {
                    break;
                }
            }
            resultMap.Add(courtIndex++, currentMatchups);
        }
        return resultMap;
    }

    private static void ShufflePairs<T>(IList<T> pairsList)
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

    private record Pairing(string Player1, string Player2);

    private record Matchup(Pairing Pairing1, Pairing Pairing2);
}