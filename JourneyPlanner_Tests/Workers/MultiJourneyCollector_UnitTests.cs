using Common_ClassLibrary;
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
        [TestMethod]
        public async Task CollectsJourneysAsExpected()
        {
            JourneyCollection journeys = new();
            journeys.Add(new Journey(DateTime.Now, DateTime.Now, "company", new TimeSpan(), "path", 20));
            
            Mock<IJourneyRetriever> journeyRetrieverMock = new();
            journeyRetrieverMock.Setup(x => x.CollectJourneys(It.IsAny<JourneyRetrieverData>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()).Result).Returns(journeys);
                
            Mock<IJourneyRetrieverInstanceCreator> instanceCreatorMock = new();
            instanceCreatorMock.Setup(x => x.CreateInstance(It.IsAny<string>(), It.IsAny<JourneyRetrieverComponents>())).Returns(journeyRetrieverMock.Object);

            JourneyRetrieverComponents components = new();
            MultiJourneyCollector c = new(components, instanceCreatorMock.Object);
            Dictionary<string, JourneyRetrieverData> retrieversAndData = new();
            JourneyRetrieverData value = new(new(), new());
            retrieversAndData.Add("key", value);

            DateTime firstDate = new();
            DateTime lastDate = new();
            JourneyCollection journeyCollection = await c.GetJourneys(retrieversAndData, firstDate, lastDate);
            Assert.IsTrue(journeyCollection.GetCount() == 1);
            Assert.IsTrue(journeyCollection[0].Cost == 20);
            instanceCreatorMock.Verify(x => x.CreateInstance("JourneyPlanner_ClassLibrary.key", components), Times.Once());
            journeyRetrieverMock.Verify(x => x.CollectJourneys(value, firstDate, lastDate), Times.Once());
        }
    }
}
