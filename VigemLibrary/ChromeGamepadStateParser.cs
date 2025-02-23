using VigemLibrary.Commands;
using VigemLibrary.Mappings;

namespace VigemLibrary
{
    public class ChromeGamepadStateParser
    {
        public IDictionary<double, IEnumerable<IControllerCommand>> GetStates(string states)
        {
            Dictionary<double, IEnumerable<IControllerCommand>> result = new();

            string[] SplitString(char c, string str) => str.Split(new[] { c }, StringSplitOptions.RemoveEmptyEntries);
            string[] timestamps = SplitString('~', states);
            foreach (string timestamp in timestamps)
            {
                double ts = 0;
                List<IControllerCommand> commands = new();
                string[] fields = SplitString(';', timestamp);
                foreach (string field in fields)
                {
                    string[] fieldData = SplitString(':', field);
                    string key = fieldData[0];
                    string value = fieldData[1];

                    switch (key)
                    {
                        case "b0":
                            commands.Add(GetButtonCommand(ButtonMappings.Cross, value));
                            break;
                        case "b1":
                            commands.Add(GetButtonCommand(ButtonMappings.Circle, value));
                            break;
                        case "b2":
                            commands.Add(GetButtonCommand(ButtonMappings.Square, value));
                            break;
                        case "b3":
                            commands.Add(GetButtonCommand(ButtonMappings.Triangle, value));
                            break;
                        case "b4":
                            commands.Add(GetButtonCommand(ButtonMappings.ShoulderLeft, value));
                            break;
                        case "b5":
                            commands.Add(GetButtonCommand(ButtonMappings.ShoulderRight, value));
                            break;
                        case "b6":
                            commands.Add(GetTriggerCommand(TriggerMappings.LeftTrigger, value));
                            break;
                        case "b7":
                            commands.Add(GetTriggerCommand(TriggerMappings.RightTrigger, value));
                            break;
                        case "b8":
                            commands.Add(GetButtonCommand(ButtonMappings.Share, value));
                            break;
                        case "b9":
                            commands.Add(GetButtonCommand(ButtonMappings.Options, value));
                            break;
                        case "b10":
                            commands.Add(GetButtonCommand(ButtonMappings.ThumbLeft, value));
                            break;
                        case "b11":
                            commands.Add(GetButtonCommand(ButtonMappings.ThumbRight, value));
                            break;
                        case "b12":
                            commands.Add(GetDpadCommand(DPadMappings.Up, value));
                            break;
                        case "b13":
                            commands.Add(GetDpadCommand(DPadMappings.Down, value));
                            break;
                        case "b14":
                            commands.Add(GetDpadCommand(DPadMappings.Left, value));
                            break;
                        case "b15":
                            commands.Add(GetDpadCommand(DPadMappings.Right, value));
                            break;
                        case "b16":
                            ThrowKeyNotImpementedException(key);
                            break;
                        case "b17":
                            ThrowKeyNotImpementedException(key);
                            break;
                        case "a0":
                            commands.Add(GetAxisCommand(AxisMappings.LeftThumbX, value));
                            break;
                        case "a1":
                            commands.Add(GetAxisCommand(AxisMappings.LeftThumbY, value));
                            break;
                        case "a2":
                            commands.Add(GetAxisCommand(AxisMappings.RightThumbX, value));
                            break;
                        case "a3":
                            commands.Add(GetAxisCommand(AxisMappings.RightThumbY, value));
                            break;
                        case "t":
                            ts = double.Parse(value);
                            break;
                    }
                }
                result.Add(ts, commands);
            }
            return result;
        }

        private static IControllerCommand GetTriggerCommand(TriggerMappings mapping, string value)
        {
            return new TriggerCommand(mapping, Convert.ToByte(double.Parse(value) * 255));
        }

        private static void ThrowKeyNotImpementedException(string key)
        {
            throw new NotImplementedException($"{key} command not implemented.");
        }

        private static AxisCommand GetAxisCommand(AxisMappings mapping, string value)
        {
            byte defaultValue = 128;
            double val = double.Parse(value);
            byte result = Convert.ToByte(defaultValue + (val * (defaultValue - (val > 0 ? 1 : 0))));
            return new AxisCommand(mapping, result);
        }

        private static DpadCommand GetDpadCommand(DPadMappings mapping, string value)
        {
            return new DpadCommand(mapping, ValueIsNotZero(value));
        }

        private static ButtonCommand GetButtonCommand(ButtonMappings mapping, string value)
        {
            return new ButtonCommand(mapping, ValueIsNotZero(value));
        }

        private static bool ValueIsNotZero(string value)
        {
            return value != "0";
        }
    }
}
