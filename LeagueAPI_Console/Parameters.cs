using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueAPI_Console
{
    public class Parameters
    {
        public string Token { get; set; }
        public string DdragonJsonFilesDirectoryPath { get; set; }
        public string OutputDirectory { get; set; }
        public string TargetVersion { get; set; }
        public int QueueId { get; set; }
        public string AccountPuuid { get; set; }
        public int MaxCount { get; set; }
    }
}
