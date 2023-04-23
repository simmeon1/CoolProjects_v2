using System.Diagnostics;
using Nefarius.ViGEm.Client.Targets;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.SystemImplementations;
using Vigem_Common.Mappings;
using VigemControllers_ClassLibrary;

namespace WinFormsApp2;

public partial class Form1 : Form
{
    private const string Cross = "Cross";
    private const string Circle = "Circle";
    private const string Square = "Square";
    private const string Triangle = "Triangle";
    private const string ShoulderLeft = "ShoulderLeft";
    private const string ShoulderRight = "ShoulderRight";
    private const string LeftTrigger = "LeftTrigger";
    private const string RightTrigger = "RightTrigger";
    private const string Share = "Share";
    private const string Options = "Options";
    private const string ThumbLeft = "ThumbLeft";
    private const string ThumbRight = "ThumbRight";
    private const string UpDpad = "UpDpad";
    private const string DownDpad = "DownDpad";
    private const string LeftDpad = "LeftDpad";
    private const string RightDpad = "RightDpad";
    private const string LeftStickLeft = "LeftStickLeft";
    private const string LeftStickRight = "LeftStickRight";
    private const string LeftStickUp = "LeftStickUp";
    private const string LeftStickDown = "LeftStickDown";
    private const string RightStickLeft = "RightStickLeft";
    private const string RightStickRight = "RightStickRight";
    private const string RightStickUp = "RightStickUp";
    private const string RightStickDown = "RightStickDown";
    
    private readonly ControllerUser userDs4;
    private readonly Stopwatch stopwatch = new();
    private readonly Dictionary<string, Keys> commandsToKeys;
    private readonly Dictionary<Keys, string> keysToCommands;
    private readonly Queue<string> gamepadActions = new();
    
    private double lastElapsedTotalMilliseconds;
    private string lastAction = "";
    private string lastCommand;
    private bool lastIsPressed;

    public Form1()
    {
        InitializeComponent();
        ControllerCreator creator = new();
        IDualShock4Controller createdDs4Controller = creator.GetDualShock4Controller();
        Dualshock4Controller cds4 = new(createdDs4Controller);
        Delayer delayer = new();
        userDs4 = new ControllerUser(cds4, delayer, 500);
        cds4.Connect();
        stopwatch.Start();
        
        commandsToKeys = new Dictionary<string, Keys>
        {
            {Cross, Keys.G},
            {Circle, Keys.H},
            {Square, Keys.F},
            {Triangle, Keys.T},
            {ShoulderLeft, Keys.U},
            {ShoulderRight, Keys.O},
            {LeftTrigger, Keys.D8},
            {RightTrigger, Keys.D9},
            {Share, Keys.R},
            {Options, Keys.Y},
            {ThumbLeft, Keys.X},
            {ThumbRight, Keys.M},
            {UpDpad, Keys.Up},
            {DownDpad, Keys.Down},
            {LeftDpad, Keys.Left},
            {RightDpad, Keys.Right},
            {LeftStickLeft, Keys.A},
            {LeftStickRight, Keys.D},
            {LeftStickUp, Keys.W},
            {LeftStickDown, Keys.S},
            {RightStickLeft, Keys.J},
            {RightStickRight, Keys.L},
            {RightStickUp, Keys.I},
            {RightStickDown, Keys.K},
        };

        keysToCommands = new Dictionary<Keys, string>();
        foreach ((string command, Keys key) in commandsToKeys)
        {
            keysToCommands.Add(key, command);
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
    }

    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
        ButtonStateChanged(e.KeyCode, true);
    }

    private void Form1_KeyUp(object sender, KeyEventArgs e)
    {
        ButtonStateChanged(e.KeyCode, false);
    }

