using System.Text.RegularExpressions;
using Xunit.Abstractions;
using static AdventOfCode._2025.Utils;

namespace AdventOfCode._2025.Day6;

public class Day6(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Part1()
    {
        Assert.Equal(4277556, Get(Day6Input.Example, new MutedTestOutputHelper()));
        Assert.Equal(4412382293768, Get(Day6Input.Input, new MutedTestOutputHelper()));
    }

    // [Fact]
    // public void Part2()
    // {
    //     Assert.Equal(3121910778619, Get(Day6Input.Example, 12, testOutputHelper));
    //     Assert.Equal(172787336861064, Get(Day6Input.Input, 12, new MutedTestOutputHelper()));
    // }

    private long Get(string input, ITestOutputHelper logger)
    {
        var lines = GetInputLines(input)
            .Select(s =>
                Regex.Split(s, "\\s+")
                    .Where(s => !Regex.IsMatch(s, "\\s+") && s.Length > 0).ToList()
            )
            .ToList();

        var opList = lines.Last();
        lines.RemoveAt(lines.Count - 1);

        var colMap = new Dictionary<int, List<long>>();

        for (var row = 0; row < lines.Count; row++)
        {
            for (var column = 0; column < lines[row].Count; column++)
            {
                if (!colMap.ContainsKey(column))
                {
                    colMap.Add(column, new List<long>());
                }
                colMap[column].Add(long.Parse(lines[row][column]));
            }
        }

        long total = 0;
        foreach (var pair in colMap)
        {
            var op = opList[pair.Key];
            var aggregate = pair.Value.Aggregate((x, y) => op == "*" ? x * y : x + y);
            total += aggregate;
            logger.WriteLine(Serialize(pair.Value) + " using " + op + " is " + aggregate + " for total " + total);
        }
        return total;
    }
}