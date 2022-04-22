using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using ViGEm_Common;

namespace ViGEm_Console
{
    public class ViGEmUseCase
    {
        private DualshockControllerWrapper controllerWrapper;
        public ViGEmUseCase(ViGEmClient client)
        {
            IDualShock4Controller controller = client.CreateDualShock4Controller();
            controller.Connect();
            // controller.AutoSubmitReport = false;
            controllerWrapper = new DualshockControllerWrapper(controller);
        }

        public async Task PlayStates(List<HtmlControllerState> states)
        {
            // Stopwatch watch = new();
            double timeDiff = 500;
            for (int i = 0; i < states.Count; i++)
            {
                HtmlControllerState state = states[i];
                HtmlControllerState nextState = i == states.Count - 1 ? state : states[i + 1];
                // await Task.Delay((int) timeDiff);
                Thread.Sleep((int) timeDiff);
                controllerWrapper.SetStateFromHtmlControllerState(state);
                timeDiff = nextState.TIMESTAMP - state.TIMESTAMP;
                // watch.Restart();
                // while (watch.ElapsedMilliseconds < timeDiff) { }
            }
        }
    }
}