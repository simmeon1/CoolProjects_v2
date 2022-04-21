using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using ViGEm_Common;

namespace ViGEm_Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // ViGEmClient clientt = new();
            // IDualShock4Controller controller = clientt.CreateDualShock4Controller();
            // controller.Connect();
            // while (true)
            // {
            //     int millisecondsDelay = 50;
            //     controller.SetDPadDirection(DualShock4DPadDirection.West);
            //     await Task.Delay(millisecondsDelay).ConfigureAwait(false);
            //     controller.SetDPadDirection(DualShock4DPadDirection.None);
            //     await Task.Delay(millisecondsDelay).ConfigureAwait(false);
            // }
            
            ViGEmClient client = new();
            ViGEmUseCase useCase = new(client);
            WindowsNativeMethods nativeMethods = new();


            string statesJson = await File.ReadAllTextAsync(@"C:\Users\simme\OneDrive\Desktop\controller_path.json");
            List<HtmlControllerState> states = HtmlControllerState.FromJsonArray(statesJson);
            
            while (true)
            {
                await useCase.PlayStates(states);
                // Point pos = new(4109, 324);
                Point pos = new(2125, 570);
            
                while (true)
                {
                    if (nativeMethods.GetColorAtLocation(pos).GetBrightness() == 0) break;
                    await Task.Delay(1000);
                }
            
                while (true)
                {
                    if (nativeMethods.GetColorAtLocation(pos).GetBrightness() == 0) continue;
                    await Task.Delay(3000);
                    break;
                }
            }
        }
    }
}