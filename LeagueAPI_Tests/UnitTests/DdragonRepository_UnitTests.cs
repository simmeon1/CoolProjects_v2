using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class DdragonRepository_UnitTests
    {
        private DdragonRepository Repo { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Mock<IFileIO> fileIOMock = new();
            fileIOMock.SetupSequence(x => x.ReadAllText(It.IsAny<string>()))
                .Returns(@"{'data':{'Aatrox':{'key': '266', 'name':'Aatrox','info':{'difficulty':4},'tags':['Fighter','Tank']}}}")
                .Returns(@"{'data':{'1001':{'name':'Boots','description':'rarityMythic<asd>ornnBonus','plaintext':'plaintext','gold':{'total':300},
                            'tags':['Boots', 'b']},'3158':{'name':'s','description':'ornnBonus','plaintext':'s','gold':{'total':300},'tags':['Boots']}}}")
                .Returns(@"[{'name':'Domination','slots':[{'runes':[{'id':8112,'name':'name','longDesc':'long<asd>desc'}]}]}]")
                .Returns(@"{'5008': 'someText'}")
                .Returns(@"{'data':{'SummonerBarrier':{'name':'name','key': '21','description':'desc','cooldown':[180]}}}");
            Repo = new(fileIOMock.Object, "");
        }
        
        [TestMethod]
        public void GetChampion_ExpectedValues()
        {
            Champion obj = Repo.GetChampion(266);
            Assert.IsTrue(obj != null);
            Assert.IsTrue(obj.Name.Equals("Aatrox"));
            Assert.IsTrue(obj.Difficulty == 4);
            Assert.IsTrue(obj.GetTagsString().Equals("Fighter, Tank"));
        }
        
        [TestMethod]
        public void GetChampion_NotFound()
        {
            Champion obj = Repo.GetChampion(0);
            Assert.IsTrue(obj == null);
        }
        
        [TestMethod]
        public void GetItem_ExpectedValues()
        {
            Item obj = Repo.GetItem(1001);
            Assert.IsTrue(obj != null);
            Assert.IsTrue(obj.Name.Equals("Boots"));
            Assert.IsTrue(obj.Description.Equals("rarityMythic<asd>ornnBonus"));
            Assert.IsTrue(obj.GetCleanDescription().Equals("rarityMythicornnBonus"));
            Assert.IsTrue(obj.Plaintext.Equals("plaintext"));
            Assert.IsTrue(obj.IsFinished.Equals(true));
            Assert.IsTrue(obj.Gold.Equals(300));
            Assert.IsTrue(obj.IsMoreThan2000G().Equals(false));
            Assert.IsTrue(obj.IsMythic().Equals(true));
            Assert.IsTrue(obj.GetTagsString().Equals("Boots, b"));
        }
        
        [TestMethod]
        public void GetItem_NotFound()
        {
            Item obj = Repo.GetItem(0);
            Assert.IsTrue(obj == null);
        }

        [TestMethod]
        public void GetRune_ExpectedValues()
        {
            Rune obj = Repo.GetRune(8112);
            Assert.IsTrue(obj != null);
            Assert.IsTrue(obj.Name.Equals("name"));
            Assert.IsTrue(obj.Tree.Equals("Domination"));
            Assert.IsTrue(obj.LongDescription.Equals("long<asd>desc"));
            Assert.IsTrue(obj.GetCleanDescription().Equals("longdesc"));
            Assert.IsTrue(obj.Slot.Equals(0));
        }

        [TestMethod]
        public void GetRune_NotFound()
        {
            Rune obj = Repo.GetRune(0);
            Assert.IsTrue(obj == null);
        }

        [TestMethod]
        public void GetSpell_ExpectedValues()
        {
            Spell obj = Repo.GetSpell(21);
            Assert.IsTrue(obj != null);
            Assert.IsTrue(obj.Name.Equals("name"));
            Assert.IsTrue(obj.Description.Equals("desc"));
            Assert.IsTrue(obj.Cooldown.Equals(180));
        }

        [TestMethod]
        public void GetSpell_NotFound()
        {
            Spell obj = Repo.GetSpell(0);
            Assert.IsTrue(obj == null);
        }
        
        [TestMethod]
        public void GetStatPerk_ExpectedValues()
        {
            string obj = Repo.GetStatPerk(5008);
            Assert.IsTrue(obj != null);
            Assert.IsTrue(obj.Equals("someText"));
        }

        [TestMethod]
        public void GetStatPerk_NotFound()
        {
            string obj = Repo.GetStatPerk(0);
            Assert.IsTrue(obj == null);
        }
    }
}
