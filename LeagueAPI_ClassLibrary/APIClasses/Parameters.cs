﻿namespace LeagueAPI_ClassLibrary
{
    public class Parameters
    {
        public string AccountName { get; set; }
        public string AccountPuuid { get; set; }
        public string Token { get; set; }
        public string TargetVersion { get; set; }
        public string DdragonJsonFilesDirectoryPath { get; set; }
        public string OutputDirectory { get; set; }
        public int QueueId { get; set; }
        public int MaxCount { get; set; }
    }
}