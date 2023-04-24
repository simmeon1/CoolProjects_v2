using System.Drawing;
using System.Text.RegularExpressions;
using Nefarius.ViGEm.Client.Targets;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.Commands;
using Vigem_ClassLibrary.SystemImplementations;
using VigemControllers_ClassLibrary;
using WindowsPixelReader;

namespace Vigem_Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, string> dict = GetArgs(args);
            if (dict["command"] == "dark-souls-run")
            {
                string runFile = dict["run-file"];
                string run = File.ReadAllText(runFile);
                bool runTypeIsPixelRead = dict["pixel-read"] == "true";
                
                Dualshock4Controller cds4 = GetConnectedDs4Controller();
                RealStopwatch executorStopWatch = new();
                CommandExecutor executor = new(executorStopWatch, cds4);
                ChromeGamepadStateParser parser = new();
                PixelReader pixelReader = new();
                RealStopwatch localStopwatch = new();
                
                IDictionary<double, IEnumerable<IControllerCommand>> states = parser.GetStates(run);
                localStopwatch.Restart();
                while (true)
                {
                    Point point = new(0, 0);
                    if (runTypeIsPixelRead)
                    {
                        point = new Point( int.Parse(dict["X"]), int.Parse(dict["Y"]));
                        localStopwatch.WaitUntilTrue(() => pixelReader.GetPixelAtLocation(point).PixelColor.GetBrightness() != 0);
                        localStopwatch.Wait(2000);
                    }
                    
                    executor.ExecuteCommands(states);

                    if (runTypeIsPixelRead)
                    {
                        localStopwatch.WaitUntilTrue(() => pixelReader.GetPixelAtLocation(point).PixelColor.GetBrightness() == 0);
                    }
                    else
                    {
                        localStopwatch.Wait(int.Parse(dict["repeat-delay"]));
                    }

                }
            }
            else if (dict["command"] == "log-cursor")
            {
                PixelReader pr = new();
                RealStopwatch localStopwatch = new();
                localStopwatch.Restart();
                while (true)
                {
                    Console.WriteLine(pr.GetPixelAtCursor());
                    localStopwatch.Wait(1000);
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