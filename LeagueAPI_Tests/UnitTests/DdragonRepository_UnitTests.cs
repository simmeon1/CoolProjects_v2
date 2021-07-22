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
                .Returns(@"{'data':{'1001':{'name':'Boots','description':'desc','plaintext':'plaintext','into':['3158'],'gold':{'total':300},'tags':['Boots']}}}")
                .Returns(@"[{'name':'Domination','slots':[{'runes':[{'id':8112,'name':'name','longDesc':'longdesc'}]}]}]")
                .Returns(@"{'5008': 'someText'}")
                .Returns(@"{'data':{'SummonerBarrier':{'name':'name','description':'desc','cooldown':[180]}}}");
            Repo = new(fileIOMock.Object, "");
        }
        
        [TestMethod]
        public void GetChampion_ExpectedValues()
        {
            var obj = Repo.GetChampion(266);
            Assert.IsTrue(obj != null);
            Assert.IsTrue(obj.Name.Equals("Aatrox"));
            Assert.IsTrue(obj.Difficulty == 4);
            Assert.IsTrue(obj.GetTagsString().Equals("Fighter, Tank"));
        }
        
        [TestMethod]
        public void GetChampion_NotFound()
        {
            var obj = Repo.GetChampion(0);
            Assert.IsTrue(obj == null);
        }
    }
}
