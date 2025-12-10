using Xunit.Abstractions;

namespace AdventOfCode._2025;

public class Day1(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Part1()
    {
        var actualMemory = GetPassword(Day1Part1Input.Example);
        Assert.Equal(6, actualMemory);
        Assert.Equal(6892, GetPassword(Day1Part1Input.Input));
    }

    private int GetPassword(string str)
    {
        var result = 0;
        var x = new CircleInt();
        var texts = str.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        foreach (var text in texts)
        {
            var add = x.Add(text, testOutputHelper);
            result += add;
        }
        return result;
    }

    private class CircleInt
    {
        private int start = 50;

        public int Add(string str, ITestOutputHelper testOutputHelper)
        {
            var zeroesHit = 0;
            var rotation = str[0];
            var amount = int.Parse(str[1..]);
            for (var i = 0; i < amount; i++)
            {
                var multiplier = rotation == 'L' ? -1 : 1;
                start += 1 * multiplier;
                if (start == 0 || start == 100)
                {
                    zeroesHit++;
                }
                else if (start == 101)
                {
                    start = 1;
                }
                else if (start == -1)
                {
                    start = 99;
                }
            }
            testOutputHelper.WriteLine(start.ToString());
            return zeroesHit;
        }
    }
}