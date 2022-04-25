using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common_ClassLibrary;
using Newtonsoft.Json;
using SharpDX.DirectInput;

namespace AutoInput
{
    public partial class AutoInput : Form
    {
        private Joystick controllerHandle;
        private List<ControllerState> controllerHandleStates = new();
        private WindowsNativeMethods nativeMethods = new();
        private readonly DualshockControllerWrapper controller = new();
        private readonly Dictionary<string, bool> keysPressed = new();
        private readonly DateTime startTime = DateTime.Now;
        private ActionPlayer actionPlayer;
        private IDelayer delayer = new RealDelayer();

        public AutoInput()
        {
            InitializeComponent();
        }

        private async void AutoInput_Shown(object sender, EventArgs e)
        {
            actionPlayer = new ActionPlayer(delayer, new WindowsNativeMethods(), controller);
            await GetControllerHandle();
        }

        private void UpdateControllerState()
        {
            return;
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
            // states.Add(state);
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

        private void AutoInput_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyData.ToString();
            if (keysPressed.ContainsKey(key) && keysPressed[key]) return;
            keysPressed[key] = true;
            UpdateControllerState();
        }

        private void AutoInput_KeyUp(object sender, KeyEventArgs e)
        {
            keysPressed[e.KeyData.ToString()] = false;
            UpdateControllerState();
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

        private void recordStatesTimer_Tick(object sender, EventArgs e)
        {
            double timestamp = (DateTime.Now - startTime).TotalMilliseconds;
            JoystickState currentDeviceState = controllerHandle.GetCurrentState();

            bool[] buttons = currentDeviceState.Buttons;
            int[] arrows = currentDeviceState.PointOfViewControllers;
            ControllerState newControllerState = new()
            {
                A0 = (short) (short.MinValue + currentDeviceState.X),
                A1 = (short) (short.MinValue + currentDeviceState.Y),
                A2 = (short) (short.MinValue + currentDeviceState.Z),
                A3 = (short) (short.MinValue + currentDeviceState.RotationZ),
                B0 = buttons[1],
                B1 = buttons[2],
                B2 = buttons[0],
                B3 = buttons[3],
                B4 = buttons[4],
                B5 = buttons[5],
                B6 = buttons[6],
                B7 = buttons[7],
                B8 = buttons[8],
                B9 = buttons[9],
                B10 = buttons[10],
                B11 = buttons[11],
                B12 = arrows[0] == 0,
                B13 = arrows[0] == 18000,
                B14 = arrows[0] == 27000,
                B15 = arrows[0] == 9000,
                // B16 = KeyIsPressed("G"),
                // B17 = KeyIsPressed("G"),
                TIMESTAMP = timestamp,
            };

            ControllerState previousControllerState = controllerHandleStates.LastOrDefault();
            if (previousControllerState != null && StatesAreTheSame(
                newControllerState,
                previousControllerState,
                ignoreSticksButton.Checked
            )) return;
            controllerHandleStates.Add(newControllerState);
            Log("Controller state added.");
        }

        private static bool StatesAreTheSame(ControllerState state1, ControllerState state2, bool ignoreSticks)
        {
            if (
                state1.B0 != state2.B0 ||
                state1.B1 != state2.B1 ||
                state1.B2 != state2.B2 ||
                state1.B3 != state2.B3 ||
                state1.B4 != state2.B4 ||
                state1.B5 != state2.B5 ||
                state1.B6 != state2.B6 ||
                state1.B7 != state2.B7 ||
                state1.B8 != state2.B8 ||
                state1.B9 != state2.B9 ||
                state1.B10 != state2.B10 ||
                state1.B11 != state2.B11 ||
                state1.B12 != state2.B12 ||
                state1.B13 != state2.B13 ||
                state1.B14 != state2.B14 ||
                state1.B15 != state2.B15 ||
                (
                    !ignoreSticks && (
                        state1.A0 != state2.A0 ||
                        state1.A1 != state2.A1 ||
                        state1.A2 != state2.A2 ||
                        state1.A3 != state2.A3
                    )
                )
            )
            {
                return false;
            }
            return true;
        }

        private void Log(string message)
        {
            logListBox.Items.Add(message);
            logListBox.TopIndex = logListBox.Items.Count - 1;
        }

        private async Task GetControllerHandle()
        {
            resetControllerHandleButton.Enabled = false;
            controllerHandle?.Unacquire();
            Log("Getting controller handle...");
            DirectInput directInput = new();
            // IList<DeviceInstance> firstDevices = directInput.GetDevices();
            // while (true)
            // {
            //     IList<DeviceInstance> secondDevices = directInput.GetDevices();
            //     foreach (DeviceInstance device in secondDevices)
            //     {
            //         if (!firstDevices.Any(d => d.InstanceGuid.Equals(device.InstanceGuid)))
            //         {
            //             return new Joystick(directInput, device.InstanceGuid);
            //         }
            //     }
            // }

            while (true)
            {
                DeviceInstance device = directInput
                    .GetDevices()
                    .FirstOrDefault(d => d.InstanceName.Equals("Wireless Controller"));
                if (device != null)
                {
                    controllerHandle = new Joystick(directInput, device.InstanceGuid);
                    controllerHandle.Acquire();
                    Log("Controller handle received.");
                    break;
                }
                await delayer.Delay(1000);
            }
            resetControllerHandleButton.Enabled = true;
        }

        private static void WaitUntilStateIsUpdate(string deviceState, Joystick controllerHandle)
        {
            while (deviceState.Equals(controllerHandle.GetCurrentState().ToString()))
            {
                var x = 1;
            }
        }

        private async void loadActionsButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new();
            dialog.Filter = "JSON|*.json";
            if (dialog.ShowDialog() != DialogResult.OK) return;

            string actionJson = await File.ReadAllTextAsync(dialog.FileName);
            try
            {
                List<Action> actions = actionJson.DeserializeObject<List<Action>>();
                actionsListBox.Items.Clear();
                foreach (Action action in actions) AddAction(action);
            }
            catch (Exception ex)
            {
                Log("Actions could not be loaded.");
            }
        }

