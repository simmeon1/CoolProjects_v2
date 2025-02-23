using VigemLibrary.Commands;
using VigemLibrary.Controllers;
using VigemLibrary.SystemImplementations;

namespace VigemLibrary
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
            foreach (var updatedTsAndCmd in tsAndCmds)
            {
                updatedTsAndCmds.Add(updatedTsAndCmd.Key - firstStateTsReduction, updatedTsAndCmd.Value);
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
