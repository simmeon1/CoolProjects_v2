using System.Linq;
using System.Threading.Tasks;
using Common_ClassLibrary;
using SharpDX.DirectInput;

namespace AutoInput
{
    public class DirectInputUseCase
    {
        private IDelayer delayer;

        public DirectInputUseCase(IDelayer delayer)
        {
            this.delayer = delayer;
        }

        public async Task<Joystick> GetControllerJoystick()
        {
            DirectInput directInput = new();
            while (true)
            {
                DeviceInstance device = directInput
                    .GetDevices()
                    .FirstOrDefault(d => d.InstanceName.Equals("Wireless Controller"));
                if (device != null) return new Joystick(directInput, device.InstanceGuid);
                await delayer.Delay(1000);
            }
        }
    }
}