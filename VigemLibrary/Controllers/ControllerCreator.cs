using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace VigemLibrary.Controllers
{
    public class ControllerCreator
    {
        private readonly ViGEmClient client = new();
        public IXbox360Controller GetXbox360Controller()
        {
            return client.CreateXbox360Controller();
        }

        public IDualShock4Controller GetDualShock4Controller()
        {
            return client.CreateDualShock4Controller();
        }
    }
}
