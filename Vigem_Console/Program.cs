using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using Nefarius.ViGEm.Client.Targets;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.Commands;
using Vigem_ClassLibrary.SystemImplementations;
using Vigem_Common.Mappings;
using VigemControllers_ClassLibrary;
using WindowsPixelReader;

namespace Vigem_Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, string> dict = GetArgs(args);
            string command = dict["command"];
            if (command == "dark-souls-run") doDarkSoulsRun(dict);
            else if (command == "ffix-jump-rope") doFf9JumpRope(dict);
            else if (command == "log-cursor") doLogCursor(dict);
            else if (command == "record") doRecord(dict);
            else if (command == "ffvi-auto-battle") doFf6AutoBattle();
            else if (command == "log-screen") doLogScreen(dict);
            else if (command == "test") doTest(dict);
        }

        private static void doTest(Dictionary<string, string> dict)
        {
            RealStopwatch s = new();
            Dualshock4Controller cds4 = GetConnectedDs4Controller();
            StopwatchControllerUser user = new(cds4, s, 100);
            nint handle = GetProcessHandle("chiaki");
            PixelReader pr = new();
            s.Restart();

            bool lastIsLeft = !true;
            while (true)
            {
                user.PressDPad(lastIsLeft ? DPadMappings.Right : DPadMappings.Left);
                lastIsLeft = !lastIsLeft;
                double ts = s.GetElapsedTotalMilliseconds();
                while (s.GetElapsedTotalMilliseconds() - ts < 300)
                {
                    Point point = new(483, 141);
                    Point screenPoint = pr.GetClientToScreen(handle, ref point);
                    Pixel pixel = pr.GetPixelAtLocation(screenPoint.X, screenPoint.Y);
                    if (pixel.PixelColor.GetBrightness() < 0.2) continue;
                    
                    s.Wait(1000);
                    int clientX = 144;
                    int clientY = 275;
                    Point point2 = new(clientX, clientY);
                    Point screenPoint2 = pr.GetClientToScreen(handle, ref point2);
                    pr.SaveScreen(screenPoint2.X, screenPoint2.Y, 196 - clientX, 286 - clientY, "C:\\D\\test2.jpg");
                    Color color = pr.GetScreenAverageColor(screenPoint2.X, screenPoint2.Y, 196 - clientX, 286 - clientY);
                    if (color is {R: 54, G: 55, B: 90})
                    {
                        user.PressButton(ButtonMappings.Options);
                    }
                }
                
                //484
                //135
                //0.1
            }
        }

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

        private static void doLogCursor(Dictionary<string, string> dict)
        {
            PixelReader pr = new();
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
                    Point cursorPos = pr.GetCursorLocation();
                    point.X = cursorPos.X;
                    point.Y = cursorPos.Y;
                }

                string message = pr.GetPixelAtLocation(point.X, point.Y).ToString();
                if (dict.ContainsKey("client"))
                {
                    nint handle = GetProcessHandle(dict["client"]);
                    Point clientPoint = pr.GetScreenToClient(handle, ref point);
                    message += $" Client coordinates = {clientPoint}";
                }
                
                Console.WriteLine(message);
                localStopwatch.Wait(int.Parse(dict["speed"]));
            }
        }

        private static void doLogScreen(Dictionary<string, string> dict)
        {
            int x1 = int.Parse(dict["X1"]);
            int x2 = int.Parse(dict["X2"]);
            int y1 = int.Parse(dict["Y1"]);
            int y2 = int.Parse(dict["Y2"]);

            int width = x2 - x1;
            int height = y2 - y1;

            PixelReader pr = new();
            Point point = new(x1, y1);
            if (dict.ContainsKey("client"))
            {
                nint handle = GetProcessHandle(dict["client"]);
                Point screenPoint = pr.GetClientToScreen(handle, ref point);
                x1 = screenPoint.X;
                y1 = screenPoint.Y;
            }

            RealStopwatch localStopwatch = new();
            localStopwatch.Restart();
            while (true)
            {
                Color c = pr.GetScreenAverageColor(x1, y1, width, height);
                Console.WriteLine(new Pixel(x1, y1, c));
                pr.SaveScreen(x1, y1, width, height, "C:\\D\\test.jpg");
                localStopwatch.Wait(int.Parse(dict["speed"]));
            }
        }

        private static void doRecord(Dictionary<string, string> dict)
        {
            string processName = dict["processName"];
            int countParam = int.Parse(dict["count"]);
            double durationParam = double.Parse(dict["duration"]);
            Console.WriteLine(
                $"ProcessName param - {processName}, count param - {countParam}, duration param - {durationParam}"
            );

            PixelReader pr = new();
            nint handle = GetProcessHandle(processName);
            Rectangle clientRect = pr.GetClientRect(handle);
            Point clientPoint = new(0, 0);
            Point screenPoint = pr.GetClientToScreen(handle, ref clientPoint);

            string directory = $"C:\\D\\Apps\\Vigem\\Recordings\\{DateTime.Now:yyyy-MM-dd--HH-mm-ss}";
            Directory.CreateDirectory(directory);

            int counter = 1;
            RealStopwatch localStopwatch = new();
            localStopwatch.Restart();
            while (true)
            {

                string filePath = $"{directory}\\{counter}.png";
                pr.SaveClient(clientRect, screenPoint, filePath);
                // if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.C)
                // {
                //     
                //     File.WriteAllText(dataFilePath, "");
                // }

                if (
                    (countParam != 0 && counter >= countParam)
                    || (durationParam != 0 && localStopwatch.GetElapsedTotalMilliseconds() >= durationParam)
                )
                {
                    break;
                }

                counter++;
            }
            Console.WriteLine("Done");
        }

        private static nint GetProcessHandle(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            Process process = processes.First();
            nint handle = process.MainWindowHandle;
            return handle;
        }

        private static void doFf9JumpRope(Dictionary<string, string> dict)
        {

            Dualshock4Controller cds4 = GetConnectedDs4Controller();
            DelayerControllerUser user = new(cds4, null, 0);
            PixelReader pr = new();
            RealStopwatch localStopwatch = new();
            // Point p = new(x, y);

            
            string client = dict["client"];
            int clientX = int.Parse(dict["X"]);
            int clientY = int.Parse(dict["Y"]);
            int speechBubble = int.Parse(dict["speechBubble"]);
            nint process = GetProcessHandle(client);
            Point clientPoint = new(clientX, clientY);
            Point screenPoint = pr.GetClientToScreen(process, ref clientPoint);

            // while (true)
            // {
            //     Pixel pixelAtLocation = pr.GetPixelAtLocation(screenPoint.X, screenPoint.Y);
            //     Console.WriteLine(pixelAtLocation);
            // }
            
            
            Func<int> getB = () => pr.GetPixelAtLocation(screenPoint.X, screenPoint.Y).PixelColor.B;
            localStopwatch.Restart();
            localStopwatch.Wait(1000);
            int counter = 0;
            user.HoldButton(ButtonMappings.Cross);
            localStopwatch.Wait(50);
            user.ReleaseButton(ButtonMappings.Cross);
            while (true)
            {
                Console.WriteLine($"{counter} cycles");

                int b = 0;
                counter += 1;

                Console.WriteLine("Looking for shadow");
                localStopwatch.WaitUntilTrue(
                    () =>
                    {
                        b = getB.Invoke();
                        Console.WriteLine(counter + "+" + b);
                        return b == speechBubble;
                    }
                );
                Console.WriteLine("Shadow found");
                // localStopwatch.Wait(10);

                user.HoldButton(ButtonMappings.Cross);
                localStopwatch.Wait(50);
                user.ReleaseButton(ButtonMappings.Cross);
                Console.WriteLine("Button pressed");


                Console.WriteLine("Looking for light");
                localStopwatch.WaitUntilTrue(
                    () =>
                    {
                        b = getB.Invoke();
                        Console.WriteLine(counter + "+" + b);
                        return b != speechBubble;
                    }
                );
                Console.WriteLine("Light found");
            }
        }

        private static void doDarkSoulsRun(Dictionary<string, string> dict)
        {
            Dualshock4Controller cds4 = GetConnectedDs4Controller();
            RealStopwatch executorStopWatch = new();
            CommandExecutor executor = new(executorStopWatch, cds4);
            ChromeGamepadStateParser parser = new();
            PixelReader pixelReader = new();
            RealStopwatch localStopwatch = new();
            string runFile = dict["run-file"];
            string run = File.ReadAllText(runFile);

            bool runTypeIsPixelRead = dict["pixel-read"] == "true";
            IDictionary<double, IEnumerable<IControllerCommand>> states = parser.GetStates(run);
            localStopwatch.Restart();
            while (true)
            {
                Point point = new(0, 0);
                if (runTypeIsPixelRead)
                {
                    point = new Point(int.Parse(dict["X"]), int.Parse(dict["Y"]));
                    localStopwatch.WaitUntilTrue(
                        () => pixelReader.GetPixelAtLocation(point.X, point.Y).PixelColor.GetBrightness() != 0
                    );
                    localStopwatch.Wait(2000);
                }

                executor.ExecuteCommands(states);

                if (runTypeIsPixelRead)
                {
                    localStopwatch.WaitUntilTrue(
                        () => pixelReader.GetPixelAtLocation(point.X, point.Y).PixelColor.GetBrightness() == 0
                    );
                }
                else
                {
                    localStopwatch.Wait(int.Parse(dict["repeat-delay"]));
                }
            }
        }

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