        private void AddAction(Action action)
        {
            int index = actionsListBox.Items.Add(action);
            actionsListBox.SetItemChecked(index, action.Enabled);
        }

        private void UpdateAction(int index, Action action)
        {
            actionsListBox.Items[index] = action;
            actionsListBox.SetItemChecked(index, action.Enabled);
        }

        private async void saveActionsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new();
            dialog.Filter = "JSON|*.json";
            if (dialog.ShowDialog() != DialogResult.OK) return;

            List<Action> actions = new();
            for (int i = 0; i < actionsListBox.Items.Count; i++) actions.Add(GetActionAtIndex(i));

            string json = actions.SerializeObject(Formatting.Indented);
            await File.WriteAllTextAsync(dialog.FileName, json);
        }

        private Action GetActionAtIndex(int i)
        {
            return (Action) actionsListBox.Items[i];
        }

        private void moveActionUpButton_Click(object sender, EventArgs e)
        {
            MoveSelectedAction(true);
        }

        private void MoveSelectedAction(bool isUp)
        {
            ListBox.SelectedIndexCollection selectedIndices = actionsListBox.SelectedIndices;
            if (!SingleActionIsSelected(selectedIndices)) return;

            int index = selectedIndices[0];
            if (!ActionCanBeMoved(index, isUp)) return;

            CheckedListBox.ObjectCollection items = actionsListBox.Items;
            int targetIndex = index + (isUp ? -1 : 1);
            bool targetIsChecked = actionsListBox.GetItemChecked(targetIndex);
            object targetItem = items[targetIndex];
            items.RemoveAt(targetIndex);
            items.Insert(index, targetItem);
            actionsListBox.SetItemChecked(index, targetIsChecked);
        }

