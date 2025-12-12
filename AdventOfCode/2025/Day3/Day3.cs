using Xunit.Abstractions;
using static AdventOfCode._2025.Utils;

namespace AdventOfCode._2025.Day3;

public class Day3(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Part1()
    {
        Assert.Equal(357, Get(Day3Input.Example, testOutputHelper));
        Assert.Equal(31210613313, Get(Day3Input.Input, new MutedTestOutputHelper()));
    }

    // [Fact]
    // public void Part2()
    // {
    //     Assert.Equal(4174379265, Get(Day3Input.Example, Part2Strategy));
    //     Assert.Equal(41823587546, Get(Day3Input.Input, Part2Strategy));
    // }

    private int Get(string input, ITestOutputHelper logger)
    {
        var result = 0;
        var lines = GetInputLines(input);
        foreach (var line in lines)
        {
            logger.WriteLine("line " + line);
            var arr = line.ToCharArray().Select(x => int.Parse(x.ToString())).ToArray();
            var numberIndexMap = new Dictionary<int, List<int>>();
            for (var i = 0; i < arr.Length; i++)
            {
                var number = arr[i];
                if (!numberIndexMap.ContainsKey(number))
                {
                    numberIndexMap.Add(number, [i]);
                }
                else
                {
                    numberIndexMap[number].Add(i);
                }
            }
            logger.WriteLine("map " + Serialize(numberIndexMap));

            var indexes = numberIndexMap
                .OrderByDescending(x => x.Key)
                .SelectMany(x => x.Value).ToList();

            var possibleNumbers = new List<int>();

            for (var i = 0; i < indexes.Count; i++)
            {
                var firstIndex = indexes[i];
                var higherIndexes = indexes.Where(index => index > firstIndex).ToList();
                if (!higherIndexes.Any())
                {
                    var possible = int.Parse(arr[firstIndex].ToString());
                    possibleNumbers.Add(possible);
                }
                else
                {
                    var secondIndex = higherIndexes.MaxBy(x => int.Parse(arr[x].ToString()));
                    var possible = int.Parse(arr[firstIndex].ToString() + arr[secondIndex]);
                    possibleNumbers.Add(possible);
                }
            }
            logger.WriteLine("possibleNumbers " + Serialize(possibleNumbers));
            var max = possibleNumbers.Max();
            logger.WriteLine("Adding max " + max + " to result");
            result += max;
        }
        return result;
    }
}