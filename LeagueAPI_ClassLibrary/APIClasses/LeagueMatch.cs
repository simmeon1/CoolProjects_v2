using System;
using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class LeagueMatch
    {
        public string matchId { get; set; }
        public string gameVersion { get; set; }
        public int? mapId { get; set; }
        public int? queueId { get; set; }
        public TimeSpan duration { get; set; }
        public List<Participant> participants { get; set; }
        public bool GameIsShorterThanOrEqualToMinutes(int minutes)
        {
            return duration.TotalMinutes <= minutes;
        }
    }
}