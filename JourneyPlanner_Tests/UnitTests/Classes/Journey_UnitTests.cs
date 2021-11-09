using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class Journey_UnitTests
    {
        private readonly Journey journey = new(new DateTime(2000, 10, 10, 10, 20, 30), new DateTime(2000, 11, 11, 11, 30, 40), "easyJet", new TimeSpan(3, 5, 10), "ABZ-EDI", 25);
        
        [TestMethod]
        public void ToStringIsCorrect()
        {
            Assert.IsTrue(journey.ToString().Equals(@"ABZ-EDI - 10/10/2000 10:20:30 - 11/11/2000 11:30:40 - easyJet - 03:05:10 - 25 - Flight"));
        }
        
        [TestMethod]
        public void GetDepartingAirportIsCorrect()
        {
            Assert.IsTrue(journey.GetDepartingAirport().Equals("ABZ"));
        }
        
        [TestMethod]
        public void GetArrivingAirportIsCorrect()
        {
            Assert.IsTrue(journey.GetArrivingAirport().Equals("EDI"));
        }
    }
}
