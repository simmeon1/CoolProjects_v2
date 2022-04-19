using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ViGEm
{
    public class HtmlControllerState
    {
        /** LX */
        public double A0 { get; set; }
        /** LY */
        public double A1 { get; set; }
        /** RX */
        public double A2 { get; set; }
        /** RY */
        public double A3 { get; set; }
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

        public static HtmlControllerState FromJsonObject(string json)
        {
            return GetParsed(json);
        }
        
        public static List<HtmlControllerState> FromJsonArray(string json)
        {
            List<HtmlControllerState> result = new();
            JArray arr = JArray.Parse(json);
            
            foreach (JToken entry in arr)
            {
                result.Add(GetParsed(entry.ToString()));
            }
            return result;
        }

        private static HtmlControllerState GetParsed(string json)
        {
            JObject obj = JObject.Parse(json);
            return new HtmlControllerState()
            {
                A0 = double.Parse(obj["A0"].ToString()),
                A1 = double.Parse(obj["A1"].ToString()),
                A2 = double.Parse(obj["A2"].ToString()),
                A3 = double.Parse(obj["A3"].ToString()),
                B0 = ToBoolean(obj, "B0"),
                B1 = ToBoolean(obj, "B1"),
                B2 = ToBoolean(obj, "B2"),
                B3 = ToBoolean(obj, "B3"),
                B4 = ToBoolean(obj, "B4"),
                B5 = ToBoolean(obj, "B5"),
                B6 = ToBoolean(obj, "B6"),
                B7 = ToBoolean(obj, "B7"),
                B8 = ToBoolean(obj, "B8"),
                B9 = ToBoolean(obj, "B9"),
                B10 = ToBoolean(obj, "B10"),
                B11 = ToBoolean(obj, "B11"),
                B12 = ToBoolean(obj, "B12"),
                B13 = ToBoolean(obj, "B13"),
                B14 = ToBoolean(obj, "B14"),
                B15 = ToBoolean(obj, "B15"),
                B16 = ToBoolean(obj, "B16"),
                B17 = ToBoolean(obj, "B17"),
            };
        }

        private static bool ToBoolean(JObject obj, string propertyName)
        {
            return Convert.ToBoolean(int.Parse(obj[propertyName].ToString()));
        }
    }
}