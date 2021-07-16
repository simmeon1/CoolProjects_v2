using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_Tests
{
    [TestClass]
    public class MatchCollector_Tests
    {
        [TestMethod]
        public void CompareTargetVersionAgainstGameVersion_DoesCorrectComparisons()
        {
            MatchCollector collector = new(new Mock<ILeagueAPIClient>().Object);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.14", "11.14.56") == 0);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.14.56", "11.14") == 0);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.15", "11.14.56") == 1);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.13", "11.14.56") == -1);
            Assert.IsTrue(collector.CompareTargetVersionAgainstGameVersion("11.14", "11.14") == 0);
        }

        [TestMethod]
        public async Task CollectMatches_CorrectMatchesReturned()
        {
            const string matchId1 = "1";
            const string matchId2 = "2";
            const string matchId3 = "3";
            const string matchId4 = "4";
            const string highVersion = "11.15";
            const string targetVersion = "11.14";
            const string lowVersion = "11.13";
            const int queueId = 450;
            const string startingPuuid = "startingPuuid";
            const string newPuuid = "newPuuid";
            const string repeatingPuuid = "repeatingPuuid";

            LeagueMatch matchWithHighVersion = new();
            matchWithHighVersion.gameVersion = highVersion;
            matchWithHighVersion.matchId = matchId1;

            LeagueMatch matchWithCorrectVersion = new();
            matchWithCorrectVersion.gameVersion = targetVersion;
            matchWithCorrectVersion.matchId = matchId2;
            matchWithCorrectVersion.participants = new() { new Participant() { puuid = newPuuid }, new Participant() { puuid = repeatingPuuid }, new Participant() { puuid = repeatingPuuid } };

            LeagueMatch matchWithCorrectVersionAndRepeatingId = new();
            matchWithCorrectVersionAndRepeatingId.gameVersion = targetVersion;
            matchWithCorrectVersionAndRepeatingId.matchId = matchId3;

            LeagueMatch matchWIthOutdatedVersion = new();
            matchWithCorrectVersionAndRepeatingId.gameVersion = lowVersion;
            matchWithCorrectVersionAndRepeatingId.matchId = matchId4;

            Mock<ILeagueAPIClient> clientMock = new();
            clientMock.Setup(x => x.GetMatchIds(queueId, startingPuuid).Result).Returns(new List<string>() { matchId1, matchId2, matchId2, matchId3, matchId4 });
            clientMock.Setup(x => x.GetMatchIds(queueId, newPuuid).Result).Returns(new List<string>());
            clientMock.Setup(x => x.GetMatchIds(queueId, repeatingPuuid).Result).Returns(new List<string>());
            clientMock.Setup(x => x.GetMatch(matchId1).Result).Returns(matchWithHighVersion);
            clientMock.Setup(x => x.GetMatch(matchId2).Result).Returns(matchWithCorrectVersion);
            clientMock.Setup(x => x.GetMatch(matchId3).Result).Returns(matchWithCorrectVersionAndRepeatingId);
            clientMock.Setup(x => x.GetMatch(matchId4).Result).Returns(matchWIthOutdatedVersion);

            MatchCollector collector = new(clientMock.Object);
            List<LeagueMatch> matches = await collector.GetMatches(startingPuuid, targetVersion, queueId);
            Assert.IsTrue(matches.Count == 1);
            Assert.IsTrue(matches[0].matchId.Equals(matchId2));
        }
    }
}
