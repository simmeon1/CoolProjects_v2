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
            // if (command == "log-client-pos") doLogClientPos(dict);
            else if (command == "record") doRecord(dict);
            else if (command == "ffvi-auto-battle") doFf6AutoBattle();
            // else if (command == "save-client") doSaveClient();
            // else if (command == "log-screen") doLogScreen(dict);
            // else if (command == "test") doTest(dict);
            else if (command == "crisis-core") doCrisisCoreTest();
            else if (command == "ffvii-super-dunk") doFf7SuperDunk();
            else if (command == "ffvii-farm") doFf7Farm();
            else if (command == "ffix-farm") doFf9Farm();
            else if (command == "ffix-grand-dragon") doFf9GrandDragon();
            else if (command == "rebirth") doFf9JumpRope3(dict);
            else if (command == "get-text") doGetTextBasedOnCursor(dict);
            else if (command == "r1-turbo") doR1Turbo(dict);
        }

        private static void doR1Turbo(Dictionary<string, string> dict)
        {
            RealStopwatch s = new();
            var user = GetStopwatchControllerUser(s, 100);
            var customUser = new CustomControllerUser(s, user);
            customUser.Create();
        }

        private static void doGetTextBasedOnCursor(Dictionary<string, string> dict)
        {
            var tesseractUseCase = new TesseractUseCase();
            RealStopwatch s = new();
            s.Restart();
            var client = dict["client"];
            User32.RestoreWindow(client);
            var rectWidth = int.Parse(dict["rectWidth"]);
            var rectHeight = int.Parse(dict["rectHeight"]);
            var speed = int.Parse(dict["speed"]);
            while (true)
            {
                Point point = User32.GetCursorPos();
                User32.ScreenToClient(client, ref point);
                var text = tesseractUseCase.GetTextFromClient(
                    client,
                    point.X,
                    point.Y,
                    point.X + rectWidth,
                    point.Y + rectHeight
                );
                Console.WriteLine($"X - {point.X}, Width - {rectWidth}, Y - {point.Y}, Height - {rectHeight} - {text}");
                s.Wait(speed);
            }
        }

        private static void doFf9Farm()
        {
            RealStopwatch s = new();
            var user = GetStopwatchControllerUser(s, 100);
            
            // user.HoldDPad(DPadMappings.Left);
            // user.HoldButton(ButtonMappings.ShoulderLeft);
            user.HoldDPad(DPadMappings.Right);
            
            while (true)
            {
                user.PressButton(ButtonMappings.Cross, 100);
                s.Wait(100);
            }
        }
        
        private static void doFf9GrandDragon()
        {
            // Client is 634x371
            RealStopwatch s = new();
            StopwatchControllerUser user = GetStopwatchControllerUser(s, 100, 50);
            var tess = new TesseractUseCase();

            var client = "chiaki";
            User32.RestoreWindow(client);
            var defaultRectWidth = 46;
            var defaultRectHeight = 14;

            string GetTextFromClient(int x, int y) => 
                tess.GetTextFromClient(client, x, y, x + defaultRectWidth, y + defaultRectHeight);

            void DoUntilTextComes(int x, int y, string text, Action action)
            {
                while (!TextContainsWord(x, y, text)) { action(); }
            }

            void DoUntilTextGoes(int x, int y, string text, Action action)
            {
                while (TextContainsWord(x, y, text)) { action(); }
            }

            bool WaitForText(int x, int y, string text, int maxWaitMilliseconds)
            {
                var elapsed = s.GetElapsedTotalMilliseconds();
                while (s.GetElapsedTotalMilliseconds() - elapsed < maxWaitMilliseconds)
                {
                    if (TextContainsWord(x, y, text))
                    {
                        return true;
                    }
                }
                return false;
            }

            void DoUntilTextComesAndGoes(int x, int y, string text, Action action)
            {
                DoUntilTextComes(x, y, text, action);
                DoUntilTextGoes(x, y, text, action);
            }

            bool TextContainsWord(int x, int y, string word) => GetTextFromClient(x, y).Trim().Contains(word);

            void ChangeCharsUntilTextSeen(int x, int y, string word) =>
                DoUntilTextComes(x, y, word, () => {
                    user.PressButton(ButtonMappings.Triangle);
                    s.Wait(200);
                });

            var spedUp = false;
            void ToggleSpeed()
            {
                user.PressButton(ButtonMappings.Options);
                user.PressButton(ButtonMappings.ShoulderRight);
                user.PressButton(ButtonMappings.Options);
                spedUp = !spedUp;
            }

            while (true)
            {
                // Speed up
                if (!spedUp)
                {
                    ToggleSpeed();
                }
                
                // Walk left right until spawn
                var goLeft = true;
                
                DoUntilTextComes(110, 273, "Attack", () => {
                    user.PressDPad(goLeft ? DPadMappings.Left : DPadMappings.Right, 300);
                    goLeft = !goLeft;
                });
                // Make sure Attack is selected
                s.Wait(100);
                user.PressDPad(DPadMappings.Left);
                user.PressDPad(DPadMappings.Left);
                
                // Switch chart until Zidane
                ChangeCharsUntilTextSeen(112, 296, "Steal");
                
                // Steal
                user.PressDPad(DPadMappings.Down);
                user.PressButton(ButtonMappings.Cross);
                user.PressButton(ButtonMappings.Cross);
                
                // Switch chart until Quina
                ChangeCharsUntilTextSeen(207, 296, "Blu Ma");
                
                // Lvl 5 Death
                user.PressDPad(DPadMappings.Down);
                user.PressDPad(DPadMappings.Right);
                user.PressButton(ButtonMappings.Cross);
                user.PressDPad(DPadMappings.Down);
                user.PressButton(ButtonMappings.Cross);
                user.PressButton(ButtonMappings.Cross);
                
                // Go through screens, con is confirm but sometimes text is different
                DoUntilTextComesAndGoes(189, 339, "con", () => { user.PressButton(ButtonMappings.Cross); });
                
                // Tent up
                void WaitForTent() => DoUntilTextComes(236, 105, "Tent", () => { user.PressButton(ButtonMappings.Square); });
                WaitForTent();
                
                // Slow down as moogle is broken when sped up
                ToggleSpeed();
                
                //Use tent
                user.PressDPad(DPadMappings.Down);
                user.PressButton(ButtonMappings.Cross);

                // If no tents, stop. Otherwise press X again to use it and continue.
                var noTents = WaitForText(313, 43, "any", 1000);
                if (noTents)
                {
                    Console.WriteLine("No more tents. Stopping.");
                    user.PressButton(ButtonMappings.Options);
                    return;
                }
                
                user.PressButton(ButtonMappings.Cross);
                
                ToggleSpeed();
                
                // Cancel tent
                WaitForTent();
                user.PressButton(ButtonMappings.Circle);
                user.PressButton(ButtonMappings.Cross);
            }
        }
        
        private static void doFf9JumpRope3(Dictionary<string, string> dict)
        {
            /*
             * How I got to 2060+ jumps:
             * 
             * At first, resized chiaki down so that recording frames would be recorded faster.
             * Final resolution was 634x371.
             * 
             * Needed a recording of the 1000 jumps. Downloaded a youtube video of the whole thing and used
             * ffmpeg to extract its frames at the same resolution.
             * Command akin to ffmpeg -i input.mp4 -vf "scale=634:371" output_%04d.png
             * 
             * Loaded images in ImageViewer which resizes itself to image resolution which is the same as chiaki
             * and placed viewer on top of chiaki. The only difference between chiaki stream and video was that
             * chiaki had black bars at top and bottom. Removed them by pressing Ctrl+O and selecting stretch option.
             * With that, images and stream were like to like.
             *
             * Used viewer with the youtube images to pinpoint a pixel and a value that would satisfy all jump speeds.
             * Pixel is top of Vivi's hat at height of his fastest jumps at 300+ jumps. His height is greater at slower
             * speeds and that would not satisfy higher speeds.
             * 
             * Coordinates are client coordinates, not screen coordinates. Given that viewer
             * and chiaki have the same size, client coordinates from viewer could be applied to chiaki.
             *
             * Spent a lot of time trying to jump by going by the white bubble but that was wrong - it would already
             * be too late if the bubble was visible. Recording of manual play with chiaki and the usb controller
             * buttons on top from control panel showed that buttons were pressed before the bubble and close to the
             * height of jump.
             *
             * A source of frustration was multiple clicks at times. This was solved by looking at the pixel and its
             * changes through the youtube images - it would change to a "must jump" state but not because of the hat
             * position but because of the white bubble appearing. Ignoring the bubble (easy as it's very bright) solved
             * this. Did shadow lookup attempts but there's more cases to code than above so gave up on it.
             *
             * A bit of luck was necessary as the same code that reached 2060 failed at 40 jumps. Suspicion is that chiaki
             * hadn't fully rendered. Could decrease streaming resolution to help next time. 
             *
             * Release build were more consistently better. I was stuck at 200 jumps for a long time thinking I need
             * a different pixel but a random release build seemed to fix it (can't remember if I had done release builds)
             * before. An attempt that failed at 762 jumps made me optimize more and removed console writes.
             *
             * Stacking image viewer on top of chiaki and youtube video worked right down to the pixel - approach is
             * extremely reliable.
             */
            
            RealStopwatch s = new();
            var user = GetStopwatchControllerUser(s, 100);
            Point clPoint = new(245, 167);
            Point shadowPoint = new(260, 215);
            var treshold = 160;
            BitmapWorker bw = new();
            var pressCounter = 0;
            Color GetColor() => bw.ProcessBitmap(
                clPoint.X, clPoint.Y, bm => bw.GetAverageColor(bm), "chiaki"
            );
            bool ShouldClick()
            {
                var color = GetColor();
                return color.R >= treshold && color.GetBrightness() < 0.7;
            }

            while (true)
            {
                if (ShouldClick())
                {
                    // Console.WriteLine($"press {++pressCounter} at {DateTime.Now:yyyy-MM-dd--HH-mm-ss.fff}");
                    // Console.WriteLine($"press {++pressCounter}");
                    user.PressButton(ButtonMappings.Cross);
                    while (ShouldClick()) { }
                }
            }
        }

        // Spam Barret Overcharge in trial. When it ends, triangle to retry brings up prompt.
        // When word Yes is located, move up to it and accept and repeat.
        // Based on laptop screen coordinates.
        private static void doRebirth(Dictionary<string, string> dict)
        {
            var tesseractUseCase = new TesseractUseCase();
            RealStopwatch s = new();
            var user = GetStopwatchControllerUser(s, 100);
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

        private static void doFf6AutoBattle()
        {
            RealStopwatch localStopwatch = new();
            var user = GetStopwatchControllerUser(localStopwatch, 100);

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
            var user = GetStopwatchControllerUser(localStopwatch, 100);
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
            var user = GetStopwatchControllerUser(s, 100);
            
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
            var user = GetStopwatchControllerUser(s, 100);
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
            dict.TryGetValue("client", out var client);
            if (client != null)
            {
                User32.RestoreWindow(client);
            }

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
                        if (client != null)
                        {
                            User32.ScreenToClient(client, ref point);
                            s += $" Client coordinates = {point}";
                        }
                        return s;
                    }
                );
                
                Console.WriteLine(message);
                localStopwatch.Wait(int.Parse(dict["speed"]));
            }
        }

        private static void doRecord(Dictionary<string, string> dict)
        {
            string processName = dict["processName"];
            User32.RestoreWindow(processName);
            int countParam = int.Parse(dict["count"]);
            // int countParam = 1;
            double durationParam = double.Parse(dict["duration"]);
            // double durationParam = 0;
            Console.WriteLine(
                $"ProcessName param - {processName}, count param - {countParam}, duration param - {durationParam}"
            );
        
            string directory = $"C:\\D\\Apps\\Vigem_scripts\\Recordings\\{DateTime.Now:yyyy-MM-dd--HH-mm-ss}";
            Directory.CreateDirectory(directory);
        
            int counter = 1;
            RealStopwatch localStopwatch = new();
            BitmapWorker bw = new();
            localStopwatch.Restart();
            while (true)
            {
                string filePath = $"{directory}\\{counter}-{DateTime.Now:yyyy-MM-dd--HH-mm-ss.fff}.png";
                bw.ProcessBitmap(
                    processName,
                    bm =>
                    {
                        bm.Save(filePath, BitmapWorker.GetImageFormatFromPath(filePath));
                        return true;
                    }
                );
        
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

        private static StopwatchControllerUser GetStopwatchControllerUser(IStopwatch s, int pressLength, int delayAfterSet = 0)
        {
            var controller = new Dualshock4Controller(new ControllerCreator().GetDualShock4Controller());
            controller.Connect();
            return new StopwatchControllerUser(controller, s, pressLength, delayAfterSet);
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