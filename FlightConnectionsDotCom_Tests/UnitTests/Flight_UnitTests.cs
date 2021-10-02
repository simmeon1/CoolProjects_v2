using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class Flight_UnitTests
    {
        private readonly Flight flight = new(new DateTime(2000, 10, 10, 10, 20, 30), new DateTime(2000, 11, 11, 11, 30, 40), "easyJet", new TimeSpan(3, 5, 10), "ABZ-EDI", 25);
        
        [TestMethod]
        public void ToStringIsCorrect()
        {
            Assert.IsTrue(flight.ToString().Equals(@"ABZ-EDI - 10/10/2000 10:20:30 - 11/11/2000 11:30:40 - 25"));
        }
        
        [TestMethod]
        public void GetDepartingAirportIsCorrect()
        {
            Assert.IsTrue(flight.GetDepartingAirport().Equals("ABZ"));
        }
        
        [TestMethod]
        public void GetArrivingAirportIsCorrect()
        {
            Assert.IsTrue(flight.GetArrivingAirport().Equals("EDI"));
        }
    }
}
