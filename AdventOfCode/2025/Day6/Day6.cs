using System.Text.RegularExpressions;
using Xunit.Abstractions;
using static AdventOfCode._2025.Utils;

namespace AdventOfCode._2025.Day6;

public class Day6(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Part1()
    {
        // Assert.Equal(4277556, Get(Day6Input.Example, new MutedTestOutputHelper(), Transform1));
        Assert.Equal(4412382293768, Get(Day6Input.Input, testOutputHelper, Transform1));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(3263827, Get(Day6Input.Example, testOutputHelper, Transform2));
        // Assert.Equal(4412382293768, Get(Day6Input.Input, new MutedTestOutputHelper(), Transform2));
    }

    private long Get(string input, ITestOutputHelper logger, Func<List<string>, List<long>> transform)
    {
        var lines = GetInputLines(input).ToList();
        var lastLine = lines.Last();
        lines.Remove(lastLine);
        var matches = Regex.Matches(lastLine, @"(\S)(\s+)");
        var colSizes = matches.Select(m => m.Value.Length).ToList();
        var opList = Regex.Matches(lastLine, @"\S");
        
        var colMap = new Dictionary<int, List<string>>();
        for (var i = 0; i < colSizes.Count;)
        {
            var colSize = colSizes[i];
            var take = colSize;
            foreach (var line in lines)
            {
                var splits = line.Chunk(take).ToList();
                for (int j = 0; j < splits.Count; j++)
                {
                    if (!colMap.ContainsKey(j))
                    {
                        colMap.Add(j, new List<string>());
                    }
                    var item = new string(splits[j]);
                    if (j < splits.Count - 1)
                    {
                        item = item[..^1];
                    }
                    colMap[j].Add(item);
                }
            }
            break;
        }
        
        long total = 0;
        foreach (var pair in colMap)
        {
            var op = opList[pair.Key];
            var pairValue = transform(pair.Value);
            var aggregate = pairValue.Aggregate((x, y) => op.Value == "*" ? x * y : x + y);
            total += aggregate;
            logger.WriteLine(Serialize(pairValue) + " using " + op + " is " + aggregate + " for total " + total);
        }
        return total;
    }

    private List<long> Transform1(List<string> pairValue)
    {
        var transform1 = pairValue.Select(s => long.Parse(s.Trim())).ToList();
        return transform1;
    }

    private List<long> Transform2(List<string> pairValue)
    {
        var highestPlacement = pairValue.Select(n => n.ToString().Length).Max();
        var strList = pairValue.Select(n =>
            {
                var numberStr = n.ToString();
                while (numberStr.Length < highestPlacement)
                {
                    numberStr = "x" + numberStr;
                }
                return numberStr;
            }
        ).ToList();

        var newList = new List<long>();
        for (var i = 0; i < highestPlacement; i++)
        {
            var newNumber = "";
            foreach (var numberStr in strList)
            {
                if (numberStr[i] != 'x')
                {
                    newNumber += numberStr[i];
                }
            }
            newList.Add(long.Parse(newNumber));
        }
        return newList;
    }
}