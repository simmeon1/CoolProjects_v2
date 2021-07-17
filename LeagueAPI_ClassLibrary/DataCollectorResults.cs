using System;
using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class DataCollectorResults
    {
        private Dictionary<int, WinLossData> ChampionData { get; set;  }
        private Dictionary<int, WinLossData> ItemData { get; set; }
        private Dictionary<int, WinLossData> RuneData { get; set; }
        private Dictionary<int, WinLossData> RuneTreeData { get; set; }
        private Dictionary<int, WinLossData> SpellData { get; set; }
        private Dictionary<int, WinLossData> StatPerkData { get; set; }
        public DataCollectorResults(
            Dictionary<int, WinLossData> championData, Dictionary<int, WinLossData> itemData, Dictionary<int, WinLossData> runeData, 
            Dictionary<int, WinLossData> runeTreeData, Dictionary<int, WinLossData> spellData, Dictionary<int, WinLossData> statPerkData)
        {
            ChampionData = championData;
            ItemData = itemData;
            RuneData = runeData;
            RuneTreeData = runeTreeData;
            SpellData = spellData;
            StatPerkData = statPerkData;
        }

        public Dictionary<int, WinLossData> GetChampionData()
        {
            return ChampionData;
        }

        public Dictionary<int, WinLossData> GetItemData()
        {
            return ItemData;
        }

        public Dictionary<int, WinLossData> GetRuneData()
        {
            return RuneData;
        }

        public Dictionary<int, WinLossData> GetRuneTreeData()
        {
            return RuneTreeData;
        }

        public Dictionary<int, WinLossData> GetSpellData()
        {
            return SpellData;
        }

        public Dictionary<int, WinLossData> GetStatPerkData()
        {
            return StatPerkData;
        }
    }
}