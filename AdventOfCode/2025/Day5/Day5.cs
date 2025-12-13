using Xunit.Abstractions;
using static AdventOfCode._2025.Utils;

namespace AdventOfCode._2025.Day5;

public class Day5(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Part1()
    {
        Assert.Equal(3, Get1(Day5Input.Example, testOutputHelper));
        Assert.Equal(652, Get1(Day5Input.Input, new MutedTestOutputHelper()));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(14, Get2(Day5Input.Example, testOutputHelper));
        Assert.Equal(341753674214273, Get2(Day5Input.Input, testOutputHelper));
    }

    private long Get1(string input, ITestOutputHelper logger)
    {
        var ranges = GetIdsAndRanges(input, out var ids, out var total);
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

    private long Get2(string input, ITestOutputHelper logger)
    {
        var ranges = GetIdsAndRanges(input, out var ids, out var total);
        ranges = ranges.OrderBy(r => r.Left).Distinct().ToList();
        while (ranges.Any())
        {
            var keepGoing = false;
            foreach (var range in ranges)
            {
                var overlapping =
                    ranges.FirstOrDefault(r => r != range && range.Left >= r.Left && range.Left <= r.Right);
                if (overlapping != (0, 0))
                {
                    var mergedRange = (Math.Min(range.Left, overlapping.Left),
                        Math.Max(range.Right, overlapping.Right));
                    ranges.Remove(range);
                    ranges.Remove(overlapping);
                    ranges.Add(mergedRange);
                    keepGoing = true;
                    logger.WriteLine("Made " + mergedRange + " out of " + range + " and " + overlapping);
                    break;
                }
            }

            if (!keepGoing)
            {
                break;
            }
        }
        return ranges.Select(r => r.Right - r.Left + 1).Sum();
    }


    private static List<(long Left, long Right)> GetIdsAndRanges(string input, out List<long> ids, out int total)
    {
        var lines = GetInputLines(input);
        var ranges = new List<(long Left, long Right)>();
        ids = new List<long>();
        total = 0;
        foreach (var line in lines)
        {
            if (line.Contains('-'))
            {
                var split = line.Split("-", StringSplitOptions.RemoveEmptyEntries);
                ranges.Add((long.Parse(split[0]), long.Parse(split[1])));
            }
            else if (line.Length > 0)
            {
                ids.Add(long.Parse(line));
            }
        }
        return ranges;
    }
}