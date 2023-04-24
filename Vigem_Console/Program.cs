using Nefarius.ViGEm.Client.Targets;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.Commands;
using Vigem_ClassLibrary.SystemImplementations;
using VigemControllers_ClassLibrary;

namespace Vigem_Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, string> dict = GetCommandAndValuesDictionary(args);
            switch (dict["command"])
            {
                case "make-run":
                {
                    string runFile = dict["--run-file"];
                    string run = File.ReadAllText(runFile);
                    double repeatAfter = double.Parse(dict["--repeat-after"]);
                    Dualshock4Controller cds4 = GetConnectedDs4Controller();
                    RealStopwatch executorStopWatch = new();
                    CommandExecutor executor = new(executorStopWatch, cds4);
                    ChromeGamepadStateParser parser = new();
                    IDictionary<double, IEnumerable<IControllerCommand>> states = parser.GetStates(run);
                    RealStopwatch localStopwatch = new();
                    localStopwatch.Restart();
                    while (true)
                    {
                        executor.ExecuteCommands(states);
                        localStopwatch.Wait(repeatAfter);
                    }
                    break;
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

        private static Dictionary<string, string> GetCommandAndValuesDictionary(string[] strings)
        {
            Dictionary<string, string> commandsAndValues = new();
            for (int i = 0; i < strings.Length; i++)
            {
                string arg = strings[i];
                if (i == 0)
                {
                    commandsAndValues.Add("command", arg);
                } 
                else if (arg.StartsWith("--"))
                {
                    commandsAndValues.Add(arg, strings[i + 1]);
                }
            }
            return commandsAndValues;
        }

    }
}