    private void ButtonStateChanged(Keys keyCode, bool isPressed)
    {
        lastElapsedTotalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
        lastCommand = keysToCommands[keyCode];
        lastIsPressed = isPressed;
        if (lastAction == $"{lastCommand}{lastIsPressed}")
        {
            return;
        }

        lastAction = $"{lastCommand}{lastIsPressed}";
        label1.Text = $@"{lastCommand} + {lastIsPressed} + {lastElapsedTotalMilliseconds}";

        bool isStickMaxValue = lastCommand is LeftStickDown or LeftStickRight or RightStickDown or RightStickRight;
        bool isStickMinValue = lastCommand is LeftStickUp or LeftStickLeft or RightStickUp or RightStickLeft;
        bool isStick = isStickMaxValue || isStickMinValue;
        string gamepadValue;
        if (isStick)
        {
            gamepadValue = lastIsPressed
                ? (isStickMaxValue ? "1" : "-1")
                : "0";
        }
        else gamepadValue = lastIsPressed ? "1" : "0";

        gamepadActions.Enqueue($"{lastCommand}:{gamepadValue};t:{lastElapsedTotalMilliseconds}");

        Action<ButtonMappings> buttonAction = lastIsPressed ? userDs4.HoldButton : userDs4.ReleaseButton;
        Action<DPadMappings> dpadAction = lastIsPressed ? userDs4.HoldDPad : userDs4.ReleaseDPad;
        Action<AxisMappings> axisAction = lastIsPressed
            ? (isStickMaxValue ? HoldStickToMaxValue : HoldStickToMinValue)
            : userDs4.ReleaseStick;

        if (lastCommand == Cross) buttonAction.Invoke(ButtonMappings.Cross);
        else if (lastCommand == Circle) buttonAction.Invoke(ButtonMappings.Circle);
        else if (lastCommand == Square) buttonAction.Invoke(ButtonMappings.Square);
        else if (lastCommand == Triangle) buttonAction.Invoke(ButtonMappings.Triangle);
        else if (lastCommand == ShoulderLeft) buttonAction.Invoke(ButtonMappings.ShoulderLeft);
        else if (lastCommand == ShoulderRight) buttonAction.Invoke(ButtonMappings.ShoulderRight);
        // else if (lastCommand == LeftTrigger) buttonAction.Invoke(ButtonMappings.LeftTrigger);
        // else if (lastCommand == RightTrigger) buttonAction.Invoke(ButtonMappings.RightTrigger);
        else if (lastCommand == Share) buttonAction.Invoke(ButtonMappings.Share);
        else if (lastCommand == Options) buttonAction.Invoke(ButtonMappings.Options);
        else if (lastCommand == ThumbLeft) buttonAction.Invoke(ButtonMappings.ThumbLeft);
        else if (lastCommand == ThumbRight) buttonAction.Invoke(ButtonMappings.ThumbRight);
        else if (lastCommand == UpDpad) dpadAction.Invoke(DPadMappings.Up);
        else if (lastCommand == DownDpad) dpadAction.Invoke(DPadMappings.Down);
        else if (lastCommand == LeftDpad) dpadAction.Invoke(DPadMappings.Left);
        else if (lastCommand == RightDpad) dpadAction.Invoke(DPadMappings.Right);
        else if (lastCommand == LeftStickLeft) axisAction.Invoke(AxisMappings.LeftThumbX);
        else if (lastCommand == LeftStickRight) axisAction.Invoke(AxisMappings.LeftThumbX);
        else if (lastCommand == LeftStickUp) axisAction.Invoke(AxisMappings.LeftThumbY);
        else if (lastCommand == LeftStickDown) axisAction.Invoke(AxisMappings.LeftThumbY);
        else if (lastCommand == RightStickLeft) axisAction.Invoke(AxisMappings.RightThumbX);
        else if (lastCommand == RightStickRight) axisAction.Invoke(AxisMappings.RightThumbX);
        else if (lastCommand == RightStickUp) axisAction.Invoke(AxisMappings.RightThumbY);
        else if (lastCommand == RightStickDown) axisAction.Invoke(AxisMappings.RightThumbY);
    }

    private void HoldStickToMaxValue(AxisMappings axis)
    {
        userDs4.HoldStick(axis, byte.MaxValue);
    }

    private void HoldStickToMinValue(AxisMappings axis)
    {
        userDs4.HoldStick(axis, byte.MinValue);
    }
    
    private void CopyCommandsToClipboard(object sender, EventArgs e)
    {
        string joined = string.Join("~", gamepadActions);
        string replaced = joined
            .Replace(Cross, "b0")
            .Replace(Circle, "b1")
            .Replace(Square, "b2")
            .Replace(Triangle, "b3")
            .Replace(ShoulderLeft, "b4")
            .Replace(ShoulderRight, "b5")
            .Replace(LeftTrigger, "b6")
            .Replace(RightTrigger, "b7")
            .Replace(Share, "b8")
            .Replace(Options, "b9")
            .Replace(ThumbLeft, "b10")
            .Replace(ThumbRight, "b11")
            .Replace(UpDpad, "b12")
            .Replace(DownDpad, "b13")
            .Replace(LeftDpad, "b14")
            .Replace(RightDpad, "b15")
            .Replace(LeftStickLeft, "a0")
            .Replace(LeftStickRight, "a0")
            .Replace(LeftStickUp, "a1")
            .Replace(LeftStickDown, "a1")
            .Replace(RightStickLeft, "a2")
            .Replace(RightStickRight, "a2")
            .Replace(RightStickUp, "a3")
            .Replace(RightStickDown, "a3");
        Clipboard.SetText(replaced);
    }

    private void button2_Click(object sender, EventArgs e)
    {
        gamepadActions.Clear();
    }
}