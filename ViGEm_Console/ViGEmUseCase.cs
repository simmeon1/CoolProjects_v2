using System.Collections.Generic;
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
            for (int i = 0; i < states.Count; i++)
            {
                HtmlControllerState state = states[i];
                HtmlControllerState nextState = i == states.Count - 1 ? state : states[i + 1];
                double timeDiff = nextState.TIMESTAMP - state.TIMESTAMP;
                controllerWrapper.SetStateFromHtmlControllerState(state);
                await Task.Delay((int) timeDiff);
            }
        }
    }
}