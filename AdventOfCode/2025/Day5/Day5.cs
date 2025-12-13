using Xunit.Abstractions;
using static AdventOfCode._2025.Utils;

namespace AdventOfCode._2025.Day5;

public class Day5(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Part1()
    {
        Assert.Equal(3, Get(Day5Input.Example, testOutputHelper));
        Assert.Equal(652, Get(Day5Input.Input, new MutedTestOutputHelper()));
    }

    // [Fact]
    // public void Part2()
    // {
    //     Assert.Equal(3121910778619, Get(Day5Input.Example, 12, testOutputHelper));
    //     Assert.Equal(172787336861064, Get(Day5Input.Input, 12, new MutedTestOutputHelper()));
    // }

    private long Get(string input, ITestOutputHelper logger)
    {
        var lines = GetInputLines(input);
        var ranges = new List<(long Left, long Right)>();
        var ids = new List<long>();
        var total = 0;
        foreach (var line in lines)
        {
            if (line.Contains('-'))
            {
                var split = line.Split("-", StringSplitOptions.RemoveEmptyEntries);
                ranges.Add((long.Parse(split[0]), long.Parse(split[1])));
            } else if (line.Length > 0)
            {
                ids.Add(long.Parse(line));
            }
        }

        foreach (var id in ids)
        {
            foreach (var range in ranges)
            {
                if (id >= range.Left && id <= range.Right)
                {
                    total++;
                    break;
                }
            }
        }
        return total;
    }
}