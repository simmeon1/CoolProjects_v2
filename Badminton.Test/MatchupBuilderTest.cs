using Badminton.Core;
using Xunit.Abstractions;

namespace Badminton.Test;

public class MatchupBuilderTest(ITestOutputHelper testOutputHelper)
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
    
    private class Logger(ITestOutputHelper testOutputHelper) : ILogger
    {
        public void WriteLine(string str)
        {
            testOutputHelper.WriteLine(str);
        }
    }

    private readonly MatchupBuilder builder = new(new Logger(testOutputHelper));

    [Fact]
    public void MatchupsAsExpectedWith5Players1Games1Courts()
    {
        var matchups = builder.GetMatchup(Names.Take(5).ToArray(), 1, 1);
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
        var matchups = builder.GetMatchup(Names.Take(5).ToArray(), 2, 1);
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
        var matchups = builder.GetMatchup(Names.Take(5).ToArray(), 3, 1);
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
        var matchups = builder.GetMatchup(Names.Take(5).ToArray(), 5, 1);
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
        var matchups = builder.GetMatchup(Names.Take(14).ToArray(), 4, 2);
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
        var matchups = builder.GetMatchup(Names.Take(5).ToArray(), 4, 1);
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
    public void MatchupsAsExpectedWith6Players4Games1Courts()
    {
        var matchups = builder.GetMatchup(Names.Take(6).ToArray(), 4, 1);
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
        var matchups = builder.GetMatchup(Names.Take(7).ToArray(), 4, 1);
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
        var matchups = builder.GetMatchup(Names.Take(4).ToArray(), 1, 1);
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
        var matchups = builder.GetMatchup(Names.Take(4).ToArray(), 2, 1);
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
        var matchups = builder.GetMatchup(Names.Take(4).ToArray(), 3, 1);
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
        var matchups = builder.GetMatchup(Names.Take(4).ToArray(), 4, 1);
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
        var matchups = builder.GetMatchup(Names.Take(4).ToArray(), 5, 1);
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
        var matchups = builder.GetMatchup(Names.Take(4).ToArray(), 6, 1);
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
}