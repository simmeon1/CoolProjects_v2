using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class Flight_UnitTests
    {
        private readonly Airport airport1 = new("ABZ", country: "UK");
        private readonly Airport airport2 = new("EDI", country: "UK");
        private Flight flight;
        
        [TestInitialize]
        public void TestInitialize()
        {
            flight = new(new DateTime(2000, 10, 10, 10, 20, 30), new DateTime(2000, 11, 11, 11, 30, 40), "easyJet", new TimeSpan(3, 5, 10), airport1, airport2, 25);
        }
        
        [TestMethod]
        public void ToStringIsCorrect()
        {
            Assert.IsTrue(flight.ToString().Equals(@"ABZ-EDI - 10/10/2000 10:20:30 - 11/11/2000 11:30:40 - easyJet - 03:05:10 - 25"));
        }
        
        [TestMethod]
        public void GetDepartingAirportIsCorrect()
        {
            Assert.IsTrue(flight.GetDepartingAirport().Equals(airport1));
        }
        
        [TestMethod]
        public void GetArrivingAirportIsCorrect()
        {
            Assert.IsTrue(flight.GetArrivingAirport().Equals(airport2));
        }
    }
}
