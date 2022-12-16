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

            stopwatch.Reset();
            stopwatch.Start();
            foreach (double ts in orderedTimestamps)
            {
                while (stopwatch.GetElapsedTotalMilliseconds() - (ts - firstStateTsReduction) < 0) {
                    //continue until it's time
                }

                IEnumerable<IControllerCommand> commands = tsAndCmds[ts];
                foreach (IControllerCommand command in commands) command.ExecuteCommand(controller);
            }
            stopwatch.Stop();
        }
    }
}
