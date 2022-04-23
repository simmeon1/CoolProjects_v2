using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common_ClassLibrary;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using ViGEm_Common;
using ViGEm_Console;

namespace ViGEm_Gui
{
    public partial class Form1 : Form
    {
        private DualshockControllerWrapper controller = new();
        private Dictionary<string, bool> keysPressed = new();
        private List<ControllerState> states = new();
        private DateTime startTime = DateTime.Now;

        public Form1()
        {
            InitializeComponent();
            controller.StartController();
        }

        private void UpdateControllerState()
        {
            ControllerState state = new()
            {
                A0 = GetAxisValue("A", "D"),
                A1 = GetAxisValue("W", "S"),
                A2 = GetAxisValue("J", "L"),
                A3 = GetAxisValue("I", "K"),
                B0 = KeyIsPressed("G"),
                B1 = KeyIsPressed("H"),
                B2 = KeyIsPressed("F"),
                B3 = KeyIsPressed("T"),
                B4 = KeyIsPressed("R"),
                B5 = KeyIsPressed("Y"),
                B6 = KeyIsPressed("E"),
                B7 = KeyIsPressed("U"),
                B8 = KeyIsPressed("Q"),
                B9 = KeyIsPressed("P"),
                B10 = KeyIsPressed("X"),
                B11 = KeyIsPressed("M"),
                B12 = KeyIsPressed("OemOpenBrackets"),
                B13 = KeyIsPressed("Oemtilde"),
                B14 = KeyIsPressed("Oem1"),
                B15 = KeyIsPressed("Oem7"),
                // B16 = KeyIsPressed("G"),
                // B17 = KeyIsPressed("G"),
                TIMESTAMP = (DateTime.Now - startTime).TotalMilliseconds,
            };
            controller.SetState(state);
            states.Add(state);
        }

        private short GetAxisValue(string minValueKey, string maxValueKey)
        {
            short value = 0;
            if (KeyIsPressed(minValueKey)) value += short.MinValue;
            if (KeyIsPressed(maxValueKey)) value += short.MaxValue;
            return (short) (value / 2);
        }

        private bool KeyIsPressed(string key)
        {
            return keysPressed.ContainsKey(key) && keysPressed[key];
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyData.ToString();
            if (keysPressed.ContainsKey(key) && keysPressed[key]) return;
            keysPressed[key] = true;
            UpdateControllerState();
        }


        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            keysPressed[e.KeyData.ToString()] = false;
            UpdateControllerState();
        }

        private void copyStatesToClipboardButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(states.SerializeObject().Replace("false", "0").Replace("true", "1"));
        }

        private async void clearStatesButton_Click(object sender, EventArgs e)
        {
            // // ViGEmClient clientt = new();
            // // IDualShock4Controller controller = clientt.CreateDualShock4Controller();
            // // controller.Connect();
            // while (true)
            // {
            //     int millisecondsDelay = 50;
            //     controller.SetDPadDirection(DualShock4DPadDirection.West);
            //     await Task.Delay(millisecondsDelay).ConfigureAwait(false);
            //     controller.SetDPadDirection(DualShock4DPadDirection.None);
            //     await Task.Delay(millisecondsDelay).ConfigureAwait(false);
            // }

            states = new List<ControllerState>();
        }

        private void pixelLogTimer_Tick(object sender, EventArgs e)
        {
            // Point pos = nativeMethods.GetCursorPosition();
            // Point pos = new(4109, 324);
            // string title = "Notepad";
            // IntPtr hwnd = nativeMethods.FindWindowByCaption(title);
            // nativeMethods.GetColorAtWindowLocation(hwnd, pos.X, pos.Y);
            // var pixel = Win32.GetPixelColor(hwnd, x, y);

            // listBox.Items.Clear();
            // listBox.Items.Add($"{pos.X}, {pos.Y}, {nativeMethods.GetColorAtLocation(pos).GetBrightness()}, {pos.X}, {pos.Y}"
            // listBox.Items.Add($"{pos.X}, {pos.Y}, {nativeMethods.GetColorAtLocation(pos).GetBrightness()}, {pos.X}, {pos.Y}, " +
            // $"{nativeMethods.GetColorAtWindowLocation(hwnd, pos.X, pos.Y).GetBrightness()}"
            // $"{nativeMethods.GetColorAtWindowLocation(hwnd, pos.X, pos.Y).GetBrightness()}"
            // );
        }

        private void Log(string message)
        {
            listBox.Items.Add(message);
            listBox.TopIndex = listBox.Items.Count - 1;
        }

        private async void respawnButton_Click(object sender, EventArgs e)
        {
            controller.SetButtonState(DualShock4Button.Square, true);
            await Task.Delay(1000).ConfigureAwait(false);
            controller.SetButtonState(DualShock4Button.Square, false);

            controller.SetDPadDirection(DualShock4DPadDirection.South);
            await Task.Delay(1000).ConfigureAwait(false);
            controller.SetDPadDirection(DualShock4DPadDirection.None);

            controller.SetButtonState(DualShock4Button.Cross, true);
            await Task.Delay(1000).ConfigureAwait(false);
            controller.SetButtonState(DualShock4Button.Cross, false);
        }
    }
}