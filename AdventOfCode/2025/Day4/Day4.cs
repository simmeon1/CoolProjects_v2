using Xunit.Abstractions;
using static AdventOfCode._2025.Utils;

namespace AdventOfCode._2025.Day4;

public class Day4(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Part1()
    {
        Assert.Equal(13, Get(Day4Input.Example, testOutputHelper));
        Assert.Equal(1540, Get(Day4Input.Input, new MutedTestOutputHelper()));
    }

    // [Fact]
    // public void Part2()
    // {
    //     Assert.Equal(3121910778619, Get(Day4Input.Example, 12, testOutputHelper));
    //     Assert.Equal(172787336861064, Get(Day4Input.Input, 12, new MutedTestOutputHelper()));
    // }

    private long Get(string input, ITestOutputHelper logger)
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
        var total = 0;
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == '.')
                {
                    continue;
                }
                
                var result = 0;
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
                        result += map[check.Row, check.Column] == '@' ? 1 : 0;
                    }
                    catch
                    {
                        // ignored
                    }
                    if (result > maxRolls)
                    {
                        break;
                    }
                }
                if (result <= maxRolls)
                {
                    total++;
                }
            }
        }
        return total;
    }
}