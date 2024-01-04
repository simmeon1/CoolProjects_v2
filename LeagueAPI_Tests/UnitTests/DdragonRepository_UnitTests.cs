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
        public async Task TestInitialize()
        {
            Mock<ILeagueAPIClient> leagueApiMock = new();
            leagueApiMock.Setup(x => x.GetDdragonChampions(It.IsAny<string>())).ReturnsAsync(
                @"
{
    'data': {
        'Aatrox': {
            'key': '266',
            'name': 'Aatrox',
            'info': {
                'difficulty': 4
            },
            'tags': [
                'Fighter',
                'Tank'
            ]
        }
    }
}"
            );
            leagueApiMock.Setup(x => x.GetDdragonItems(It.IsAny<string>())).ReturnsAsync(
                @"{'data':{'1001':{'name':'Boots','description':'rarityMythic<asd>ornnBonus','plaintext':'plaintext','gold':{'total':300},
                            'tags':['Boots', 'b']},'3158':{'name':'s','description':'360 mana.*<raritylegendary>tearItem</raritylegendary>','plaintext':'s','gold':{'total':300},'tags':['Boots'], 'into':['Boots2', 'Boots3']}}}"
            );
            leagueApiMock.Setup(x => x.GetDdragonRunes(It.IsAny<string>())).ReturnsAsync(
                @"
[
    {
        'key': 'Domination',
        'name': 'Domination',
        'slots': [
            {
                'runes': [
                    {
                        'id': 8112,
                        'name': 'Electrocute',
                        'longDesc': '<s>longDesc1</s>'
                    },
                    {
                        'id': 8124,
                        'name': 'Predator',
                        'longDesc': '<s>longDesc2</s>'
                    }
                ]
            },
            {
                'runes': [
                    {
                        'id': 8126,
                        'name': 'CheapShot',
                        'longDesc': '<s>longDesc3</s>'
                    },
                    {
                        'id': 8139,
                        'name': 'TasteOfBlood',
                        'longDesc': '<s>longDesc4</s>'
                    }
                ]
            }   
        ]
    }
]"
            );
            leagueApiMock.Setup(x => x.GetDdragonStatPerks(It.IsAny<string>())).ReturnsAsync(@"
{
    '5008': '+9 Adaptive Force'
}");
            leagueApiMock.Setup(x => x.GetDdragonSpells(It.IsAny<string>())).ReturnsAsync(
                @"{'data':{'SummonerBarrier':{'name':'name','key': '21','description':'desc','cooldown':[180]}}}"
            );
            Repo = new DdragonRepository(leagueApiMock.Object);
            await Repo.RefreshData("11.24");
        }

        [TestMethod]
        public void GetChampion_ExpectedValues()
        {
            Champion obj = Repo.GetChampion(266);
            Assert.IsTrue(obj != null);
            Assert.IsTrue(obj.Name.Equals("Aatrox"));
            Assert.IsTrue(obj.Difficulty == 4);
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
            Assert.IsTrue(obj.IsFinished().Equals(true));
            Assert.IsTrue(obj.Gold.Equals(300));
            Assert.IsTrue(obj.IsMoreThan2000G().Equals(false));
            Assert.IsTrue(obj.IsMythic().Equals(true));
            Assert.IsTrue(obj.GetTagsString().Equals("Boots, b"));
            Assert.IsTrue(obj.BuildsInto.Count == 0);
            Assert.IsTrue(obj.GetSecondFormNameForTearItem().Equals(""));

            Item obj2 = Repo.GetItem(obj.Name);
            Assert.IsTrue(obj2.Id == obj.Id);

            Item obj3 = Repo.GetItem("blah");
            Assert.IsTrue(obj3 == null);

            Item obj4 = Repo.GetItem(3158);
            Assert.IsTrue(obj4.BuildsInto.Count == 2);
            Assert.IsTrue(obj4.BuildsInto[0].Equals("Boots2"));
            Assert.IsTrue(obj4.BuildsInto[1].Equals("Boots3"));
            Assert.IsTrue(!obj4.IsFinished());
            Assert.IsTrue(obj4.GetSecondFormNameForTearItem().Equals("tearItem"));

            obj4.BuildsInto = null;
            Assert.IsTrue(obj4.IsFinished());

            obj4.BuildsInto = new();
            Assert.IsTrue(obj4.IsFinished());
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
            Rune obj = Repo.GetRune(8139);
            List<KeyValuePair<string, object>> props = obj.GetProperties();
            Assert.AreEqual(4, props.Count);
            Assert.AreEqual("Name", props[0].Key);
            Assert.AreEqual("TasteOfBlood", props[0].Value);
            Assert.AreEqual("Tree", props[1].Key);
            Assert.AreEqual("Domination", props[1].Value);
            Assert.AreEqual("Slot", props[2].Key);
            Assert.AreEqual(1, props[2].Value);
            Assert.AreEqual("Description", props[3].Key);
            Assert.AreEqual("longDesc4", props[3].Value);
            Assert.AreEqual("Domination", obj.GetTree());
            Assert.AreEqual("Runes", obj.GetCategory());
            Assert.AreEqual(1, obj.GetSlot());
        }

        [TestMethod]
        public void GetRune_NotFound()
        {
            Assert.AreEqual(null, Repo.GetRune(0));
        }

        [TestMethod]
        public void GetSpell_ExpectedValues()
        {
            Spell obj = Repo.GetSpell(21);
            Assert.IsTrue(obj != null);
            Assert.IsTrue(obj.Name.Equals("name"));
            Assert.IsTrue(obj.Description.Equals("desc"));
            Assert.IsTrue(obj.Cooldown.Equals("180"));
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
            StatPerk obj = Repo.GetStatPerk(5008);
            List<KeyValuePair<string, object>> props = obj.GetProperties();
            Assert.AreEqual(1, props.Count);
            Assert.AreEqual("Name", props[0].Key);
            Assert.AreEqual("+9 Adaptive Force", props[0].Value);
            Assert.AreEqual("Stat Perks", obj.GetCategory());
        }

        [TestMethod]
        public void GetStatPerk_NotFound()
        {
            Assert.AreEqual(null, Repo.GetStatPerk(0));
        }
    }
}