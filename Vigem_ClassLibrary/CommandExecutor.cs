using Vigem_ClassLibrary.Commands;
using Vigem_ClassLibrary.SystemImplementations;
using Vigem_Common;

namespace Vigem_ClassLibrary
{
    public class CommandExecutor
    {
        private readonly IStopwatch stopwatch;
        private readonly IController controller;

        public CommandExecutor(IStopwatch stopwatch, IController controller)
        {
            this.stopwatch = stopwatch;
            this.controller = controller;
        }

        public void ExecuteCommands(IDictionary<double, IEnumerable<IControllerCommand>> tsAndCmds)
        {
            IEnumerable<double> orderedTimestamps = tsAndCmds.Keys.OrderBy(t => t);
            double firstStateTsReduction = orderedTimestamps.First();

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
                stopwatch.WaitUntilTimestampReached(ts);
                foreach (IControllerCommand command in commands) command.ExecuteCommand(controller);
            }
            stopwatch.Stop();
        }
    }
}
