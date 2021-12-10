﻿using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class MultiJourneyCollector_UnitTests
    {
        private const string worker = "worker";
        private readonly JourneyCollection journeys = new();

        [TestInitialize]
        public void TestInitialize()
        {
            journeys.Add(new Journey(DateTime.Now, DateTime.Now, "company", new TimeSpan(), "ABZ-LTN", 20));
        }

        [TestMethod]
        public async Task CollectsJourneysAsExpected_NoExistingResultsAsync()
        {
            Dictionary<string, JourneyRetrieverData> retrieversAndData = GetRetrieversAndData();

            Mock<IJourneyRetriever> journeyRetrieverMock = new();
            journeyRetrieverMock.Setup(x => x.GetJourneysForDates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<DateTime>>()).Result).Returns(journeys);

            Mock<IJourneyRetrieverInstanceCreator> instanceCreatorMock = new();
            instanceCreatorMock.Setup(x => x.CreateInstance(It.IsAny<string>(), It.IsAny<JourneyRetrieverComponents>())).Returns(journeyRetrieverMock.Object);

            MultiJourneyCollector c = new(instanceCreatorMock.Object);
            JourneyRetrieverComponents components = new(null, new Mock<ILogger>().Object, null, new Mock<IDelayer>().Object, null, null);
            await AssertThatTheOneJourneyIsReturned(c, components, retrieversAndData);

            instanceCreatorMock.Verify(x => x.CreateInstance($"JourneyPlanner_ClassLibrary.{worker}", components), Times.Once());
            journeyRetrieverMock.Verify((x => x.GetJourneysForDates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<DateTime>>()).Result), Times.Once());
        }

        [TestMethod]
        public async Task CollectsJourneysAsExpected_ThrowsExceptionAsync()
        {
            Dictionary<string, JourneyRetrieverData> retrieversAndData = GetRetrieversAndData();
            retrieversAndData.Add("test", new(new List<DirectPath>() { new DirectPath("LTN", "ABZ") }, new()));

            Mock<IJourneyRetriever> journeyRetrieverMock = new();
            journeyRetrieverMock.SetupSequence(x => x.GetJourneysForDates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<DateTime>>()).Result)
                .Returns(journeys)
                .Throws(new Exception());

            Mock<IJourneyRetrieverInstanceCreator> instanceCreatorMock = new();
            instanceCreatorMock.Setup(x => x.CreateInstance(It.IsAny<string>(), It.IsAny<JourneyRetrieverComponents>())).Returns(journeyRetrieverMock.Object);

            MultiJourneyCollector c = new(instanceCreatorMock.Object);
            JourneyRetrieverComponents components = new(null, new Mock<ILogger>().Object, null, new Mock<IDelayer>().Object, null, null);

            await AssertThatTheOneJourneyIsReturned(c, components, retrieversAndData);
            instanceCreatorMock.Verify(x => x.CreateInstance($"JourneyPlanner_ClassLibrary.{worker}", components), Times.Once());
            journeyRetrieverMock.Verify((x => x.GetJourneysForDates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<DateTime>>()).Result), Times.Exactly(2));
        }

        [TestMethod]
        public async Task CollectsJourneysAsExpected_HasExistingResultsAsync()
        {
            Dictionary<string, JourneyRetrieverData> retrieversAndData = GetRetrieversAndData();

            MultiJourneyCollectorResults results = new();
            results.JourneyCollection = journeys;
            results.Progress.Add(worker, new());
            results.Progress[worker].Add("ABZ-LTN", true);

            MultiJourneyCollector c = new(null);
            JourneyRetrieverComponents components = new(null, new Mock<ILogger>().Object, null, new Mock<IDelayer>().Object, null, null);
            await AssertThatTheOneJourneyIsReturned(c, components, retrieversAndData, results);
        }

        private static Dictionary<string, JourneyRetrieverData> GetRetrieversAndData()
        {
            Dictionary<string, JourneyRetrieverData> retrieversAndData = new();
            JourneyRetrieverData value = new(new List<DirectPath>() { new DirectPath("ABZ", "LTN") }, new());
            retrieversAndData.Add(worker, value);
            return retrieversAndData;
        }

        private static async Task AssertThatTheOneJourneyIsReturned(MultiJourneyCollector c, JourneyRetrieverComponents components, Dictionary<string, JourneyRetrieverData> retrieversAndData, MultiJourneyCollectorResults results = null)
        {
            MultiJourneyCollectorResults collectorResults = await c.GetJourneys(components, retrieversAndData, new(), new(), results);
            JourneyCollection journeyCollection = collectorResults.JourneyCollection;
            Assert.IsTrue(journeyCollection.GetCount() == 1);
            Assert.IsTrue(journeyCollection[0].Cost == 20);
        }
    }
}
