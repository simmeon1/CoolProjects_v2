﻿using System;
using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class SpectatorData
    {
        public List<SpectatedParticipant> participants { get; set; }
        public SpectatorData(List<SpectatedParticipant> participants)
        {
            this.participants = participants;
        }
    }
}