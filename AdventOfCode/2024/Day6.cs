﻿using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace AdventOfCode._2024;

public class Day6(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Part1Example()
    {
        Assert.Equal(41, GetResult(exampleString1, false, []));
    }

    [Fact]
    public void Part1Real()
    {
        Assert.Equal(5199, GetResult(realString, false, []));
    }
    
    [Fact]
    public void Part2Example()
    {
        Assert.Equal(6, GetResult(exampleString1, true, []));
    }
    
    [Fact]
    public void Part2Real()
    {
        Assert.Equal(90669332, GetResult(realString, true, []));
    }

    private class Pos(int x, int y)
    {
        public int x { get; set; } = x;
        public int y { get; set; } = y;
    }

    private class Guard(int x, int y, char c): Pos(x, y)
    {
        public char c { get; set; } = c;
    }

    private int GetResult(string str, bool paradox, HashSet<string> log)
    {
        var lineLength = str.IndexOf(Environment.NewLine);
        var map = str.ReplaceLineEndings("").Chunk(lineLength).Select(x => x.ToList()).ToList();

        var transforms = new List<char>(['^', '>', 'v', '<']);
        bool IsGuard(char c) => transforms.Contains(c);
        var guardX = map.FindIndex(x => x.Any(IsGuard));
        var guardY = map[guardX].FindIndex(IsGuard);
        var guard = new Guard(guardX, guardY, map[guardX][guardY]);
        var history = new HashSet<string>();
        void TurnGuard() {
            guard.c = transforms.ElementAtOrDefault(transforms.IndexOf(guard.c) + 1);
            if (guard.c == '\0')
            {
                guard.c = transforms.First();
            }
        }
        string GetLog(int x, int y, char c) => $"{x}-{y}-{c}";
        string GetGuardLog() => $"{guard.x}-{guard.y}-{guard.c}";
        void Log()
        {
            var l = GetGuardLog();
            if (!history.Add(l))
            {
                throw new ArgumentException();
            }
        }

        Log();

        var paradoxTotal = 0;
        while (true)
        {
            Pos GetForward() =>
                guard.c switch
                {
                    '^' => new Pos(guard.x - 1, guard.y),
                    '>' => new Pos(guard.x, guard.y + 1),
                    'v' => new Pos(guard.x + 1, guard.y),
                    _ => new Pos(guard.x, guard.y - 1)
                };

            var f = GetForward();
            void MarkGuardX() => map[guard.x][guard.y] = 'X';

            bool ForwardDoesNotExist(Pos p) => map.ElementAtOrDefault(p.x) == null || map[p.x].ElementAtOrDefault(p.y) == '\0';

            if (ForwardDoesNotExist(f))
            {
                MarkGuardX();
                break;
            }
            
            if (map[f.x][f.y] == '#')
            {
                TurnGuard();
                continue;
            }

            MarkGuardX();
            guard.x = f.x;
            guard.y = f.y;
            map[guard.x][guard.y] = guard.c;
            Log();

            var orig = guard.c;
            TurnGuard();
            var possibleForward = GetForward();
            guard.c = orig;
            if (!ForwardDoesNotExist(possibleForward) && history.Contains(GetLog(possibleForward.x, possibleForward.y, guard.c)))
            {
                paradoxTotal++;
            }
        }

        // testOutputHelper.WriteLine(string.Join(Environment.NewLine, map.Select(r => string.Join("", r))));
        return !paradox ? map.Sum(r => r.Count(c => c == 'X')) : paradoxTotal;

        // var result = 0;
        // for (int i = 0; i < map.Count; i++)
        // {
        //     for (int j = 0; j < map[i].Count; j++)
        //     {
        //         if (i == guardX && j == guardY)
        //         {
        //             continue;
        //         }
        //         
        //         var orig = map[i][j];
        //         map[i][j] = '#';
        //         try
        //         {
        //             GetResult(str, false, history);
        //         }
        //         catch (Exception e)
        //         {
        //             result++;
        //         }
        //         finally
        //         {
        //             map[i][j] = orig;
        //         }
        //     }
        // }
        //
        // return result;
    }

    private string exampleString1 = @"....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...";
    private string exampleString2 = "";
    private string realString = @".......#.........#..............#........................#......#....#............................#..#...............#............
...................#.............................................#...................................#............................
...............#.................................##.........#..........#.....##...#...#.....#....#..................#.............
#.............................................#..#.............#..........#.##.............................................#......
#.........................................................#.....#........................................##.......................
...........#.......#.#...........#....#.#...#......................................#...........................#...##............#
........................................................#..#.....................................#..#................#....#...#...
#.#.....#.........................#...........#.......................................#.......#.....................##............
.......#..........##.....#........#..................................................#.........................................#.#
#.#..............#.#.............................#............#....#...............#.........................#............#..#.#.#
..........................................................#....................................................#..................
.......................#....#...........#.....................#....#...#.......#..............#.......................#...........
..............................#..............#.......#....................................#.....................#...#...#.........
........#.................#...#....##..............#...........................#............................#.................#...
..................#........................................#......#......................................................#........
.............#.................#...#..............................................................................#...#...........
.....................................#..........................................................#.#.#......#.........#............
..........#...................................................................#.........##.......#..#.............................
...#.#....................................#..#...........#.......................................................##...............
........................................................................................................#.........................
.........#...........#......#..............#........#....................#...#......#.............................................
........#....#...............#....................#..........#.....................................#......#......#........#.#.....
........................#..........................#.......#........................#................#......#.....................
......................................#.........................................#............................#....................
...................................#.#...#...........#.........................................#.....................#............
.................#....#.....................................................................#..................#.........#........
...........#...........................................................#.............................#...............#..#.........
..#.....................#.........................................................................................................
....................#....#.#...........................#...........................................#..............#...............
...#...................................................#.............................................#.........#..........#.......
.............................###.....#..#..#......................................................................................
##................#.#.......#..............#.....#...............#..#............#................................................
......#...............................................#.#..#............................#............#.................#..........
........................................#.#...................................................#.....#.............................
#.......#.#.......................#.....#....................................#...................#................................
...##................................................................................................#.....................#......
............#.............#..................#.................#..................................................................
...#..............#..............................................................................#..........#......#..............
...#..#...#................................................#.....................#......#......................#.......#..........
.................#..#........#...............#.#................#......##.................#.................................#.....
#.....#.......#...........................................#..........#..............................#...#.#..#......#.....#.......
....................................#.........#........#..................#.............................................#.........
#...............................#........................#.................................................#.....#................
..#..................................#.......#.....................#..............................................................
........#...#.....................................#......#..............#..#..#..........................#.#....#.................
.............#..........#....#................#.......#..........................................#.....#..........................
.............................................#................................#.........................#...#.....................
.........#........#...................................................................#.......#....#..............................
....................................#.......................#..............#...............................#......................
....#....................#................................#....#...........#.............#........................................
....#...........#...................#.#..................................................#.....................#..................
..........#....................#............#.##...#.......#....#...............#.....#...#.........................#.............
.......#....................................................................................................#.................#...
...............................................................................#......#.................#...#.....................
..................#........#..........#..........................................................................#................
..........................................................................................#.......................................
.#..#...........#.................................##...........................#.....#...................#..#.....................
........................#..#..........................................................#............#..................#..........#
...........#...................................................##...........................#...............................#.....
........#......................................................................................................#..................
........#..#............................#..#...........#..............................#......................#.##...........#....#
........#...#.#..............#...#.....................#....................#.#..............................#....................
.......................................................#....................................................#..#..................
...........................................#......................#................#........................#....................#
.............#...............#................................................................................#...................
..#...#............#.............#.....#........................#............#....................#...............................
.....#...................................................................................................................#........
....................#..#...........#....#.............................................................................#.......#...
..............#......#.........................#...........#...#...........................................#...#..................
..............................#.....#.........................................................#......#.#......#...................
............................................................#...................#.#............................#..................
.............###.#........................................#........................................................#..............
......#................................#...............##.........................................................................
.........#.....#.#.....................#.......................................#...............#..........#....#....#.............
..............................................#....................................#...........................................#..
............#.#..........#........#..#.....#.............#..........#............#......#...................##..........#.........
............#....#...................................#.............................#.........................#....................
.#..................................#....#.........................................................................#..............
.........................#...#...#.................#..................#.............#.#...#...............#.......................
.........#...............................#...........#.............................#...........................#..........#.......
......#..........................#.............#..........#.......................................................................
.......#...............#.........#.............#...............................#..................................................
..#......#....................##...........................................#........................................#.............
..........................#.....#.......................#...........................................................#.#...........
..#....................#.............#................................................................#.#.........................
.........#....................#.....#....#...............#.......................#........#.......................................
..............#...................................................................#................#...#..........................
....................#.....#.......#..#......................................#...........#..................................##....#
.......#.......................................................................#..................................................
......#.............##..........................#..#......................^.......................................................
..............................#..........#...................#.................................#.....................#............
.......................##..........................#.......#..............................#...................................#.#.
................#....................................................................#...............................#.........#..
..........................................#......#.....................................#................#........#................
#......................................#.......................................................#..#...........#...................
.......#.............#..................#..#...#............#.......#.....#.....#.#...............................................
.............................#........#.............#.....#.......#...................................#.#..................#......
.................#......................................................................................#........#..#.............
........................................................................#..............#.....#..............................#.....
....#....#..........................................#......##...#....................#...................#..........#.............
.................##.......................##.#.........#................................................#.........................
.#..............................................#.........................#.#.#..........#.....................................##.
............................................................#..........#....#...................................................#.
....#...........#.............................#.....................#.........#..................#...........................#....
......#.#.......#..#......#........#..#...................#....#..................................................................
............#........................................................................................................#............
.....................................................#..........................................#................#................
...#.......#..#..............#.............................#....................................#.................................
.................##.........#........................#....................................#...............#.#.#...................
................#......#......................#..#..........................................#.....................................
......#..................................................##.#.#........#...........................#.......................#......
........................#............#......#.........#.............#.............................................#.....#.##......
......................#........................................#.......#.........#.................#..#...........#...............
..#.......#.........##..#.#................................................................................#.........#............
........................................................................................#............#...........#................
................#...#....................#..##..#...............#.............................................................#...
...#........#..........................#.....#..........................#....................................#.........#.......##.
.................#...........................#.........#......#............................#.....................#................
...................#..........#....#.......#...................#.............................#..........#.........................
.......#..............#.................................#.........#....................................#.................#....##.#
..................................#.......................#....#..#.........................#....................#................
.....#.........#............#...#.........#........#...................#...#......................#.....#............#............
#.#....#...#........#................................##..........................#.............................##...#....#........
.............##..........##...#......#.......#.#................#.#.......................#.............#.........................
.............#....................................#....................................#..........................................
..................................#..............................#.......................................#................#.......
...................#...............#..............................................................................................
....#........#.................................#........#.....................................#.......#................#........#.
.................#......#.......................................................##.....#..................#.......................
...............#.......................#.#........#...........................................#........#..........................";
}