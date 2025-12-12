using System.Text.Json;
using Xunit.Abstractions;

namespace AdventOfCode._2025.Day2;

public class Day2
{
    private readonly ITestOutputHelper testOutputHelper = new MutedTestOutputHelper();

    public Day2(ITestOutputHelper testOutputHelper)
    {
        // this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(1227775554, Get(Day2Input.Example, Part1Strategy));
        Assert.Equal(31210613313, Get(Day2Input.Input, Part1Strategy));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(4174379265, Get(Day2Input.Example, Part2Strategy));
        Assert.Equal(41823587546, Get(Day2Input.Input, Part2Strategy));
    }

    private long Get(string str, Action<long, List<long>> strategy)
    {
        var result = new Dictionary<string, ICollection<long>>();
        foreach (var range in str.Split(",", StringSplitOptions.RemoveEmptyEntries))
        {
            testOutputHelper.WriteLine("Processing " + range);
            var rangeList = new List<long>();
            result.Add(range, rangeList);
            var pieces = range.Split("-", StringSplitOptions.RemoveEmptyEntries);
            var left = long.Parse(pieces[0]);
            var right = long.Parse(pieces[1]);
            while (left <= right)
            {
                strategy(left, rangeList);
                left++;
            }
        }
        testOutputHelper.WriteLine("Final dict " + Serialize(result));
        return result.Values.SelectMany(x => x).Sum();
    }

    private void Part1Strategy(long left, List<long> rangeList)
    {
        testOutputHelper.WriteLine("Processing left " + left);
        var leftStr = left.ToString();
        var length = leftStr.Length;
        if (length % 2 != 0)
        {
            return;
        }

        var chunks = leftStr.Chunk(length / 2).ToList();
        if (new string(chunks[0]) == new string(chunks[1]))
        {
            testOutputHelper.WriteLine("Added left " + left);
            rangeList.Add(left);
        }
    }

    private void Part2Strategy(long left, List<long> rangeList)
    {
        testOutputHelper.WriteLine("left " + left);
        var leftStr = left.ToString();
        var length = leftStr.Length;
        if (length < 2)
        {
            return;
        }
        var currShortLength = (int) Math.Ceiling(length / 2.0);
        for (var i = currShortLength; i > 0; i--)
        {
            var chunks = leftStr.ToCharArray().Chunk(i).ToList();
            testOutputHelper.WriteLine("chunks " + Serialize(chunks));
            if (chunks.DistinctBy(x => new string(x)).Count() == 1 && !new string(chunks[0]).StartsWith("0"))
            {
                testOutputHelper.WriteLine("Added " + left);
                rangeList.Add(left);
                return;
            }
        }
    }

    private static string Serialize(object obj) => JsonSerializer.Serialize(obj);
}