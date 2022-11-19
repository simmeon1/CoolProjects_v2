// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Common_ClassLibrary;
// using JourneyPlanner_ClassLibrary;
// using JourneyPlanner_ClassLibrary.Classes;
// using JourneyPlanner_ClassLibrary.Interfaces;
// using JourneyPlanner_ClassLibrary.Workers;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Moq;
//
// namespace JourneyPlanner_Tests.UnitTests.Workers
// {
//     [TestClass]
//     public class MultiJourneyCollectorUnitTests
//     {
//         private const string Worker = "worker";
//         private JourneyCollection journeys;
//
//         [TestInitialize]
//         public void TestInitialize()
//         {
//             journeys = new JourneyCollection();
//             journeys.Add(new Journey
//                 (
//                     DateTime.Now,
//                     DateTime.Now,
//                     "company",
//                     new TimeSpan(),
//                     "ABZ-LTN", 20
//                 )
//             );
//         }
//
//         [TestMethod]
//         public async Task CollectsJourneysAsExpected_NoExistingResultsAsync()
//         {
//             Dictionary<string, JourneyRetrieverData> retrieversAndData = GetRetrieversAndData();
//
//             Mock<IJourneyRetriever> journeyRetrieverMock = new();
//             journeyRetrieverMock.Setup(x => x.GetJourneysForDates(
//                 It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<DateTime>>()).Result
//             ).Returns(journeys);
//
//             Mock<IJourneyRetrieverInstanceCreator> instanceCreatorMock = new();
//             instanceCreatorMock.Setup(x => x.CreateInstance(
//                 It.IsAny<string>(), It.IsAny<JourneyRetrieverComponents>())
//             ).Returns(journeyRetrieverMock.Object);
//
//             MultiJourneyCollector c = new(instanceCreatorMock.Object);
//             JourneyRetrieverComponents components = new(
//                 null,
//                 new Mock<ILogger>().Object,
//                 null,
//                 new Mock<IDelayer>().Object,
//                 null,
//                 null
//             );
//             await AssertThatTheOneJourneyIsReturned(c, components, retrieversAndData);
//
//             instanceCreatorMock.Verify(x => x.CreateInstance(
//                 $"JourneyPlanner_ClassLibrary.JourneyRetrievers.{Worker}", components
//             ), Times.Once());
//             journeyRetrieverMock.Verify((x => x.GetJourneysForDates(
//                     It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<DateTime>>()).Result
//                 ), Times.Once());
//         }
//
//         [TestMethod]
//         public async Task CollectsJourneysAsExpected_ThrowsExceptionAsync()
//         {
//             Dictionary<string, JourneyRetrieverData> retrieversAndData = GetRetrieversAndData();
//             retrieversAndData.Add("test", new JourneyRetrieverData(
//                 new List<DirectPath>() {new("LTN", "ABZ")}, new Dictionary<string, string>())
//             );
//
//             Mock<IJourneyRetriever> journeyRetrieverMock = new();
//             journeyRetrieverMock.SetupSequence(x => x.GetJourneysForDates(
//                     It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<DateTime>>()).Result)
//                 .Returns(journeys)
//                 .Throws(new Exception());
//
//             Mock<IJourneyRetrieverInstanceCreator> instanceCreatorMock = new();
//             instanceCreatorMock.Setup(x => x.CreateInstance(It.IsAny<string>(), It.IsAny<JourneyRetrieverComponents>()))
//                 .Returns(journeyRetrieverMock.Object);
//
//             MultiJourneyCollector c = new(instanceCreatorMock.Object);
//             JourneyRetrieverComponents components = new(null, new Mock<ILogger>().Object, null,
//                 new Mock<IDelayer>().Object, null, null);
//
//             await AssertThatTheOneJourneyIsReturned(c, components, retrieversAndData);
//             instanceCreatorMock.Verify(x => x.CreateInstance($"JourneyPlanner_ClassLibrary.JourneyRetrievers.{Worker}", components),
//                 Times.Once());
//             journeyRetrieverMock.Verify(
//                 (x => x.GetJourneysForDates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<DateTime>>()).Result),
//                 Times.Exactly(2));
//         }
//
//         [TestMethod]
//         public async Task CollectsJourneysAsExpected_HasExistingResults_Keep()
//         {
//             Dictionary<string, JourneyRetrieverData> retrieversAndData = GetRetrieversAndData();
//
//             MultiJourneyCollectorResults results = new()
//             {
//                 JourneyCollection = journeys
//             };
//             results.Progress.Add(Worker, new Dictionary<string, bool>());
//             results.Progress[Worker].Add("ABZ-LTN", true);
//
//             MultiJourneyCollector c = new(null);
//             JourneyRetrieverComponents components = new(null, new Mock<ILogger>().Object, null,
//                 new Mock<IDelayer>().Object, null, null);
//             await AssertThatTheOneJourneyIsReturned(c, components, retrieversAndData, results);
//         }
//         
//         [TestMethod]
//         public async Task CollectsJourneysAsExpected_HasExistingResults_Remove()
//         {
//             Dictionary<string, JourneyRetrieverData> retrieversAndData = GetRetrieversAndData();
//             MultiJourneyCollectorResults results = new()
//             {
//                 JourneyCollection = journeys
//             };
//             results.Progress.Add(Worker, new Dictionary<string, bool>());
//             results.Progress[Worker].Add("ABZ-LTN", false);
//
//             Mock<IJourneyRetriever> journeyRetrieverMock = new();
//             journeyRetrieverMock.SetupSequence(x => x.GetJourneysForDates(
//                     It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<DateTime>>()).Result)
//                 .Returns(journeys)
//                 .Throws(new Exception());
//
//             Mock<IJourneyRetrieverInstanceCreator> instanceCreatorMock = new();
//             instanceCreatorMock.Setup(x => x.CreateInstance(It.IsAny<string>(), It.IsAny<JourneyRetrieverComponents>()))
//                 .Returns(journeyRetrieverMock.Object);
//
//             MultiJourneyCollector c = new(instanceCreatorMock.Object);
//             JourneyRetrieverComponents components = new(null, new Mock<ILogger>().Object, null,
//                 new Mock<IDelayer>().Object, null, null);
//
//             await AssertThatTheOneJourneyIsReturned(c, components, retrieversAndData);
//             instanceCreatorMock.Verify(x => x.CreateInstance($"JourneyPlanner_ClassLibrary.JourneyRetrievers.{Worker}", components),
//                 Times.Once());
//             journeyRetrieverMock.Verify(
//                 (x => x.GetJourneysForDates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<DateTime>>()).Result),
//                 Times.Exactly(1));
//         }
//
//         private static Dictionary<string, JourneyRetrieverData> GetRetrieversAndData()
//         {
//             Dictionary<string, JourneyRetrieverData> retrieversAndData = new();
//             JourneyRetrieverData value = new(
//                 new List<DirectPath>() {new("ABZ", "LTN")},
//                 new Dictionary<string, string>());
//             retrieversAndData.Add(Worker, value);
//             return retrieversAndData;
//         }
//
//         private static async Task AssertThatTheOneJourneyIsReturned(MultiJourneyCollector c,
//             JourneyRetrieverComponents components, Dictionary<string, JourneyRetrieverData> retrieversAndData,
//             MultiJourneyCollectorResults results = null)
//         {
//             MultiJourneyCollectorResults collectorResults =
//                 await c.GetJourneys(components, retrieversAndData, new DateTime(), new DateTime().AddDays(1), results);
//             JourneyCollection journeyCollection = collectorResults.JourneyCollection;
//             Assert.IsTrue(journeyCollection.GetCount() == 1);
//             Assert.IsTrue(journeyCollection[0].Cost == 20);
//         }
//         
//         private static async Task AssertThatZeroJourneysAreReturned(MultiJourneyCollector c,
//             JourneyRetrieverComponents components, Dictionary<string, JourneyRetrieverData> retrieversAndData,
//             MultiJourneyCollectorResults results = null)
//         {
//             MultiJourneyCollectorResults collectorResults =
//                 await c.GetJourneys(components, retrieversAndData, new DateTime(), new DateTime().AddDays(1), results);
//             JourneyCollection journeyCollection = collectorResults.JourneyCollection;
//             Assert.IsTrue(journeyCollection.GetCount() == 0);
//         }
//     }
// }