using Vigem_ClassLibrary.Commands;
using Vigem_ClassLibrary.SystemImplementations;
using Vigem_Common;

namespace Vigem_ClassLibrary
{
    public class CommandExecutor
    {
        private readonly IStopwatch stopwatch;
        public CommandExecutor(IStopwatch stopwatch)
        {
            this.stopwatch = stopwatch;
        }

        public void ExecuteCommands(IDictionary<double, IEnumerable<IControllerCommand>> tsAndCmds, IController controller, double firstStateTs)
        {
            IEnumerable<double> orderedTimestamps = tsAndCmds.Keys.OrderBy(t => t);
            double firstStateTsReduction = orderedTimestamps.First() - firstStateTs;

            Dictionary<double, IEnumerable<IControllerCommand>> updatedTsAndCmds = new();
            foreach ((double ts, IEnumerable<IControllerCommand> commands) in tsAndCmds)
            {
                updatedTsAndCmds.Add(ts - firstStateTsReduction, commands);
            }
            orderedTimestamps = updatedTsAndCmds.Keys.OrderBy(t => t);
            stopwatch.Restart();

            foreach (double ts in orderedTimestamps)
            {
                IEnumerable<IControllerCommand> commands = updatedTsAndCmds[ts];
                while (stopwatch.GetElapsedTotalMilliseconds() < ts) {
                    //continue until it's time
                }

                // double elapsedTotalMilliseconds = stopwatch.GetElapsedTotalMilliseconds();
                // Console.WriteLine($"Commands: {commands.Count()} {elapsedTotalMilliseconds} - {ts} = {elapsedTotalMilliseconds - ts}");

                // double elapsedTotalMilliseconds = stopwatch.GetElapsedTotalMilliseconds();
                foreach (IControllerCommand command in commands) command.ExecuteCommand(controller);
                // Console.WriteLine($"Commands exec time: {stopwatch.GetElapsedTotalMilliseconds() - elapsedTotalMilliseconds}");
            }
            stopwatch.Stop();
        }
    }
}