        private bool ActionCanBeMoved(int index, bool isUp)
        {
            if (isUp && index != 0) return true;
            if (!isUp && index != actionsListBox.Items.Count - 1) return true;
            Log("Cannot move action further.");
            return false;
        }

        private bool SingleActionIsSelected(ListBox.SelectedIndexCollection selectedIndices, bool logMessage = true)
        {
            if (selectedIndices.Count == 1) return true;
            if (logMessage) Log("Please select a single action.");
            return false;
        }

        private void moveActionDownButton_Click(object sender, EventArgs e)
        {
            MoveSelectedAction(false);
        }

        private void addActionButton_Click(object sender, EventArgs e)
        {
            try
            {
                AddAction(addActionTextBox.Text.DeserializeObject<Action>());
            }
            catch (Exception ex)
            {
                Log("Could not add action.");
            }
        }

        private void deleteActionButton_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = actionsListBox.SelectedIndices;
            if (!SingleActionIsSelected(selectedIndices)) return;
            actionsListBox.Items.RemoveAt(selectedIndices[0]);
        }

        private void clearLogButton_Click(object sender, EventArgs e)
        {
            logListBox.Items.Clear();
            logListBox.TopIndex = 0;
        }

        private void actionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateActionTextBox.Text = "";
            ListBox.SelectedIndexCollection selectedIndices = actionsListBox.SelectedIndices;
            if (!SingleActionIsSelected(selectedIndices, false)) return;
            Action action = GetActionAtIndex(selectedIndices[0]);


            updateActionTextBox.Text =
                action.Type == ActionType.SetStates ? "" : action.SerializeObject(Formatting.Indented);
        }

        private void updateActionButton_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = actionsListBox.SelectedIndices;
            if (!SingleActionIsSelected(selectedIndices)) return;
            try
            {
                UpdateAction(selectedIndices[0], updateActionTextBox.Text.DeserializeObject<Action>());
            }
            catch (Exception ex)
            {
                Log("Could not update action.");
            }
        }

        private void actionsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            GetActionAtIndex(e.Index).Enabled = e.NewValue == CheckState.Checked;
        }

        private async void connectControllerButton_Click(object sender, EventArgs e)
        {
            try
            {
                controller.Connect();
                await GetControllerHandle();
            }
            catch (Exception exception)
            {
                Log("Couldn't connect controller.");
            }
        }

        private async void disconnectControllerButton_Click(object sender, EventArgs e)
        {
            try
            {
                controller.Disconnect();
                await GetControllerHandle();
            }
            catch (Exception exception)
            {
                Log("Couldn't disconnect controller.");
            }
        }

        private async void resetControllerHandleButton_Click(object sender, EventArgs e)
        {
            await GetControllerHandle();
        }

        private void recordStatesButton_CheckedChanged(object sender, EventArgs e)
        {
            bool newCheckState = recordStatesButton.Checked;
            if (newCheckState)
            {
                controllerHandleStates.Clear();
            }
            else
            {
                AddAction(
                    new Action()
                    {
                        Type = ActionType.SetStates,
                        Enabled = true,
                        Arguments = new[]
                            {controllerHandleStates.SerializeObject().Replace("false", "0").Replace("true", "1")},
                    }
                );
            }

            recordStatesTimer.Enabled = newCheckState;
        }

        private async void playActionsButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!playActionsButton.Checked)
            {
                playActionsButton.Enabled = false;
                return;
            }
            
            CheckedListBox.CheckedItemCollection checkedActions = actionsListBox.CheckedItems;
            Log($"Playing {checkedActions.Count} actions.");
            for (int i = 0; i < checkedActions.Count; i++)
            {
                if (!playActionsButton.Checked) continue;
                Action action = (Action) checkedActions[i];
                Log($"Playing action {i + 1}.");
                // await Task.Run(async () => await actionPlayer.PlayAction(action));
                await actionPlayer.PlayAction(action);
            }
            Log($"Finished playing actions.");
            playActionsButton.Enabled = true;
        }
    }
}