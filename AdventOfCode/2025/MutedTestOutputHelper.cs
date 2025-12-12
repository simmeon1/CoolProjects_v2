using Xunit.Abstractions;

namespace AdventOfCode._2025;

public class MutedTestOutputHelper : ITestOutputHelper
{
    public void WriteLine(string message)
    { }

    public void WriteLine(string format, params object[] args)
    { }
}