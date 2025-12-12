using System.Text.Json;
using Xunit.Abstractions;

namespace AdventOfCode._2025.Day2;

public class Day2
{
    private readonly ITestOutputHelper testOutputHelper;

    public Day2(ITestOutputHelper testOutputHelper)
    {
        // this.testOutputHelper = testOutputHelper;
        this.testOutputHelper = new MutedTestOutputHelper();
    }

    [Fact]
    public void Part1()
    {
        var result = Get(Day2Input.Input);
        Assert.Equal(1227775554, result);
    }

    private long Get(string str)
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
                testOutputHelper.WriteLine("Processing left " + left);
                var leftStr = left.ToString();
                var length = leftStr.Length;
                if (length % 2 != 0)
                {
                    left++;
                    continue;
                }

                var chunks = leftStr.Chunk(length / 2).ToList();
                if (new string(chunks[0]) == new string(chunks[1]))
                {
                    testOutputHelper.WriteLine("Added left " + left);
                    rangeList.Add(left);
                }
                left++;
            }
        }
        testOutputHelper.WriteLine("Final dict " + Serialize(result));
        return result.Values.SelectMany(x => x).Sum();
    }

    private static string Serialize(object obj) => JsonSerializer.Serialize(obj);
}