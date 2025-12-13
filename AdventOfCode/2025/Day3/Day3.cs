using Xunit.Abstractions;
using static AdventOfCode._2025.Utils;

namespace AdventOfCode._2025.Day3;

public class Day3(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Part1()
    {
        Assert.Equal(357, Get(Day3Input.Example, 2, testOutputHelper));
        Assert.Equal(17359, Get(Day3Input.Input, 2, new MutedTestOutputHelper()));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(3121910778619, Get(Day3Input.Example, 12, testOutputHelper));
        Assert.Equal(172787336861064, Get(Day3Input.Input, 12, new MutedTestOutputHelper()));
    }

    private long Get(string input, int targetLength, ITestOutputHelper logger)
    {
        long result = 0;
        var lines = GetInputLines(input);
        foreach (var line in lines)
        {
            var arr = line.ToCharArray().Select(x => int.Parse(x.ToString())).ToArray();
            var pairsList = arr.Index();
            var ordered = pairsList
                .OrderByDescending(x => x.Item)
                .ThenBy(x => x.Item)
                .ToList();

            var num = new List<(int Index, int Number)>();
            for (var i = 0; i < ordered.Count; i++)
            {
                var el = ordered[i];
                var count = ordered.Count(x => x.Index > el.Index);
                if (count >= targetLength - num.Count - 1 && num.Select(x => x.Index).LastOrDefault(-1) < el.Index)
                {
                    num.Add(el);
                    if (num.Count == targetLength)
                    {
                        break;
                    }
                    i = -1;
                }
            }
            var join = string.Join("", num.Select(x => x.Number));
            result += long.Parse(join);
        }
        return result;
    }
}