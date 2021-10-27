using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class Parameters
    {
        public string AccountName { get; set; }
        public string AccountPuuid { get; set; }
        public string Token { get; set; }
        public List<string> RangeOfTargetVersions { get; set; }
        public string DdragonJsonFilesDirectoryPath { get; set; }
        public string OutputDirectory { get; set; }
        public int QueueId { get; set; }
        public int MaxCount { get; set; }
        public List<int> IncludeWinRatesForMinutes { get; set; }
        public string ExistingMatchesFile { get; set; }
    }
}
