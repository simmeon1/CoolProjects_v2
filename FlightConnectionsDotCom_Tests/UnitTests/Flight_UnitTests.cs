using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class Flight_UnitTests
    {
        [TestMethod]
        public void ToStringIsCorrect()
        {
            Flight flight = new(new DateTime(2000, 10, 10, 10, 20, 30), new DateTime(2000, 11, 11, 11, 30, 40), "easyJet", new TimeSpan(3, 5, 10), "ABZ-EDI", 25);
            Assert.IsTrue(flight.ToString().Equals(@"ABZ-EDI - 10/10/2000 10:20:30 - 11/11/2000 11:30:40 - 25"));
        }
    }
}
