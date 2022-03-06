using System;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.JourneyRetrievers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Classes
{
    [TestClass]
    public class JourneyUnitTests
    {
        private readonly Journey journey = new(new DateTime(2000, 10, 10, 10, 20, 30), new DateTime(2000, 11, 11, 11, 30, 40), "easyJet", new TimeSpan(3, 5, 10), "ABZ-EDI", 25);
        
        [TestMethod]
        public void ToStringIsCorrect()
        {
            Assert.IsTrue(journey.ToString().Equals(@$"ABZ-EDI - 10/10/2000 10:20:30 - 11/11/2000 11:30:40 - easyJet - 03:05:10 - 25 - {nameof(GoogleFlightsWorker)}"));
        }
        
        [TestMethod]
        public void GetDepartingAirportIsCorrect()
        {
            Assert.IsTrue(journey.GetDepartingLocation().Equals("ABZ"));
        }
        
        [TestMethod]
        public void GetArrivingAirportIsCorrect()
        {
            Assert.IsTrue(journey.GetArrivingLocation().Equals("EDI"));
        }
    }
}
