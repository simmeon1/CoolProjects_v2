using System.Diagnostics;
using Vigem_ClassLibrary.Commands;
using Vigem_ClassLibrary.SystemImplementations;
using Vigem_Common;

namespace Vigem_ClassLibrary
{
    public class CommandExecutor
    {
        private IStopwatch stopwatch;
        public CommandExecutor(IStopwatch stopwatch)
        {
            this.stopwatch = stopwatch;
        }

        public void ExecuteCommands(IDictionary<double, IEnumerable<IControllerCommand>> tsAndCmds, IController controller)
        {
            stopwatch.Reset();
            IEnumerable<double> orderedTimestamps = tsAndCmds.Keys.OrderBy(t => t);
            stopwatch.Start();
            foreach (double ts in orderedTimestamps)
            {
                IEnumerable<IControllerCommand> commands = tsAndCmds[ts];
                while (ts > stopwatch.GetElapsedTotalMilliseconds()) {
                    //Wait for timer
                }

                foreach (IControllerCommand command in commands) command.ExecuteCommand(controller);
            }
            stopwatch.Stop();
        }
    }
}
