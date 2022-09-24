using System;
using System.Collections.Generic;
using System.Linq;
using Common_ClassLibrary;

namespace LeagueAPI_ClassLibrary
{
    public class DataCollector
    {
        private readonly IDDragonRepository repository;
        private Dictionary<ITableEntry, TableEntryAndWinLossData<ITableEntry>> entriesDict = new();
        private Dictionary<string, ITableEntry> compsDict = new();

        public DataCollector(IDDragonRepository repository)
        {
            this.repository = repository;
        }

        public DataCollectorResults GetData(List<LeagueMatch> leagueMatches)
        {
            entriesDict = new Dictionary<ITableEntry, TableEntryAndWinLossData<ITableEntry>>();
            compsDict = new Dictionary<string, ITableEntry>();

            foreach (LeagueMatch match in leagueMatches)
            {
                List<Champion> winners = new();
                List<Champion> losers = new();
                
                foreach (Participant participant in match.participants)
                {
                    bool win = participant.win.Value;
                    int champId = participant.championId.Value;
                    AddChampion(champId, win);
                    Champion champ = repository.GetChampion(champId);
                    List<Champion> champList = win ? winners : losers;
                    champList.Add(champ);

                    foreach (int id in new List<int>
                    {
                        participant.item0.Value,
                        participant.item1.Value,
                        participant.item2.Value,
                        participant.item3.Value,
                        participant.item4.Value,
                        participant.item5.Value,
                        participant.item6.Value,
                    })
                    {
                        AddItem(id, win);
                    }

                    foreach (int id in new List<int>
                    {
                        participant.perk1_1.Value,
                        participant.perk1_2.Value,
                        participant.perk1_3.Value,
                        participant.perk1_4.Value,
                        participant.perk2_1.Value,
                        participant.perk2_2.Value,
                    })
                    {
                        AddRune(id, win);
                    }
                    
                    foreach (int id in new List<int>
                    {
                        participant.statPerkDefense.Value,
                        participant.statPerkFlex.Value,
                        participant.statPerkOffense.Value,
                    })
                    {
                        AddStatPerk(id, win);
                    }
                    
                    foreach (int id in new List<int>
                    {
                        participant.summoner1Id.Value,
                        participant.summoner2Id.Value
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

        private void AddCompToDict(List<Champion> champs, bool win)
        {
            string comp = GetCompFromChampionList(champs);
            if (!compsDict.ContainsKey(comp))
            {
                compsDict.Add(comp, new TeamComposition(comp));
            }
            AddOrUpdateEntry(win, compsDict[comp]);
        }

        private static string GetCompFromChampionList(List<Champion> champs)
        {
            IEnumerable<string> roles = champs.Select(champ => champ.GetFirstTag()).ToList().OrderBy(r => r);
            return roles.ConcatenateListOfStringsToCommaAndSpaceString();
        }

        private List<TableEntryAndWinLossData<ITableEntry>> GetSortedEntries()
        {
            List<TableEntryAndWinLossData<ITableEntry>> entries = entriesDict.Values.ToList();
            List<TableEntryAndWinLossData<ITableEntry>> sortedEntries = new();

            List<TableEntryAndWinLossData<Champion>> champs = new();
            List<TableEntryAndWinLossData<Item>> items = new();
            List<TableEntryAndWinLossData<Rune>> runes = new();
            List<TableEntryAndWinLossData<StatPerk>> statPerks = new();
            List<TableEntryAndWinLossData<Spell>> spells = new();
            List<TableEntryAndWinLossData<TeamComposition>> comps = new();
            foreach (TableEntryAndWinLossData<ITableEntry> x in entries)
            {
                ITableEntry entry = x.GetEntry();
                WinLossData winLossData = x.GetWinLossData();
                if (entry is Champion c) champs.Add(new TableEntryAndWinLossData<Champion>(c, winLossData));
                else if (entry is Item i) items.Add(new TableEntryAndWinLossData<Item>(i, winLossData));
                else if (entry is Rune r) runes.Add(new TableEntryAndWinLossData<Rune>(r, winLossData));
                else if (entry is StatPerk p) statPerks.Add(new TableEntryAndWinLossData<StatPerk>(p, winLossData));
                else if (entry is Spell s) spells.Add(new TableEntryAndWinLossData<Spell>(s, winLossData));
                else if (entry is TeamComposition tc) comps.Add(new TableEntryAndWinLossData<TeamComposition>(tc, winLossData));
            }

            champs = champs.OrderBy(c => c.GetEntry().Name).ToList();
            items = items
                .OrderByDescending(c => c.GetEntry().IsMythic())
                .ThenByDescending(c => c.GetEntry().IsFinished())
                .ThenByDescending(c => c.GetEntry().IsMoreThan2000G())
                .ThenByDescending(c => c.GetWinLossData().GetWinRate())
                .ToList();
            runes = runes
                .OrderBy(c => c.GetEntry().GetTree())
                .ThenBy(c => c.GetEntry().GetSlot())
                .ThenByDescending(c => c.GetWinLossData().GetWinRate())
                .ToList();
            statPerks = statPerks.OrderByDescending(c => c.GetWinLossData().GetWinRate()).ToList();
            spells = spells.OrderByDescending(c => c.GetWinLossData().GetWinRate()).ToList();
            comps = comps.OrderByDescending(c => c.GetWinLossData().GetWinRate()).ToList();

            AddEntriesToSortedEntries(champs, sortedEntries);
            AddEntriesToSortedEntries(items, sortedEntries);
            AddEntriesToSortedEntries(runes, sortedEntries);
            AddEntriesToSortedEntries(statPerks, sortedEntries);
            AddEntriesToSortedEntries(spells, sortedEntries);
            AddEntriesToSortedEntries(comps, sortedEntries);

            return sortedEntries;
        }

        private static void AddEntriesToSortedEntries<T>(
            IEnumerable<TableEntryAndWinLossData<T>> entries,
            ICollection<TableEntryAndWinLossData<ITableEntry>> sortedEntries
        ) where T : ITableEntry
        {
            foreach (TableEntryAndWinLossData<T> e in entries)
            {
                sortedEntries.Add(new TableEntryAndWinLossData<ITableEntry>(e.GetEntry(), e.GetWinLossData()));
            }
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

        private void AddEntryUsingFunc(
            int id,
            Func<int, ITableEntry> entryRetrieveFunc,
            bool win
        )
        {
            if (id == 0) return;
            ITableEntry entry = entryRetrieveFunc.Invoke(id);
            if (entry == null) return;
            AddOrUpdateEntry(win, entry);
        }

        private void AddOrUpdateEntry(bool win, ITableEntry entry)
        {
            if (!entriesDict.ContainsKey(entry))
            {
                entriesDict.Add(entry, new TableEntryAndWinLossData<ITableEntry>(entry, new WinLossData()));
            }

            TableEntryAndWinLossData<ITableEntry> dictEntry = entriesDict[entry];
            WinLossData winLossData = dictEntry.GetWinLossData();
            if (win) winLossData.AddWin();
            else winLossData.AddLoss();
        }
    }
}