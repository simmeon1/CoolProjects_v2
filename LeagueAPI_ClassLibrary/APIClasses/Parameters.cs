﻿using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class Parameters
    {
        public string AccountPuuid { get; set; }
        public string Token { get; set; }
        public List<string> RangeOfTargetVersions { get; set; }
        public string OutputDirectory { get; set; }
        public string MatchId { get; set; }
        public int MaxCount { get; set; }
        public string ExistingMatchesFile { get; set; }
    }
}
