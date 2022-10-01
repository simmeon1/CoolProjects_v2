using System;
using System.Collections.Generic;
using System.Linq;
using Common_ClassLibrary;

namespace LeagueAPI_ClassLibrary
{
    public class DataCollector
    {
        private readonly IDDragonRepository repository;
        private readonly Dictionary<string, ITableEntryWithWinLossData> entriesDict = new();

        public DataCollector(IDDragonRepository repository)
        {
            this.repository = repository;
        }

        public DataCollectorResults GetData(List<LeagueMatch> leagueMatches)
        {
            entriesDict.Clear();

            foreach (LeagueMatch match in leagueMatches)
            {
                List<Champion> winners = new();
                List<Champion> losers = new();
                
                foreach (Participant participant in match.participants)
                {
                    bool win = participant.win;
                    int champId = participant.championId;
                    Champion champ = repository.GetChampion(champId);
                    if (champ != null) (win ? winners : losers).Add(champ);
                    AddChampion(champId, win);
                    AddRole(champ, win);

                    foreach (int id in new List<int>
                    {
                        participant.item0,
                        participant.item1,
                        participant.item2,
                        participant.item3,
                        participant.item4,
                        participant.item5,
                        participant.item6,
                    })
                    {
                        AddItem(id, win);
                    }

                    foreach (int id in new List<int>
                    {
                        participant.perk1_1,
                        participant.perk1_2,
                        participant.perk1_3,
                        participant.perk1_4,
                        participant.perk2_1,
                        participant.perk2_2,
                    })
                    {
                        AddRune(id, win);
                    }
                    
                    foreach (int id in new List<int>
                    {
                        participant.statPerkDefense,
                        participant.statPerkFlex,
                        participant.statPerkOffense,
                    })
                    {
                        AddStatPerk(id, win);
                    }
                    
                    foreach (int id in new List<int>
                    {
                        participant.summoner1Id,
                        participant.summoner2Id
                    })
                    {
                        AddSpell(id, win);
                    }
                }
                AddCompToDict(winners, true);
                AddCompToDict(losers, false);
            }

            return new DataCollectorResults(GetSortedEntries());
        }

        private void AddCompToDict(IEnumerable<Champion> champs, bool win)
        {
            IEnumerable<string> roles = champs.Select(champ => champ.GetFirstTag()).ToList().OrderBy(r => r);
            string compFromChampionList = roles.ConcatenateListOfStringsToCommaAndSpaceString();
            if (compFromChampionList.IsNullOrEmpty()) return;
            TeamComposition tc = new(compFromChampionList);
            AddOrUpdateEntry(win, tc);
        }
        
        private void AddRole(Champion champ, bool win)
        {
            if (champ == null) return;
            AddOrUpdateEntry(win, new Role(champ.GetFirstTag()));
        }

        private List<ITableEntry> GetSortedEntries()
        {
            List<ITableEntryWithWinLossData> entries = entriesDict.Values.ToList();
            List<ITableEntry> sortedEntries = new();

            List<TableEntry<Champion>> champs = new();
            List<TableEntry<Item>> items = new();
            List<TableEntry<Rune>> runes = new();
            List<TableEntry<StatPerk>> statPerks = new();
            List<TableEntry<Spell>> spells = new();
            List<TableEntry<TeamComposition>> comps = new();
            List<TableEntry<Role>> roles = new();
            foreach (ITableEntryWithWinLossData entry in entries)
            {
                if (entry is TableEntry<Champion> c) champs.Add(c);
                else if (entry is TableEntry<Item> i) items.Add(i);
                else if (entry is TableEntry<Rune> r) runes.Add(r);
                else if (entry is TableEntry<StatPerk> p) statPerks.Add(p);
                else if (entry is TableEntry<Spell> s) spells.Add(s);
                else if (entry is TableEntry<TeamComposition> tc) comps.Add(tc);
                else if (entry is TableEntry<Role> ro) roles.Add(ro);
            }

            List<IEnumerable<ITableEntry>> lists = new()
            {
                champs.OrderBy(c => c.GetEntry().Name).ToList(),
                items
                    .OrderByDescending(c => c.GetEntry().IsMythic())
                    .ThenByDescending(c => c.GetEntry().IsFinished())
                    .ThenByDescending(c => c.GetEntry().IsMoreThan2000G())
                    .ThenByDescending(c => c.GetWinLossData().GetWinRate())
                    .ThenBy(c => c.GetEntry().GetIdentifier())
                    .ToList(),
                runes
                    .OrderBy(c => c.GetEntry().GetTree())
                    .ThenBy(c => c.GetEntry().GetSlot())
                    .ThenByDescending(c => c.GetWinLossData().GetWinRate())
                    .ThenBy(c => c.GetEntry().GetIdentifier())
                    .ToList(),
                statPerks
                    .OrderByDescending(c => c.GetWinLossData().GetWinRate())
                    .ThenBy(c => c.GetEntry().GetIdentifier())
                    .ToList(),
                spells
                    .OrderByDescending(c => c.GetWinLossData().GetWinRate())
                    .ThenBy(c => c.GetEntry().GetIdentifier())
                    .ToList(),
                comps
                    .OrderByDescending(c => c.GetWinLossData().GetWinRate())
                    .ThenBy(c => c.GetEntry().GetIdentifier())
                    .ToList(),
                roles
                .OrderByDescending(c => c.GetWinLossData().GetWinRate())
                .ThenBy(c => c.GetEntry().GetIdentifier())
                .ToList()
            };
            
            
            foreach (IEnumerable<ITableEntry> list in lists)
            {
                sortedEntries.AddRange(list);
            }

            return sortedEntries;
        }

        private void AddItem(int id, bool win)
        {
            AddEntryUsingFunc(id, (x) => repository.GetItem(x), win);
        }

        private void AddChampion(int id, bool win)
        {
            AddEntryUsingFunc(id, (x) => repository.GetChampion(x), win);
        }

        private void AddRune(int id, bool win)
        {
            AddEntryUsingFunc(id, (x) => repository.GetRune(x), win);
        }
        
        private void AddSpell(int id, bool win)
        {
            AddEntryUsingFunc(id, (x) => repository.GetSpell(x), win);
        }
        
        private void AddStatPerk(int id, bool win)
        {
            AddEntryUsingFunc(id, (x) => repository.GetStatPerk(x), win);
        }

        private void AddEntryUsingFunc<T>(
            int id,
            Func<int, T> entryRetrieveFunc,
            bool win
        ) where T: ITableEntry
        {
            if (id == 0) return;
            T entry = entryRetrieveFunc.Invoke(id);
            if (entry == null) return;
            AddOrUpdateEntry(win, entry);
        }

        private void AddOrUpdateEntry<T>(bool win, T entry) where T: ITableEntry
        {
            string entryId = entry.GetIdentifier();
            if (!entriesDict.ContainsKey(entryId))
            {
                entriesDict.Add(entryId, new TableEntry<T>(entry, new WinLossData()));
            }

            ITableEntryWithWinLossData dictEntry = entriesDict[entryId];
            WinLossData winLossData = dictEntry.GetWinLossData();
            if (win) winLossData.AddWin();
            else winLossData.AddLoss();
        }
    }
}