using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common_ClassLibrary
{
    public static class Globals
    {
        public static string GetDateConcatenatedWithGuid(DateTime time, string guid)
        {
            return $"{time:yyyy-MM-dd--HH-mm-ss}_{guid}";
        }
    }
}
