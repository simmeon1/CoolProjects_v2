using Xunit.Abstractions;
using static AdventOfCode._2025.Utils;

namespace AdventOfCode._2025.Day4;

public class Day4(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Part1()
    {
        Assert.Equal(13, Get(Day4Input.Example, false, testOutputHelper));
        Assert.Equal(1540, Get(Day4Input.Input, false, new MutedTestOutputHelper()));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(43, Get(Day4Input.Example, true, testOutputHelper));
        Assert.Equal(8972, Get(Day4Input.Input, true, new MutedTestOutputHelper()));
    }

    private long Get(string input, bool repeat, ITestOutputHelper logger)
    {
        var lines = GetInputLines(input);
        var map = new char[lines.Length, lines.First().Length];
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            for (var j = 0; j < line.Length; j++)
            {
                var ch = lines[i][j];
                map[i,j] = ch;
            }
        }

        var maxRolls = 3;
        var total = new List<(int Row, int Column)>();

        while (true)
        {
            var currentTotal = new List<(int Row, int Column)>();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == '.')
                    {
                        continue;
                    }
                    
                    var result = new List<(int Row, int Column)>();
                    var checks = new (int Row, int Column)[]
                    {
                        (i - 1, j - 1),
                        (i - 1, j),
                        (i - 1, j + 1),
                        (i + 1, j - 1),
                        (i + 1, j),
                        (i + 1, j + 1),
                        (i, j - 1),
                        (i, j + 1),
                    };
                    foreach (var check in checks)
                    {
                        try
                        {
                            if (map[check.Row, check.Column] == '@')
                            {
                                result.Add((check.Row, check.Column));
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                        if (result.Count > maxRolls)
                        {
                            break;
                        }
                    }
                    if (result.Count <= maxRolls)
                    {
                        currentTotal.Add((i,j));
                        total.Add((i,j));
                    }
                }
            }

            if (!repeat || currentTotal.Count == 0)
            {
                break;
            }

            foreach (var c in currentTotal)
            {
                map[c.Row, c.Column] = '.';
            }
            
        }
        return total.Count;
    }
}