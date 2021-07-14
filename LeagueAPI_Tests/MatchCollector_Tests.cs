using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueAPI_Tests
{
    [TestClass]
    public class MatchCollector_Tests
    {
        [TestMethod]
        public void CompareTargetVersionAgainstGameVersion_DoesCorrectComparisons()
        {
            MatchCollector collector = new MatchCollector(new Mock<ILeagueAPIClient>().Object);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.14", "11.14.56") == -1);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.13", "11.14.56") == -1);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.15", "11.14.56") == 1);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.14.5", "11.14") == 1);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.13.5", "11.14") == -1);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.15.5", "11.14") == 1);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.15", "11.15.0") == 0);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.15", "11.15") == 0);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("12.0", "11.15") == 1);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.9", "12") == -1);
        }
    }
}
