using Vigem_ClassLibrary.Commands;
using Vigem_ClassLibrary.Mappings;

namespace Vigem_ClassLibrary
{
    public class ChromeGamepadStateParser
    {
        public IDictionary<decimal, IEnumerable<IControllerCommand>> GetStates(string states)
        {
            Dictionary<decimal, IEnumerable<IControllerCommand>> result = new();

            string[] timestamps = SplitString('~', states);
            foreach (string timestamp in timestamps)
            {
                decimal ts = 0;
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
                            ThrowKeyNotImpementedException(key);
                            break;
                        case "b7":
                            ThrowKeyNotImpementedException(key);
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
                            commands.Add(GetDpadCommand(DPadMappings.North));
                            break;
                        case "b13":
                            commands.Add(GetDpadCommand(DPadMappings.South));
                            break;
                        case "b14":
                            commands.Add(GetDpadCommand(DPadMappings.West));
                            break;
                        case "b15":
                            commands.Add(GetDpadCommand(DPadMappings.East));
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
                            ts = decimal.Parse(value);
                            break;
                    }
                }
                result.Add(ts, commands);
            }
            return result;
        }

        private static void ThrowKeyNotImpementedException(string key)
        {
            throw new NotImplementedException($"{key} command not implemented.");
        }

        private static AxisCommand GetAxisCommand(AxisMappings mapping, string value)
        {
            decimal val = decimal.Parse(value) + 1;
            decimal val2 = val * Convert.ToDecimal(127.5) * -1;
            return new AxisCommand(mapping, Convert.ToByte(val2));
        }

        private static DpadCommand GetDpadCommand(DPadMappings mapping)
        {
            return new DpadCommand(mapping);
        }

        private static ButtonCommand GetButtonCommand(ButtonMappings mapping, string value)
        {
            return new ButtonCommand(mapping, value != "0");
        }

        private static string[] SplitString(char separator, string states)
        {
            return states.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
