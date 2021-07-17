﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class WinLossData
    {
        private int Wins { get; set; }
        private int Losses { get; set; }
        public int GetWins()
        {
            return Wins;
        }
        
        public int GetLosses()
        {
            return Losses;
        }
        
        public void AddWin()
        {
            Wins++;
        }
        
        public void AddLoss()
        {
            Losses++;
        }

        public int GetTotal()
        {
            return Wins + Losses;
        }
        
        public double GetWinRate()
        {
            int total = GetTotal();
            if (total == 0) return 0;
            return ((double)Wins / total) * 100;
        }
    }
}
