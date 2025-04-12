using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using Nefarius.ViGEm.Client.Targets;
using VigemLibrary;
using VigemLibrary.Commands;
using VigemLibrary.Controllers;
using VigemLibrary.Mappings;
using VigemLibrary.SystemImplementations;
using WindowsScreenReading;

namespace Vigem_Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, string> dict = GetArgs(args);
            string command = dict["command"];
            // if (command == "dark-souls-run") doDarkSoulsRun(dict);
            // else if (command == "ffix-jump-rope") doFf9JumpRope(dict);
            // else if (command == "ffix-jump-rope-2") doFf9JumpRope2(dict);
            if (command == "log-cursor") doLogCursor(dict);
            // else if (command == "record") doRecord(dict);
            else if (command == "ffvi-auto-battle") doFf6AutoBattle();
            // else if (command == "log-screen") doLogScreen(dict);
            // else if (command == "test") doTest(dict);
            else if (command == "crisis-core") doCrisisCoreTest();
            else if (command == "ffvii-super-dunk") doFf7SuperDunk();
            else if (command == "ffvii-farm") doFf7Farm();
            // else if (command == "rebirth") doFf9JumpRope3(dict);
        }

        // private static void doFf9JumpRope3(Dictionary<string, string> dict)
        // {
        //     RealStopwatch s = new();
        //     Dualshock4Controller cds4 = GetConnectedDs4Controller();
        //     StopwatchControllerUser user = new(cds4, s, 100);
        //     PixelReader pr = new();
        //     nint handle = GetProcessHandle("chiaki");
        //     Point clientCoordinate = new(785, 433);
        //     User32.ClientToScreen(handle, ref clientCoordinate);
        //
        //     while (true)
        //     {
        //         var pixel = pr.GetPixelAtLocation(clientCoordinate.X, clientCoordinate.Y);
        //         if (pixel.PixelColor.GetBrightness() > 0.8)
        //         {
        //             //846 466
        //             s.Wait(50);
        //             user.PressButton(ButtonMappings.Cross);
        //             s.Wait(100);
        //         }
        //     }
        // }

        // Spam Barret Overcharge in trial. When it ends, triangle to retry brings up prompt.
        // When word Yes is located, move up to it and accept and repeat.
        // Based on laptop screen coordinates.
        private static void doRebirth(Dictionary<string, string> dict)
        {
            var tesseractUseCase = new TesseractUseCase();
            RealStopwatch s = new();
            Dualshock4Controller cds4 = GetConnectedDs4Controller();
            StopwatchControllerUser user = new(cds4, s, 100);
            while (true)
            {
                user.PressButton(ButtonMappings.Triangle);
                if (tesseractUseCase.ClientContainsTextInRect(
                    "chiaki",
                    "Yes",
                    932,
                    610,
                    988,
                    633
                ))
                {
                    for (int i = 0; i < 10; i++) user.PressDPad(DPadMappings.Up);
                    for (int i = 0; i < 10; i++) user.PressButton(ButtonMappings.Cross);
                }
            }
        }

        // private static void doTest(Dictionary<string, string> dict)
        // {
        //     RealStopwatch s = new();
        //     Dualshock4Controller cds4 = GetConnectedDs4Controller();
        //     StopwatchControllerUser user = new(cds4, s, 100);
        //     nint handle = GetProcessHandle("chiaki");
        //     PixelReader pr = new();
        //     s.Restart();
        //
        //     bool lastIsLeft = !true;
        //     while (true)
        //     {
        //         user.PressDPad(lastIsLeft ? DPadMappings.Right : DPadMappings.Left);
        //         lastIsLeft = !lastIsLeft;
        //         double ts = s.GetElapsedTotalMilliseconds();
        //         while (s.GetElapsedTotalMilliseconds() - ts < 300)
        //         {
        //             Point screenPoint = new(483, 141);
        //             User32.ClientToScreen(handle, ref screenPoint);
        //             Pixel pixel = pr.GetPixelAtLocation(screenPoint.X, screenPoint.Y);
        //             if (pixel.PixelColor.GetBrightness() < 0.8) continue;
        //             
        //             s.Wait(1000);
        //             int clientX = 144;
        //             int clientY = 275;
        //             Point screenPoint2 = new(clientX, clientY);
        //             User32.ClientToScreen(handle, ref screenPoint2);
        //             pr.SaveScreen(screenPoint2.X, screenPoint2.Y, 196 - clientX, 286 - clientY, "C:\\D\\test2.png");
        //             Color color = pr.GetScreenAverageColor(screenPoint2.X, screenPoint2.Y, 196 - clientX, 286 - clientY);
        //             if (color is {R: 54, G: 55, B: 90})
        //             {
        //                 user.PressButton(ButtonMappings.Options);
        //             }
        //         }
        //         
        //         //484
        //         //135
        //         //0.1
        //     }
        // }

        private static void doFf6AutoBattle()
        {
            RealStopwatch localStopwatch = new();
            Dualshock4Controller cds4 = GetConnectedDs4Controller();
            StopwatchControllerUser user = new(cds4, localStopwatch, 100);

            DPadMappings lastPressedDpad = DPadMappings.Left;
            while (true)
            {
                DPadMappings dpadToPress =
                    lastPressedDpad == DPadMappings.Left ? DPadMappings.Right : DPadMappings.Left;
                user.PressDPad(dpadToPress);
                lastPressedDpad = dpadToPress;
                //Uncomment to walk in same two spaces. Leave to walk from one wall to another.
                //localStopwatch.Wait(300);
            }
        }
        
        private static void doCrisisCoreTest()
        {
            RealStopwatch localStopwatch = new();
            Dualshock4Controller cds4 = GetConnectedDs4Controller();
            StopwatchControllerUser user = new(cds4, localStopwatch, 100);
            int delay = 100;
            while (true)
            {
                // user.PressButton(ButtonMappings.Circle);
                // localStopwatch.Wait(delay);
                
                user.PressButton(ButtonMappings.Square);
                localStopwatch.Wait(delay);
                // user.HoldButton(ButtonMappings.ShoulderRight);
                
                user.PressButton(ButtonMappings.Triangle);
                localStopwatch.Wait(delay);
                user.PressButton(ButtonMappings.Circle);
                localStopwatch.Wait(delay);;
                user.HoldButton(ButtonMappings.ThumbLeft);
                user.HoldButton(ButtonMappings.ThumbRight);
                localStopwatch.Wait(delay);
                user.ReleaseButton(ButtonMappings.ThumbLeft);
                user.ReleaseButton(ButtonMappings.ThumbRight);
                localStopwatch.Wait(delay);
                
                // user.PressDPad(DPadMappings.Down);
                // localStopwatch.Wait(500);
                // user.PressDPad(DPadMappings.Up);
                // localStopwatch.Wait(500);
                
            }
        }
        
        private static void doFf7Farm()
        {
            RealStopwatch s = new();
            Dualshock4Controller cds4 = GetConnectedDs4Controller();
            StopwatchControllerUser user = new(cds4, s, 100);
            
            // user.HoldDPad(DPadMappings.Left);
            // user.HoldButton(ButtonMappings.ShoulderLeft);
            user.HoldDPad(DPadMappings.Down);
            
            while (true)
            {
                user.PressButton(ButtonMappings.Cross, 100);
                s.Wait(100);
            }
        }
        
        private static void doFf7SuperDunk()
        {
            RealStopwatch s = new();
            Dualshock4Controller cds4 = GetConnectedDs4Controller();
            // int delay = 500;
            StopwatchControllerUser user = new(cds4, s, 100);
            //normal speed
            
            // user.PressButton(ButtonMappings.ThumbLeft);
            while (true)
            {
                for (int i = 0; i < 15; i++)
                {
                    user.PressButton(ButtonMappings.Cross, 100);
                    s.Wait(300);
                }
                
                user.PressButton(ButtonMappings.Circle, 450);
                // localStopwatch.Wait(delay);
            }
            
            // user.PressButton(ButtonMappings.ThumbLeft);
            
            //at x3 but too fast and random, need pixel read?
            // while (true)
            // {
            //     for (int i = 0; i < 20; i++)
            //     {
            //         user.PressButton(ButtonMappings.Cross, 50);
            //         s.Wait(100);
            //     }
            //     
            //     user.PressButton(ButtonMappings.Circle, 164);
            //     // localStopwatch.Wait(delay);
            // }
        }


        private static void doLogCursor(Dictionary<string, string> dict)
        {
            // PixelReader pr = new();
            BitmapWorker bw = new();
            RealStopwatch localStopwatch = new();
            localStopwatch.Restart();
            while (true)
            {
                Point point = new(0, 0);
                bool hasCoordinates = dict.ContainsKey("X") && dict.ContainsKey("Y");
                
                if (hasCoordinates)
                {
                    //Don't think this works, gotta update
                    point.X = int.Parse(dict["X"]);
                    point.Y = int.Parse(dict["Y"]);
                }
                else
                {
                    Point cursorPos = User32.GetCursorPos();
                    point.X = cursorPos.X;
                    point.Y = cursorPos.Y;
                }

                string message = bw.ProcessBitmap(
                    point.X, point.Y, bm =>
                    {
                        string s = new Pixel(point.X, point.Y, bw.GetAverageColor(bm)).ToString();
                        if (dict.ContainsKey("client"))
                        {
                            User32.ScreenToClient(dict["client"], ref point);
                            s += $" Client coordinates = {point}";
                        }
                        return s;
                    }
                );
                
                Console.WriteLine(message);
                localStopwatch.Wait(int.Parse(dict["speed"]));
            }
        }

        // private static void doLogScreen(Dictionary<string, string> dict)
        // {
        //     int x1 = int.Parse(dict["X1"]);
        //     int x2 = int.Parse(dict["X2"]);
        //     int y1 = int.Parse(dict["Y1"]);
        //     int y2 = int.Parse(dict["Y2"]);
        //
        //     int width = x2 - x1;
        //     int height = y2 - y1;
        //
        //     PixelReader pr = new();
        //     Point point = new(x1, y1);
        //     if (dict.ContainsKey("client"))
        //     {
        //         nint handle = GetProcessHandle(dict["client"]);
        //         User32.ClientToScreen(handle, ref point);
        //         x1 = point.X;
        //         y1 = point.Y;
        //     }
        //
        //     RealStopwatch localStopwatch = new();
        //     localStopwatch.Restart();
        //     while (true)
        //     {
        //         Color c = pr.GetScreenAverageColor(x1, y1, width, height);
        //         Console.WriteLine(new Pixel(x1, y1, c));
        //         pr.SaveScreen(x1, y1, width, height, "C:\\D\\test.png");
        //         localStopwatch.Wait(int.Parse(dict["speed"]));
        //     }
        // }

        // private static void doRecord(Dictionary<string, string> dict)
        // {
        //     string processName = dict["processName"];
        //     int countParam = int.Parse(dict["count"]);
        //     double durationParam = double.Parse(dict["duration"]);
        //     Console.WriteLine(
        //         $"ProcessName param - {processName}, count param - {countParam}, duration param - {durationParam}"
        //     );
        //
        //     PixelReader pr = new();
        //     nint handle = GetProcessHandle(processName);
        //     Rectangle clientRect = User32.GetClientRect(handle);
        //     var clientPoint = User32.ClientToScreen(handle);
        //
        //     string directory = $"C:\\D\\Apps\\Vigem\\Recordings\\{DateTime.Now:yyyy-MM-dd--HH-mm-ss}";
        //     Directory.CreateDirectory(directory);
        //
        //     int counter = 1;
        //     RealStopwatch localStopwatch = new();
        //     localStopwatch.Restart();
        //     while (true)
        //     {
        //
        //         string filePath = $"{directory}\\{counter}-{DateTime.Now:yyyy-MM-dd--HH-mm-ss.fff}.png";
        //         pr.SaveClient(clientRect, clientPoint, filePath);
        //         // if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.C)
        //         // {
        //         //     
        //         //     File.WriteAllText(dataFilePath, "");
        //         // }
        //
        //         if (
        //             (countParam != 0 && counter >= countParam)
        //             || (durationParam != 0 && localStopwatch.GetElapsedTotalMilliseconds() >= durationParam)
        //         )
        //         {
        //             break;
        //         }
        //
        //         counter++;
        //     }
        //     Console.WriteLine("Done");
        // }

        // private static nint GetProcessHandle(string processName)
        // {
        //     Process[] processes = Process.GetProcessesByName(processName);
        //     Process process = processes.First();
        //     nint handle = process.MainWindowHandle;
        //     return handle;
        // }

        // private static void doFf9JumpRope(Dictionary<string, string> dict)
        // {
        //
        //     Dualshock4Controller cds4 = GetConnectedDs4Controller();
        //     StopwatchControllerUser user = new(cds4, new RealStopwatch(), 0);
        //     PixelReader pr = new();
        //     RealStopwatch localStopwatch = new();
        //     // Point p = new(x, y);
        //
        //     
        //     string client = dict["client"];
        //     int clientX = int.Parse(dict["X"]);
        //     int clientY = int.Parse(dict["Y"]);
        //     int speechBubble = int.Parse(dict["speechBubble"]);
        //     nint process = GetProcessHandle(client);
        //     Point clientPoint = new(clientX, clientY);
        //     User32.ClientToScreen(process, ref clientPoint);
        //
        //     // while (true)
        //     // {
        //     //     Pixel pixelAtLocation = pr.GetPixelAtLocation(screenPoint.X, screenPoint.Y);
        //     //     Console.WriteLine(pixelAtLocation);
        //     // }
        //     
        //     
        //     Func<int> getB = () => pr.GetPixelAtLocation(clientPoint.X, clientPoint.Y).PixelColor.B;
        //     localStopwatch.Restart();
        //     localStopwatch.Wait(1000);
        //     int counter = 0;
        //     user.HoldButton(ButtonMappings.Cross);
        //     localStopwatch.Wait(50);
        //     user.ReleaseButton(ButtonMappings.Cross);
        //     while (true)
        //     {
        //         Console.WriteLine($"{counter} cycles");
        //
        //         int b = 0;
        //         counter += 1;
        //
        //         Console.WriteLine("Looking for shadow");
        //         localStopwatch.WaitUntilTrue(
        //             () =>
        //             {
        //                 b = getB.Invoke();
        //                 Console.WriteLine(counter + "+" + b);
        //                 return b == speechBubble;
        //             }
        //         );
        //         Console.WriteLine("Shadow found");
        //         // localStopwatch.Wait(10);
        //
        //         user.HoldButton(ButtonMappings.Cross);
        //         localStopwatch.Wait(50);
        //         user.ReleaseButton(ButtonMappings.Cross);
        //         Console.WriteLine("Button pressed");
        //
        //
        //         Console.WriteLine("Looking for light");
        //         localStopwatch.WaitUntilTrue(
        //             () =>
        //             {
        //                 b = getB.Invoke();
        //                 Console.WriteLine(counter + "+" + b);
        //                 return b != speechBubble;
        //             }
        //         );
        //         Console.WriteLine("Light found");
        //     }
        // }
        
        // private static void doFf9JumpRope2(Dictionary<string, string> dict)
        // {
        //
        //     Dualshock4Controller cds4 = GetConnectedDs4Controller();
        //     RealStopwatch localStopwatch = new();
        //     StopwatchControllerUser user = new(cds4, localStopwatch, 50);
        //     PixelReader pr = new();
        //     
        //     string client = dict["client"];
        //     int clientX = int.Parse(dict["X"]);
        //     int clientY = int.Parse(dict["Y"]);
        //     int speechBubble = int.Parse(dict["speechBubble"]);
        //     nint process = GetProcessHandle(client);
        //     Point clientPoint = new(clientX, clientY);
        //     User32.ClientToScreen(process, ref clientPoint);
        //     
        //     // Func<float> getB = () => pr.GetPixelAtLocation(screenPoint.X, screenPoint.Y).PixelColor.GetBrightness();
        //     // Func<int> getB = () => pr.GetPixelAtLocation(screenPoint.X, screenPoint.Y).PixelColor.B;
        //     localStopwatch.Restart();
        //     localStopwatch.Wait(1000);
        //     int counter = 0;
        //     
        //     user.PressButton(ButtonMappings.Cross);
        //     localStopwatch.Wait(2000);
        //     user.PressButton(ButtonMappings.Cross);
        //     localStopwatch.Wait(2000);
        //     user.PressButton(ButtonMappings.Cross);
        //     localStopwatch.Wait(2000);
        //
        //     Func<bool> pressCondition = () => pr.GetPixelAtLocation(clientPoint.X, clientPoint.Y).PixelColor.GetBrightness() <= 0.2;
        //     user.PressButton(ButtonMappings.Cross);
        //     localStopwatch.Wait(500);
        //     while (true)
        //     {
        //         // byte pixelColorB = pr.GetPixelAtLocation(screenPoint.X, screenPoint.Y).PixelColor.B;
        //         // Console.WriteLine(pixelColorB); 
        //         while (!pressCondition.Invoke())
        //         {
        //             // Console.WriteLine(counter + "+" + b); 
        //         }
        //         // localStopwatch.Wait( counter < 50 ? 100 : 10);
        //         // localStopwatch.Wait(counter >= 100 ? 20 : 25);
        //         // localStopwatch.Wait(50);
        //         counter++;
        //         Console.WriteLine($"Button press - {counter}");
        //         user.PressButton(ButtonMappings.Cross, 150);
        //         // localStopwatch.Wait(100);
        //         while (pressCondition.Invoke())
        //         {
        //             // Console.WriteLine(counter + "+" + b); 
        //         }
        //     }
        // }


        // private static void doDarkSoulsRun(Dictionary<string, string> dict)
        // {
        //     Dualshock4Controller cds4 = GetConnectedDs4Controller();
        //     RealStopwatch executorStopWatch = new();
        //     CommandExecutor executor = new(executorStopWatch, cds4);
        //     ChromeGamepadStateParser parser = new();
        //     PixelReader pixelReader = new();
        //     RealStopwatch localStopwatch = new();
        //     string runFile = dict["run-file"];
        //     string run = File.ReadAllText(runFile);
        //
        //     bool runTypeIsPixelRead = dict["pixel-read"] == "true";
        //     IDictionary<double, IEnumerable<IControllerCommand>> states = parser.GetStates(run);
        //     localStopwatch.Restart();
        //     while (true)
        //     {
        //         Point point = new(0, 0);
        //         if (runTypeIsPixelRead)
        //         {
        //             point = new Point(int.Parse(dict["X"]), int.Parse(dict["Y"]));
        //             localStopwatch.WaitUntilTrue(
        //                 () => pixelReader.GetPixelAtLocation(point.X, point.Y).PixelColor.GetBrightness() != 0
        //             );
        //             localStopwatch.Wait(2000);
        //         }
        //
        //         executor.ExecuteCommands(states);
        //
        //         if (runTypeIsPixelRead)
        //         {
        //             localStopwatch.WaitUntilTrue(
        //                 () => pixelReader.GetPixelAtLocation(point.X, point.Y).PixelColor.GetBrightness() == 0
        //             );
        //         }
        //         else
        //         {
        //             localStopwatch.Wait(int.Parse(dict["repeat-delay"]));
        //         }
        //     }
        // }

        private static Dualshock4Controller GetConnectedDs4Controller()
        {
            ControllerCreator creator = new();
            IDualShock4Controller createdDs4Controller = creator.GetDualShock4Controller();
            Dualshock4Controller cds4 = new(createdDs4Controller);
            cds4.Connect();
            return cds4;
        }

        private static Dictionary<string, string> GetArgs(string[] args)
        {
            Dictionary<string, string> result = new();
            foreach (string arg in args)
            {
                MatchCollection matches = Regex.Matches(arg, "--(.*?)=(.*)");
                Match match = matches[0];
                result.Add(match.Groups[1].ToString(), match.Groups[2].ToString());
            }

            return result;
        }
    }
}