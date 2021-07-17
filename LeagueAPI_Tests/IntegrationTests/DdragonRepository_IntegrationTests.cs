using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.IntegrationTests
{
    [TestClass]
    public class DdragonRepository_IntegrationTests
    {
        public TestContext TestContext { get; set; }
        private IntegrationTestData IntegrationTestData { get; set; }
        private DdragonRepository Repository { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {

            IntegrationTestData = JsonConvert.DeserializeObject<IntegrationTestData>(File.ReadAllText((string)TestContext.Properties["integrationTestDataPath"]));
            Repository = new(IntegrationTestData.DdragonJsonFilesDirectoryPath);
        }
        
        [TestMethod]
        public void GetChampion_DataIsCorrect()
        {
            Champion champ = Repository.GetChampion(103);
            Assert.IsTrue(champ.Name.Equals("Ahri"));
            Assert.IsTrue(champ.Difficulty == 5);
            Assert.IsTrue(champ.GetTagsString().Equals("Mage, Assassin"));
        }
    }
}