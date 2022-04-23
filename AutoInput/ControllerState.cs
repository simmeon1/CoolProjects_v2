using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AutoInput
{
    public class ControllerState
    {
        /** LX */
        public short A0 { get; set; }
        /** LY */
        public short A1 { get; set; }
        /** RX */
        public short A2 { get; set; }
        /** RY */
        public short A3 { get; set; }
        /** x */
        public bool B0 { get; set; }
        /** circle */
        public bool B1 { get; set; }
        /** square */
        public bool B2 { get; set; }
        /** triangle */
        public bool B3 { get; set; }
        /** l1 */
        public bool B4 { get; set; }
        /** r1 */
        public bool B5 { get; set; }
        /** l2 */
        public bool B6 { get; set; }
        /** r2 */
        public bool B7 { get; set; }
        /** share */
        public bool B8 { get; set; }
        /** pause */
        public bool B9 { get; set; }
        /** l3 */
        public bool B10 { get; set; }
        /** r3 */
        public bool B11 { get; set; }
        /** up */
        public bool B12 { get; set; }
        /** down */
        public bool B13 { get; set; }
        /** left */
        public bool B14 { get; set; }
        /** right */
        public bool B15 { get; set; }
        /** ps */
        public bool B16 { get; set; }
        /** touch */
        public bool B17 { get; set; }
        public double TIMESTAMP { get; set; }

        public static ControllerState FromJsonObject(string json)
        {
            return GetParsed(json);
        }
        
        public static List<ControllerState> FromJsonArray(string json)
        {
            List<ControllerState> result = new();
            JArray arr = JArray.Parse(json);
            
            foreach (JToken entry in arr)
            {
                result.Add(GetParsed(entry.ToString()));
            }
            return result;
        }

        private static ControllerState GetParsed(string json)
        {
            JObject obj = JObject.Parse(json);
            ControllerState controllerState = new();
            controllerState.A0 = Convert.ToInt16(double.Parse(obj["A0"].ToString()));
            controllerState.A1 = Convert.ToInt16(double.Parse(obj["A1"].ToString()));
            controllerState.A2 = Convert.ToInt16(double.Parse(obj["A2"].ToString()));
            controllerState.A3 = Convert.ToInt16(double.Parse(obj["A3"].ToString()));
            controllerState.B0 = ToBoolean(obj, "B0");
            controllerState.B1 = ToBoolean(obj, "B1");
            controllerState.B2 = ToBoolean(obj, "B2");
            controllerState.B3 = ToBoolean(obj, "B3");
            controllerState.B4 = ToBoolean(obj, "B4");
            controllerState.B5 = ToBoolean(obj, "B5");
            controllerState.B6 = ToBoolean(obj, "B6");
            controllerState.B7 = ToBoolean(obj, "B7");
            controllerState.B8 = ToBoolean(obj, "B8");
            controllerState.B9 = ToBoolean(obj, "B9");
            controllerState.B10 = ToBoolean(obj, "B10");
            controllerState.B11 = ToBoolean(obj, "B11");
            controllerState.B12 = ToBoolean(obj, "B12");
            controllerState.B13 = ToBoolean(obj, "B13");
            controllerState.B14 = ToBoolean(obj, "B14");
            controllerState.B15 = ToBoolean(obj, "B15");
            controllerState.B16 = ToBoolean(obj, "B16");
            controllerState.B17 = ToBoolean(obj, "B17");
            controllerState.TIMESTAMP = double.Parse(obj["TIMESTAMP"].ToString());
            return controllerState;
        }

        private static bool ToBoolean(JObject obj, string propertyName)
        {
            double value = double.Parse(obj[propertyName].ToString());
            return Convert.ToBoolean(value > 0 ? 1 : 0);
        }
    }
}