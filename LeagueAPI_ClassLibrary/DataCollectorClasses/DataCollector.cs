using System;
using System.Collections.Generic;

namespace LeagueAPI_ClassLibrary
{
    public class DataCollector
    {
        public DataCollectorResults GetData(List<LeagueMatch> leagueMatches)
        {
            Dictionary<int, WinLossData> championData = new();
            Dictionary<int, WinLossData> itemData = new();
            Dictionary<int, WinLossData> runeData = new();
            Dictionary<int, WinLossData> runeTreeData = new();
            Dictionary<int, WinLossData> spellData = new();
            Dictionary<int, WinLossData> statPerkData = new();

            foreach (LeagueMatch match in leagueMatches)
            {
                foreach (Participant participant in match.participants)
                {
                    bool win = participant.win.Value;
                    AddDataToDictionary(participant.championId.Value, championData, win);
                    AddDataToDictionary(participant.item0.Value, itemData, win);
                    AddDataToDictionary(participant.item1.Value, itemData, win);
                    AddDataToDictionary(participant.item2.Value, itemData, win);
                    AddDataToDictionary(participant.item3.Value, itemData, win);
                    AddDataToDictionary(participant.item4.Value, itemData, win);
                    AddDataToDictionary(participant.item5.Value, itemData, win);
                    AddDataToDictionary(participant.item6.Value, itemData, win);
                    AddDataToDictionary(participant.perk1_1.Value, runeData, win);
                    AddDataToDictionary(participant.perk1_2.Value, runeData, win);
                    AddDataToDictionary(participant.perk1_3.Value, runeData, win);
                    AddDataToDictionary(participant.perk1_4.Value, runeData, win);
                    AddDataToDictionary(participant.perk2_1.Value, runeData, win);
                    AddDataToDictionary(participant.perk2_2.Value, runeData, win);
                    AddDataToDictionary(participant.perk1_1.Value, runeData, win);
                    AddDataToDictionary(participant.perk1_2.Value, runeData, win);
                    AddDataToDictionary(participant.perk1_3.Value, runeData, win);
                    AddDataToDictionary(participant.perk1_4.Value, runeData, win);
                    AddDataToDictionary(participant.perk2_1.Value, runeData, win);
                    AddDataToDictionary(participant.perk2_2.Value, runeData, win);
                    AddDataToDictionary(participant.perkTree_1.Value, runeTreeData, win);
                    AddDataToDictionary(participant.perkTree_2.Value, runeTreeData, win);
                    AddDataToDictionary(participant.summoner1Id.Value, spellData, win);
                    AddDataToDictionary(participant.summoner2Id.Value, spellData, win);
                    AddDataToDictionary(participant.statPerkDefense.Value, statPerkData, win);
                    AddDataToDictionary(participant.statPerkFlex.Value, statPerkData, win);
                    AddDataToDictionary(participant.statPerkOffense.Value, statPerkData, win);
                }
            }
            return new DataCollectorResults(championData, itemData, runeData, runeTreeData, spellData, statPerkData);
        }

        private void AddDataToDictionary(int id, Dictionary<int, WinLossData> dict, bool win)
        {
            if (id == 0) return;
            if (!dict.ContainsKey(id)) dict.Add(id, new());
            if (win) dict[id].AddWin(); else dict[id].AddLoss();
        }
    }
}