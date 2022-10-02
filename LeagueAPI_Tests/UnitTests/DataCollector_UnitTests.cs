using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class DataCollector_UnitTests
    {
        private Mock<IDDragonRepository> repo;

        [TestInitialize]
        public void TestInitialize()
        {
            repo = new Mock<IDDragonRepository>();
        }
        
        [TestMethod]
        public void ChampionsAreCorrectlySorted()
        {
            Champion champ1 = SetUpChamp(1, "champd", new List<string> {"tagd"});
            Champion champ2 = SetUpChamp(2, "champc", new List<string> {"tagc"});
            Champion champ3 = SetUpChamp(3, "champb", new List<string> {"tagb"});
            Champion champ4 = SetUpChamp(4, "champa", new List<string> {"taga"});
            LeagueMatch match = new();
            Participant p1 = new()
            {
                championId = 1,
                win = true,
            };
            Participant p2 = new()
            {
                championId = 2,
                win = false
            };
            Participant p3 = new()
            {
                championId = 3,
                win = true,
            };
            Participant p4 = new()
            {
                championId = 4,
                win = false
            };
            match.participants = new List<Participant> { p1, p2, p3, p4 };
            List<LeagueMatch> matches = new() { match };
            List<ITableEntry> tableEntries = GetEntries(matches);
            Assert.AreEqual(10, tableEntries.Count);
            AssertTableEntryMatchesObject(tableEntries[0], champ4, false);
            AssertTableEntryMatchesObject(tableEntries[1], champ3, true);
            AssertTableEntryMatchesObject(tableEntries[2], champ2, false);
            AssertTableEntryMatchesObject(tableEntries[3], champ1, true);
            AssertTableEntryMatchesIdentifier<TeamComposition>(tableEntries[4], "tagb, tagd", true);
            AssertTableEntryMatchesIdentifier<TeamComposition>(tableEntries[5], "taga, tagc", false);
            AssertTableEntryMatchesIdentifier<Role>(tableEntries[6], "tagb", true);
            AssertTableEntryMatchesIdentifier<Role>(tableEntries[7], "tagd", true);
            AssertTableEntryMatchesIdentifier<Role>(tableEntries[8], "taga", false);
            AssertTableEntryMatchesIdentifier<Role>(tableEntries[9], "tagc", false);
        }

        [TestMethod]
        public void TeamCompositionsAndRolesAreCorrectlySorted()
        {
            Champion champ1 = SetUpChamp(1, "champa", new List<string> {"taga"});
            Champion champ2 = SetUpChamp(2, "champb", new List<string> {"tagb"});
            Champion champ3 = SetUpChamp(3, "champc", new List<string> {"tagd"});
            Champion champ4 = SetUpChamp(4, "champd", new List<string> {"tage"});
            Champion champ5 = SetUpChamp(5, "champe", new List<string> {"tagc"});
            Champion champ6 = SetUpChamp(6, "champf", new List<string> {"tagh"});
            LeagueMatch match1 = new();
            LeagueMatch match2 = new();
            Participant p1 = new()
            {
                championId = 1,
                win = true,
            };
            Participant p2 = new()
            {
                championId = 2,
                win = true
            };
            Participant p3 = new()
            {
                championId = 3,
                win = false
            };
            Participant p4 = new()
            {
                championId = 4,
                win = false
            };
            Participant p5 = new()
            {
                championId = 5,
                win = false
            };
            Participant p6 = new()
            {
                championId = 6,
                win = false
            };
            match1.participants = new List<Participant> { p1, p2, p3, p4 };
            match2.participants = new List<Participant> { p5, p6 };
            List<LeagueMatch> matches = new() { match1, match2 };
            List<ITableEntry> tableEntries = GetEntries(matches);
            Assert.AreEqual(15, tableEntries.Count);
            AssertTableEntryMatchesObject(tableEntries[0], champ1, true);
            AssertTableEntryMatchesObject(tableEntries[1], champ2, true);
            AssertTableEntryMatchesObject(tableEntries[2], champ3, false);
            AssertTableEntryMatchesObject(tableEntries[3], champ4, false);
            AssertTableEntryMatchesObject(tableEntries[4], champ5, false);
            AssertTableEntryMatchesObject(tableEntries[5], champ6, false);
            AssertTableEntryMatchesIdentifier<TeamComposition>(tableEntries[6], "taga, tagb", true);
            AssertTableEntryMatchesIdentifier<TeamComposition>(tableEntries[7], "tagc, tagh", false);
            AssertTableEntryMatchesIdentifier<TeamComposition>(tableEntries[8], "tagd, tage", false);
            AssertTableEntryMatchesIdentifier<Role>(tableEntries[9], "taga", true);
            AssertTableEntryMatchesIdentifier<Role>(tableEntries[10], "tagb", true);
            AssertTableEntryMatchesIdentifier<Role>(tableEntries[11], "tagc", false);
            AssertTableEntryMatchesIdentifier<Role>(tableEntries[12], "tagd", false);
            AssertTableEntryMatchesIdentifier<Role>(tableEntries[13], "tage", false);
            AssertTableEntryMatchesIdentifier<Role>(tableEntries[14], "tagh", false);
        }

        [TestMethod]
        public void SpellsAreCorrectlySorted()
        {
            Spell spell1 = SetUpSpell(1, "spellb");
            Spell spell2 = SetUpSpell(2, "spella");
            Spell spell3 = SetUpSpell(3, "spelld");
            Spell spell4 = SetUpSpell(4, "spellc");
            LeagueMatch match = new();
            Participant p1 = new()
            {
                win = true,
                summoner1Id = 1,
                summoner2Id = 2,
            };
            Participant p2 = new()
            {
                win = false,
                summoner1Id = 3,
                summoner2Id = 4,
            };
            match.participants = new List<Participant> { p1, p2 };
            List<LeagueMatch> matches = new() { match };
            List<ITableEntry> tableEntries = GetEntries(matches);
            Assert.AreEqual(4, tableEntries.Count);
            
            AssertTableEntryMatchesObject(tableEntries[0], spell2, true);
            AssertTableEntryMatchesObject(tableEntries[1], spell1, true);
            AssertTableEntryMatchesObject(tableEntries[2], spell4, false);
            AssertTableEntryMatchesObject(tableEntries[3], spell3, false);
        }
        
        [TestMethod]
        public void NullEntryNotAdded()
        {
            repo.Setup(r => r.GetSpell(1)).Returns((Spell) null);
            LeagueMatch match = new();
            Participant p1 = new()
            {
                win = true,
                summoner1Id = 1,
            };
            match.participants = new List<Participant> { p1 };
            List<LeagueMatch> matches = new() { match };
            List<ITableEntry> tableEntries = GetEntries(matches);
            Assert.AreEqual(0, tableEntries.Count);
        }
        
        [TestMethod]
        public void DuplicateEntryHasTwoWins()
        {
            Spell spell = new();
            repo.Setup(r => r.GetSpell(1)).Returns(spell);
            LeagueMatch match = new();
            Participant p1 = new()
            {
                win = true,
                summoner1Id = 1,
                summoner2Id = 1,
            };
            match.participants = new List<Participant> { p1 };
            List<LeagueMatch> matches = new() { match };
            List<ITableEntry> tableEntries = GetEntries(matches);
            Assert.AreEqual(1, tableEntries.Count);
            Assert.AreEqual(true, tableEntries[0] is TableEntry<Spell>);
            TableEntry<Spell> e = tableEntries[0] as TableEntry<Spell>;
            Assert.AreEqual(true, spell == e.GetEntry());
            Assert.AreEqual(2, e.GetWinLossData().GetWins());
            Assert.AreEqual(0, e.GetWinLossData().GetLosses());
        }
        
        [TestMethod]
        public void StatPerksAreCorrectlySorted()
        {
            StatPerk perk1 = SetUpStatPerk(1, "perkf");
            StatPerk perk2 = SetUpStatPerk(2, "perke");
            StatPerk perk3 = SetUpStatPerk(3, "perkd");
            StatPerk perk4 = SetUpStatPerk(4, "perkc");
            StatPerk perk5 = SetUpStatPerk(5, "perkb");
            StatPerk perk6 = SetUpStatPerk(6, "perka");
            LeagueMatch match = new();
            Participant p1 = new()
            {
                win = true,
                statPerkDefense = 1,
                statPerkFlex = 2,
                statPerkOffense = 3,
            };
            Participant p2 = new()
            {
                win = false,
                statPerkDefense = 4,
                statPerkFlex = 5,
                statPerkOffense = 6,
            };
            match.participants = new List<Participant> { p1, p2 };
            List<LeagueMatch> matches = new() { match };
            List<ITableEntry> tableEntries = GetEntries(matches);
            Assert.AreEqual(6, tableEntries.Count);
            AssertTableEntryMatchesObject(tableEntries[0], perk3, true);
            AssertTableEntryMatchesObject(tableEntries[1], perk2, true);
            AssertTableEntryMatchesObject(tableEntries[2], perk1, true);
            AssertTableEntryMatchesObject(tableEntries[3], perk6, false);
            AssertTableEntryMatchesObject(tableEntries[4], perk5, false);
            AssertTableEntryMatchesObject(tableEntries[5], perk4, false);
        }
        
        [TestMethod]
        public void ItemsAreCorrectlySorted()
        {
            Item item1 = SetUpItem(1, "itemh", false, false, false);
            Item item2 = SetUpItem(2, "itemg", false, false, false);
            Item item3 = SetUpItem(3, "itemf", false, false, false);
            Item item4 = SetUpItem(4, "iteme", false, false, false);
            Item item5 = SetUpItem(5, "itemd", false, false, false);
            Item item6 = SetUpItem(6, "itemc", false, true, true);
            Item item7 = SetUpItem(7, "itemb", true, true, true);
            Item item8 = SetUpItem(8, "itema", false, false, false);
            LeagueMatch match = new();
            Participant p1 = new()
            {
                win = true,
                item0 = 1,
                item1 = 2,
                item2 = 3,
                item3 = 4,
                item4 = 5,
                item5 = 6,
                item6 = 7,
            };
            Participant p2 = new()
            {
                win = false,
                item0 = 8,
            };
            match.participants = new List<Participant> { p1, p2 };
            List<LeagueMatch> matches = new() { match };
            List<ITableEntry> tableEntries = GetEntries(matches);
            Assert.AreEqual(8, tableEntries.Count);
            AssertTableEntryMatchesObject(tableEntries[0], item7, true);
            AssertTableEntryMatchesObject(tableEntries[1], item6, true);
            AssertTableEntryMatchesObject(tableEntries[2], item5, true);
            AssertTableEntryMatchesObject(tableEntries[3], item4, true);
            AssertTableEntryMatchesObject(tableEntries[4], item3, true);
            AssertTableEntryMatchesObject(tableEntries[5], item2, true);
            AssertTableEntryMatchesObject(tableEntries[6], item1, true);
            AssertTableEntryMatchesObject(tableEntries[7], item8, false);
        }
        
        [TestMethod]
        public void RunesAreCorrectlySorted()
        {
            Rune rune1 = SetUpRune(1, "runeg", "treeb", 1);
            Rune rune2 = SetUpRune(2, "runef", "treeb", 1);
            Rune rune3 = SetUpRune(3, "runee", "treeb", 1);
            Rune rune4 = SetUpRune(4, "runed", "treeb", 0);
            Rune rune5 = SetUpRune(5, "runec", "treea", 1);
            Rune rune6 = SetUpRune(6, "runeb", "treea", 0);
            Rune rune7 = SetUpRune(7, "runea", "treeb", 1);
            LeagueMatch match = new();
            Participant p1 = new()
            {
                win = true,
                perk1_1 = 1,
                perk1_2 = 2,
                perk1_3 = 3,
                perk1_4 = 4,
                perk2_1 = 5,
                perk2_2 = 6,
            };
            Participant p2 = new()
            {
                win = false,
                perk1_1 = 7,
            };
            match.participants = new List<Participant> { p1, p2 };
            List<LeagueMatch> matches = new() { match };
            List<ITableEntry> tableEntries = GetEntries(matches);
            Assert.AreEqual(7, tableEntries.Count);
            AssertTableEntryMatchesObject(tableEntries[0], rune6, true);
            AssertTableEntryMatchesObject(tableEntries[1], rune5, true);
            AssertTableEntryMatchesObject(tableEntries[2], rune4, true);
            AssertTableEntryMatchesObject(tableEntries[3], rune3, true);
            AssertTableEntryMatchesObject(tableEntries[4], rune2, true);
            AssertTableEntryMatchesObject(tableEntries[5], rune1, true);
            AssertTableEntryMatchesObject(tableEntries[6], rune7, false);
        }

        private static void AssertTableEntryMatchesObject<T>(ITableEntry tableEntry, T obj, bool win) where T: ITableEntry
        {
            Assert.AreEqual(true, tableEntry is TableEntry<T>);
            TableEntry<T> entry = tableEntry as TableEntry<T>;
            Assert.AreEqual(obj, entry.GetEntry());
            Assert.AreEqual(win ? 1 : 0, entry.GetWinLossData().GetWins());
            Assert.AreEqual(win ? 0 : 1, entry.GetWinLossData().GetLosses());
        }
        
        private static void AssertTableEntryMatchesIdentifier<T>(ITableEntry tableEntry, string id, bool win) where T: ITableEntry
        {
            Assert.AreEqual(true, tableEntry is TableEntry<T>);
            TableEntry<T> entry = tableEntry as TableEntry<T>;
            Assert.AreEqual(id, entry.GetEntry().GetIdentifier());
            Assert.AreEqual(win ? 1 : 0, entry.GetWinLossData().GetWins());
            Assert.AreEqual(win ? 0 : 1, entry.GetWinLossData().GetLosses());
        }
        
        private Champion SetUpChamp(int id, string name, List<string> tags)
        {
            Champion champ = new() {Name = name, Tags = tags};
            repo.Setup(r => r.GetChampion(id)).Returns(champ);
            return champ;
        }
        
        private Spell SetUpSpell(int id, string name)
        {
            Spell spell = new() {Name = name};
            repo.Setup(r => r.GetSpell(id)).Returns(spell);
            return spell;
        }
        
        private StatPerk SetUpStatPerk(int id, string name)
        {
            StatPerk perk = new(name);
            repo.Setup(r => r.GetStatPerk(id)).Returns(perk);
            return perk;
        }
        
        private Rune SetUpRune(int id, string name, string tree, int slot)
        {
            Rune rune = new(name, tree, "", slot);
            repo.Setup(r => r.GetRune(id)).Returns(rune);
            return rune;
        }
        
        private Item SetUpItem(int id, string name, bool isMythic, bool isFinished, bool isMoreThan2000G)
        {
            Item item = new()
            {
                Name = name,
                Description = isMythic ? "rarityMythic" : "",
                BuildsInto = isFinished ? null : new List<string>(){ "10" },
                Gold = isMoreThan2000G ? 2001 : 1
            };
            repo.Setup(r => r.GetItem(id)).Returns(item);
            return item;
        }
        
        private List<ITableEntry> GetEntries(List<LeagueMatch> matches)
        {
            DataCollector collector = new(repo.Object);
            DataCollectorResults data = collector.GetData(matches);
            List<ITableEntry> tableEntries = data.GetEntries();
            return tableEntries;
        }
    }
}
