// using Common_ClassLibrary;
// using LeagueAPI_ClassLibrary;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Moq;
// using System.Collections.Generic;
// using System.Threading.Tasks;
//
// namespace LeagueAPI_Tests.UnitTests
// {
//     [TestClass]
//     public class MatchCollector_UnitTests
//     {
//         [TestMethod]
//         public void CompareTargetVersionAgainstGameVersion_DoesCorrectComparisons()
//         {
//             MatchCollector collector = new(new Mock<ILeagueAPIClient>().Object, new Logger_Debug(), new Mock<IMatchCollectorEventHandler>().Object);
//             Assert.IsTrue(MatchCollector.CompareTargetVersionAgainstGameVersion(new List<string> { "11.14" }, "11.14") == 0);
//             Assert.IsTrue(MatchCollector.CompareTargetVersionAgainstGameVersion(new List<string> { "11.15" }, "11.14") == 1);
//             Assert.IsTrue(MatchCollector.CompareTargetVersionAgainstGameVersion(new List<string> { "11.13" }, "11.14") == -1);
//             Assert.IsTrue(MatchCollector.CompareTargetVersionAgainstGameVersion(new List<string> { "12.1" }, "11.1") == 1);
//             Assert.IsTrue(MatchCollector.CompareTargetVersionAgainstGameVersion(new List<string> { "11.1" }, "12.1") == -1);
//             Assert.IsTrue(MatchCollector.CompareTargetVersionAgainstGameVersion(new List<string> { "11.3", "11.1" }, "11.2") == 0);
//             Assert.IsTrue(MatchCollector.CompareTargetVersionAgainstGameVersion(new List<string> { "11.1", "11.3" }, "11.2") == 0);
//             Assert.IsTrue(MatchCollector.CompareTargetVersionAgainstGameVersion(new List<string> { "11.1", "11.3" }, "12.2") == -1);
//             Assert.IsTrue(MatchCollector.CompareTargetVersionAgainstGameVersion(new List<string> { "11.1", "11.3" }, "10.2") == 1);
//         }
//
//         [TestMethod]
//         public async Task CollectMatches_CorrectMatchesReturned()
//         {
//             const string matchId1 = "1";
//             const string matchId2 = "2";
//             const string matchId3 = "3";
//             const string matchId4 = "4";
//             const string highVersion = "11.15";
//             const string targetVersion = "11.14";
//             const string lowVersion = "11.13";
//             const int queueId = 450;
//             const string startingPuuid = "startingPuuid";
//             const string newPuuid = "newPuuid";
//             const string repeatingPuuid = "repeatingPuuid";
//
//             LeagueMatch matchWithHighVersion = new();
//             matchWithHighVersion.gameVersion = highVersion;
//             matchWithHighVersion.matchId = matchId1;
//
//             LeagueMatch matchWithCorrectVersion = new();
//             matchWithCorrectVersion.gameVersion = targetVersion;
//             matchWithCorrectVersion.matchId = matchId2;
//             matchWithCorrectVersion.participants = new() { new Participant() { puuid = newPuuid }, new Participant() { puuid = repeatingPuuid }, new Participant() { puuid = repeatingPuuid } };
//
//             LeagueMatch matchWithCorrectVersionAndRepeatingId = new();
//             matchWithCorrectVersionAndRepeatingId.gameVersion = targetVersion;
//             matchWithCorrectVersionAndRepeatingId.matchId = matchId3;
//
//             LeagueMatch matchWIthOutdatedVersion = new();
//             matchWithCorrectVersionAndRepeatingId.gameVersion = lowVersion;
//             matchWithCorrectVersionAndRepeatingId.matchId = matchId4;
//
//             Mock<ILeagueAPIClient> clientMock = new();
//             clientMock.Setup(x => x.GetMatchIds(startingPuuid, queueId).Result).Returns(new List<string>() { matchId1, matchId2, matchId2, matchId3, matchId4 });
//             clientMock.Setup(x => x.GetMatchIds(newPuuid, queueId).Result).Returns(new List<string>());
//             clientMock.Setup(x => x.GetMatchIds(repeatingPuuid, queueId).Result).Returns(new List<string>());
//             clientMock.Setup(x => x.GetMatch(matchId1).Result).Returns(matchWithHighVersion);
//             clientMock.Setup(x => x.GetMatch(matchId2).Result).Returns(matchWithCorrectVersion);
//             clientMock.Setup(x => x.GetMatch(matchId3).Result).Returns(matchWithCorrectVersionAndRepeatingId);
//             clientMock.Setup(x => x.GetMatch(matchId4).Result).Returns(matchWIthOutdatedVersion);
//
//             MatchCollector collector = new(clientMock.Object, new Logger_Debug(), new Mock<IMatchCollectorEventHandler>().Object);
//             List<LeagueMatch> matches = await collector.GetMatches(startingPuuid, new List<string> { targetVersion }, 10, queueId);
//             Assert.IsTrue(matches.Count == 1);
//             Assert.IsTrue(matches[0].matchId.Equals(matchId2));
//         }
//
//         [TestMethod]
//         public async Task CollectMatches_ExceptionThrown()
//         {
//             const string matchId2 = "2";
//             const string targetVersion = "11.14";
//             const int queueId = 450;
//             const string startingPuuid = "startingPuuid";
//
//             LeagueMatch matchWithCorrectVersion = new();
//             matchWithCorrectVersion.gameVersion = targetVersion;
//             matchWithCorrectVersion.matchId = matchId2;
//             matchWithCorrectVersion.participants = new() { null };
//
//             Mock<ILeagueAPIClient> clientMock = new();
//             clientMock.Setup(x => x.GetMatchIds(startingPuuid, queueId).Result).Returns(new List<string>() { matchId2 });
//             clientMock.Setup(x => x.GetMatch(matchId2).Result).Returns(matchWithCorrectVersion);
//
//             MatchCollector collector = new(clientMock.Object, new Logger_Debug(), new Mock<IMatchCollectorEventHandler>().Object);
//             List<LeagueMatch> matches = await collector.GetMatches(startingPuuid, new List<string> { targetVersion }, 10, queueId);
//             Assert.IsTrue(matches.Count == 1);
//             Assert.IsTrue(matches[0].matchId.Equals(matchId2));
//         }
//
//         [TestMethod]
//         public async Task CollectMatches_OnlyOneMatch()
//         {
//             const string matchId2 = "2";
//             const string targetVersion = "11.14";
//             const int queueId = 450;
//             const string startingPuuid = "startingPuuid";
//
//             LeagueMatch matchWithCorrectVersion = new();
//             matchWithCorrectVersion.gameVersion = targetVersion;
//             matchWithCorrectVersion.matchId = matchId2;
//             matchWithCorrectVersion.participants = new() { new Participant() };
//
//             Mock<ILeagueAPIClient> clientMock = new();
//             clientMock.Setup(x => x.GetMatchIds(startingPuuid, queueId).Result).Returns(new List<string>() { matchId2 });
//             clientMock.Setup(x => x.GetMatch(matchId2).Result).Returns(matchWithCorrectVersion);
//
//             MatchCollector collector = new(clientMock.Object, new Logger_Debug(), new Mock<IMatchCollectorEventHandler>().Object);
//             List<LeagueMatch> matches = await collector.GetMatches(startingPuuid, new List<string> { targetVersion }, 1, queueId);
//             Assert.IsTrue(matches.Count == 1);
//             Assert.IsTrue(matches[0].matchId.Equals(matchId2));
//         }
//         
//         [TestMethod]
//         public async Task CollectMatches_OnlyOneMatch_MatchesProvided()
//         {
//             const string matchId = "2";
//             const string targetVersion = "11.14";
//             const int queueId = 450;
//             const string startingPuuid = "startingPuuid";
//
//             LeagueMatch match = new();
//             match.gameVersion = targetVersion;
//             match.matchId = matchId;
//             match.participants = new() { new Participant() };
//
//             Mock<ILeagueAPIClient> clientMock = new();
//             clientMock.Setup(x => x.GetMatchIds(startingPuuid, queueId).Result).Returns(new List<string>() { matchId });
//             clientMock.Setup(x => x.GetMatch(matchId).Result).Returns(match);
//
//             MatchCollector collector = new(clientMock.Object, new Logger_Debug(), new Mock<IMatchCollectorEventHandler>().Object);
//             List<LeagueMatch> matches = await collector.GetMatches(startingPuuid, new List<string> { targetVersion }, 1, queueId, new List<LeagueMatch>() { match });
//             Assert.IsTrue(matches.Count == 1);
//             Assert.IsTrue(matches[0].matchId.Equals(matchId));
//         }
//         
//         [TestMethod]
//         public async Task CollectMatches_TwoMatches_MatchesProvided()
//         {
//             const string matchId1 = "1";
//             const string matchId2 = "2";
//             const string targetVersion = "11.14";
//             const int queueId = 450;
//             const string startingPuuid = "startingPuuid";
//
//             LeagueMatch match1 = new();
//             match1.gameVersion = targetVersion;
//             match1.matchId = matchId1;
//             match1.participants = new() { new Participant() { puuid = "p1" } };
//             
//             LeagueMatch match2 = new();
//             match2.gameVersion = targetVersion;
//             match2.matchId = matchId2;
//             match2.participants = new() { new Participant() { puuid = "p2" } };
//
//             Mock<ILeagueAPIClient> clientMock = new();
//             clientMock.Setup(x => x.GetMatchIds(It.IsAny<string>(), queueId).Result).Returns(new List<string>() { matchId1, matchId2 });
//             clientMock.Setup(x => x.GetMatch(matchId1).Result).Returns(match1);
//             clientMock.Setup(x => x.GetMatch(matchId2).Result).Returns(match2);
//
//             MatchCollector collector = new(clientMock.Object, new Logger_Debug(), new Mock<IMatchCollectorEventHandler>().Object);
//             List<LeagueMatch> matches = await collector.GetMatches(startingPuuid, new List<string> { targetVersion }, 10, queueId, new List<LeagueMatch>() { match1 });
//             Assert.IsTrue(matches.Count == 2);
//             Assert.IsTrue(matches[0].matchId.Equals(matchId1));
//             Assert.IsTrue(matches[1].matchId.Equals(matchId2));
//         }
//     }
// }
