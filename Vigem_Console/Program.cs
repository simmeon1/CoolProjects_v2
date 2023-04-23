using System.Diagnostics;
using Nefarius.ViGEm.Client.Targets;
using System.Drawing;
using System.Runtime.InteropServices;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.Commands;
using Vigem_ClassLibrary.SystemImplementations;
using Vigem_Common.Mappings;
using VigemControllers_ClassLibrary;

//using WindowsPixelReader;

namespace Vigem_Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ControllerCreator creator = new();

            IDualShock4Controller createdDs4Controller = creator.GetDualShock4Controller();
            Dualshock4Controller cds4 = new(createdDs4Controller);
            Delayer delayer = new();
            ControllerUser userDs4 = new(cds4, delayer, 500);
            cds4.Connect();

            //PixelReader pr = new();
            

            ChromeGamepadStateParser parser = new();

            // List<double> orderedTimestamps = new();
            // for (int i = 0; i < 100; i++)
            // {
            //     orderedTimestamps.Add(i * 100);
            // }
            // Stopwatch stopwatch = new();
            // stopwatch.Start();
            //
            // //stopwatch.Restart();
            // foreach (double ts in orderedTimestamps)
            // {
            //     while (stopwatch.Elapsed.TotalMilliseconds - ts <= 0)
            //     {
            //         //continue until it's time
            //     }
            //
            //     Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
            // }
            // //stopwatch.Stop();
            // return;
            
            // List<double> orderedTimestamps = new();
            // for (int i = 0; i < 100; i++)
            // {
            //     orderedTimestamps.Add(i * 100);
            // }
            // RealStopwatch stopwatch = new();
            // stopwatch.Start();
            // foreach (double ts in orderedTimestamps)
            // {
            //     while (stopwatch.GetElapsedTotalMilliseconds() <= ts) {
            //         //continue until it's time
            //     }
            //     
            //     Console.WriteLine(stopwatch.GetElapsedTotalMilliseconds());
            //
            //     IEnumerable<IControllerCommand> commands = new List<IControllerCommand>();
            //     foreach (IControllerCommand command in commands) command.ExecuteCommand(null);
            // }
            // return;

            // RealStopwatch stopwatch = new();
            // stopwatch.Start();
            // while (true)
            // {
            //     stopwatch.Restart();
            //     double diff = 10;
            //     while (stopwatch.GetElapsedTotalMilliseconds() - diff <= 0) {
            //         //continue until it's time
            //     }
            //     Console.WriteLine(stopwatch.GetElapsedTotalMilliseconds());
            //     //Console.WriteLine(GetDiff(ts, firstStateTsReduction));
            //
            //     // IEnumerable<IControllerCommand> commands = tsAndCmds[ts];
            //     // foreach (IControllerCommand command in commands) command.ExecuteCommand(controller);
            // }
            //
            // while (true)
            // {
            //     //s.Reset();
            //     Stopwatch s = new();
            //     s.Start();
            //     while (s.Elapsed.TotalMilliseconds < 100)
            //     {
            //         
            //     }
            //     //await Task.Delay(100);
            //     Console.WriteLine(s.Elapsed.TotalMilliseconds);
            //     //s.Reset();
            // }
            //
            //
            // // Stopwatch s = new();
            // // s.Start();
            // while (true)
            // {
            //     //s.Reset();
            //     Stopwatch s = new();
            //     s.Start();
            //     Thread.Sleep(94);
            //     //await Task.Delay(100);
            //     Console.WriteLine(s.ElapsedMilliseconds);
            //     //s.Reset();
            // }
            
            //Set mario controls
            // await Task.Delay(1000);
            // await userDs4.PressDPad(DPadMappings.Left);
            // await Task.Delay(1000);
            // await userDs4.PressDPad(DPadMappings.Right);
            // await Task.Delay(1000);
            // await userDs4.PressButton(ButtonMappings.Cross);


            // string darkSoulsRun = "a1:-1;t:4593.300000000745~b1:1;t:4773.39999999851~b1:0;t:10890.89999999851~b5:1;t:11266.60000000149~b5:0;t:11412.800000000745~b11:1;t:11521.5~b11:0;t:11625.300000000745~b11:1;t:11713.5~b11:0;t:11768~b11:1;t:11850.199999999255~b11:0;t:11939.800000000745~b11:1;t:12008~b11:0;t:12082.89999999851~b11:1;t:12177.10000000149~b11:0;t:12268.39999999851~a1:0.003921627998352051;t:12569.39999999851~b0:1;t:15175.89999999851~b0:0;t:15268.60000000149~b0:1;t:15365.699999999255~b0:0;t:15448.60000000149~b0:1;t:15552.39999999851~b0:0;t:15628~b0:1;t:15731.10000000149~b0:0;t:15802.89999999851~b0:1;t:15906.800000000745~b0:0;t:15977.699999999255~b0:1;t:16074.800000000745~b0:0;t:16146.39999999851~b0:1;t:16250.5~b0:0;t:16315.300000000745~b0:1;t:16416~b0:0;t:16539.800000000745~b2:1;t:17171.5~b2:0;t:17299~b13:1;t:17582.10000000149~b13:0;t:17755.699999999255~b0:1;t:18041.300000000745~b0:0;t:18171.300000000745~";
            string darkSoulsRun = "a0:1;t:145351.8262~a0:0;t:147808.7426~a0:-1;t:148081.5571~a0:0;t:148143.8495~b0:1;t:148315.6817~b0:0;t:148573.1937~a0:1;t:148614.2008~a0:0;t:148870.5956~b0:1;t:149054.7562~b0:0;t:149142.9093~a0:1;t:149471.2923~a0:0;t:149974.1375~a0:1;t:150459.5261~a0:0;t:150533.1387~b0:1;t:150599.019~b0:0;t:150840.4086~a0:1;t:150867.1433~a0:0;t:151143.0114~a0:1;t:151465.0884~a0:0;t:151638.9428~b0:1;t:151649.2897~b0:0;t:151895.7182~a0:1;t:151901.4855~a0:0;t:152187.6714~a0:-1;t:152591.485~b0:1;t:152621.4487~b0:0;t:153415.7202~a0:0;t:154620.5041~a0:1;t:154789.0705~a0:0;t:154830.8214~b0:1;t:155391.8079~b0:0;t:155601.8226";
            var states = parser.GetStates(darkSoulsRun);

            await delayer.Delay(1000);

            CommandExecutor executor = new(new RealStopwatch());
            while (true)
            {
                executor.ExecuteCommands(states, cds4, 0);
                await delayer.Delay(24000);
            }

            //cds4.SetTriggerState(TriggerMappings.LeftTrigger, 255);
            //cds4.SetTriggerState(TriggerMappings.RightTrigger, 255);

            //DELETE ME
            //await userDs4.PressButton(ButtonMappings.Options);

            //float? lastBrightness = null;
            //while (true)
            //{
            //    //await delayer.Delay(1000);
            //    //var pos = pr.GetCursorLocation();
            //    float brightness = pr.GetColorAtLocation(new Point(2725, 163)).GetBrightness();
            //    if (lastBrightness == null)
            //    {
            //        lastBrightness = brightness;
            //    }
            //    else if (lastBrightness != brightness)
            //    {
            //        break;
            //    }
            //    //Console.WriteLine($"{pos.X}, {pos.Y}");
            //}
        }
    }
}