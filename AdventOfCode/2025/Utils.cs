using System.Text.Json;

namespace AdventOfCode._2025;

public static class Utils
{
    public static string Serialize(object obj) => JsonSerializer.Serialize(obj);

    public static string[] GetInputLines(string str) => str
        .Replace("\r", "")
        .Split("\n", StringSplitOptions.RemoveEmptyEntries);
}